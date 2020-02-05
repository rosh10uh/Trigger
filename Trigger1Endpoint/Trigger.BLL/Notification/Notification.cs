using Microsoft.Extensions.Logging;
using System;
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

        /// <summary>
        /// Use to initialize notification 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        public Notification(IConnectionContext connectionContext, ILogger<Notification> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        /// <summary>
        /// Use to get all notification detail
        /// </summary>
        /// <param name="managerId">Manager id</param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllNotification(int managerId)
        {
            NotificationModel notificationModel = new NotificationModel() { managerId = managerId };
            var notification = await Task.FromResult(_connectionContext.TriggerContext.NotificationRepository.GetAllNotification(notificationModel));

            if (notification?.Count > 0)
                return JsonSettings.UserCustomDataWithStatusMessage(notification, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            else
                return JsonSettings.UserCustomDataWithStatusMessage(notification, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.noNotifications);
        }

        /// <summary>
        /// Use to update notification mark as read
        /// </summary>
        /// <param name="ids">Comma separated ids</param>
        /// <returns></returns>
        public async Task<JsonData> UpdateNotificationMarkAsReadAsync(string ids)
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
    }
}
