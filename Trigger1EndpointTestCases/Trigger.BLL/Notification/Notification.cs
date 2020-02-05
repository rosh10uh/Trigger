using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Notification
{
    public class Notification
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<Notification> _logger;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Use to initialize notification 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        public Notification(IConnectionContext connectionContext, ILogger<Notification> logger, IOptions<AppSettings> appSettings)
        {
            _connectionContext = connectionContext;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Use to get all notification detail
        /// </summary>
        /// <param name="managerId">Manager id</param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetAllNotification(int managerId)
        {
            try
            {
                NotificationModel notificationModel = new NotificationModel() { managerId = managerId };
                var notification = await Task.FromResult(_connectionContext.TriggerContext.NotificationRepository.GetAllNotification(notificationModel));

                if (notification?.Count > 0)
                    return JsonSettings.UserCustomDataWithStatusMessage(notification, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                else
                    return JsonSettings.UserCustomDataWithStatusMessage(notification, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.noNotifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
            
        }

        /// <summary>
        /// Use to update notification mark as read
        /// </summary>
        /// <param name="ids">Comma separated ids</param>
        /// <returns></returns>
        public virtual async Task<JsonData> UpdateNotificationMarkAsReadAsync(string ids)
        {
            try
            {
                NotificationModel notificationModel = new NotificationModel() { ids = ids };
                var notificationupdate = await Task.FromResult(_connectionContext.TriggerContext.NotificationRepository.InvokeUpdateNotificationStatus(notificationModel));
                if (notificationupdate?.result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        // Methods to send notification to reporting employee

        /// <summary>
        /// Method to send notification to reporting person if any employee is assigned
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="employee"></param>
        public virtual void SendNotifications(int empId, EmployeeModel employee)
        {
            if (employee.ManagerId > 0)
            {
                employee.EmpId = empId;
                InvokeNotificationAsync(employee).Wait();
                EmployeeModel manager = new EmployeeModel() { EmpId = employee.ManagerId };
                manager = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(manager);

                if (manager != null)
                {
                    manager.CreatedBy = manager.EmpId;
                    if (manager.EmpId != 0 && manager.ManagerId > 0)
                    {
                        InvokeNotificationAsync(manager).Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Notification send to reporting person if any employee added/removed or changed 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public virtual async Task InvokeNotificationAsync(EmployeeModel employee)
        {

            NotificationHubClient Hub = NotificationHubClient.CreateClientFromConnectionString(_appSettings.nHubConnectionString, _appSettings.nHubConnection);
            List<Trigger.DTO.UserLoginModel> lstUserLogin = GetDeviceInfoById(GetUserLoginInfo(employee.ManagerId));

            var regId = lstUserLogin.Where(x => x.deviceType == Enums.DeviceType.Android.ToString()).Select(x => x.deviceID).FirstOrDefault();
            var registrations1 = await Hub.GetAllRegistrationsAsync(500);
            var tag = registrations1.Where(x => x.RegistrationId == regId).Select(x => x.Tags);

            var tagname = new List<string>();
            foreach (var tagName in tag)
            {
                tagname.Add(tagName.FirstOrDefault());
            }
            var tags = lstUserLogin.Where(x => x.deviceType == Enums.DeviceType.iOS.ToString()).Select(x => x.deviceID);
            await SendPushNotifications(Hub, tagname, employee.ManagerId, tags);
        }

        /// <summary>
        /// Get Notification by logged in user id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userId"></param>
        /// <returns>notification object</returns>
        private List<Trigger.DTO.NotificationModel> GetNotificationById(int userId)
        {
            EmployeeModel employeeModel = new EmployeeModel() { ManagerId = userId };
            List<Trigger.DTO.NotificationModel> notification = _connectionContext.TriggerContext.EmployeeRepository.GetNotificationById(employeeModel);
            return notification;
        }

        /// <summary>
        /// Method to send notification to mobile device
        /// </summary>
        /// <param name="Hub"></param>
        /// <param name="tagName"></param>
        /// <param name="managerId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private async Task SendPushNotifications(NotificationHubClient Hub, List<string> tagName, int managerId, IEnumerable<string> tags)
        {
            StringBuilder notificationIds = new StringBuilder();
            List<Trigger.DTO.NotificationModel> lstNotification = GetNotificationById(managerId);
            foreach (var notification in lstNotification)
            {
                 if (notification.managerId == managerId)
                {
                    NotificationOutcome outcome = null;
                    if (tagName.Count > 0)
                    {
                        outcome = await SendGCMNotifications(tagName, notification, Hub);
                    }
                    else if (tags.Any())
                    {
                        outcome = await SendIOSNotifications(tags, tags.Count(), notification, Hub);
                    }
                    if (outcome != null)
                    {
                        notificationIds = GetNotificationString(outcome, notificationIds, notification.id);
                    }
                }
            }
            InvokeUpdateNotificationFlagIsSent(notificationIds.ToString());
        }

        /// <summary>
        /// Method to Send notification to Android device
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="notification"></param>
        /// <param name="Hub"></param>
        /// <returns></returns>
        private async Task<NotificationOutcome> SendGCMNotifications(List<string> tagName, Trigger.DTO.NotificationModel notification, NotificationHubClient Hub)
        {
            if (tagName.Count > 0)
            {
                var notif = Messages.notifyForEmployee.ToString().Replace("[0]", notification.message).Replace("[1]", notification.type).Replace("[2]", notification.id.ToString());
                return await Hub.SendGcmNativeNotificationAsync(notif, tagName);
            }
            return null;
        }

        /// <summary>
        /// Method to Send notification to IOS device
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="iosTagCount"></param>
        /// <param name="notification"></param>
        /// <param name="Hub"></param>
        /// <returns></returns>
        private async Task<NotificationOutcome> SendIOSNotifications(IEnumerable<string> tags, int iosTagCount, Trigger.DTO.NotificationModel notification, NotificationHubClient Hub)
        {
            try
            {
                if (iosTagCount > 0)
                {
                    var alert = Messages.alertForEmployee.ToString().Replace("[0]", notification.message).Replace("[1]", notification.type).Replace("[2]", notification.id.ToString());
                    return await Hub.SendAppleNativeNotificationAsync(alert, tags);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message.ToString());
            }
            return null;
        }

        /// <summary>
        /// Method to get Notification string
        /// </summary>
        /// <param name="outcome"></param>
        /// <param name="notificationIds"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        private StringBuilder GetNotificationString(NotificationOutcome outcome, StringBuilder notificationIds, int notificationId)
        {
            if (!((outcome.State == NotificationOutcomeState.Abandoned) ||
                            (outcome.State == NotificationOutcomeState.Unknown)))
            {
                if (notificationIds.Length == 0)
                    notificationIds.Append(notificationId);
                else
                    notificationIds.Append(Messages.comma + notificationId);
            }
            return notificationIds;
        }

        /// <summary>
        /// Update notitfication flag if it sent 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="notificationIds"></param>
        private void InvokeUpdateNotificationFlagIsSent(string notificationIds)
        {
            try
            {
                if (notificationIds == "")
                    return;

                var notificationModel = new DTO.NotificationModel() { ids = notificationIds };
                _connectionContext.TriggerContext.NotificationRepository.UpdateNotificationFlagIsSent(notificationModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }

        /// <summary>
        /// Method to get employee details by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private int GetUserLoginInfo(int userId)
        {
            UserLogin userLogin = new UserLogin() { existingEmpId = userId };
            userLogin = _connectionContext.TriggerContext.UserLoginRepository.GetUserDetails(userLogin);
            return userLogin.userId;
        }

        /// <summary>
        /// Get device info by logged in user id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userId"></param>
        /// <returns>list of users</returns>
        private List<Trigger.DTO.UserLoginModel> GetDeviceInfoById(int userId)
        {
            EmployeeModel employeeModel = new EmployeeModel() { UserId = userId };
            List<Trigger.DTO.UserLoginModel> lstUsers = _connectionContext.TriggerContext.EmployeeRepository.GetDeviceInfoById(employeeModel);
            return lstUsers;
        }
    }
}
