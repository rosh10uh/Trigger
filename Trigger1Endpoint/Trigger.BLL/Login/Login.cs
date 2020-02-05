using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Login
{
    public class Login
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<Login> _logger;
        private readonly Employee.Employee _employee;
        private readonly IActionPermission _actionPermission;

        private readonly string _storageAccountURL;
        private readonly string _blobContainerEmployee = Messages.profilePic;
        private readonly string _blobContainerCompany = Messages.companyLogo;

        public Login(IConnectionContext connectionContext, ILogger<Login> logger, IActionPermission actionPermission, Employee.Employee employee)
        {
            _connectionContext = connectionContext;
            _logger = logger;
            _employee = employee;
            _actionPermission = actionPermission;
            _storageAccountURL = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.StorageAccountURL.ToString()];
        }

        #region Login
        public JsonData InvokeLogin(UserLoginModel userLoginModel, string apiVersion)
        {
            try
            {
                UserDataModel userDataModel = new UserDataModel() { userName = userLoginModel.username };
                var userData = _connectionContext.TriggerContext.LoginRepository.InvokeLogin(userDataModel);
                userData.Message = userData.Error;
                return RegisterDeviceInfo(userData, userLoginModel, apiVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        private JsonData RegisterDeviceInfo(UserDataModel userDataModel, UserLoginModel userLoginModel, string apiVersion)
        {
            if (userDataModel.Message == "" || userDataModel.Message == null)
            {
                int result = 0;
                if (userDataModel.userId > 0 && userLoginModel.deviceID != null && userLoginModel.deviceType != null)
                {
                    if (userDataModel.roleId == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                    {
                        return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.mobileAppsForManagerAndExecutiveLogin);
                    }
                    userLoginModel.userId = userDataModel.userId;
                    result = RegisterUserDeviceInfo(userLoginModel);
                }

                userDataModel = SetUserData(userDataModel, userDataModel.empId, apiVersion);
                SendNotifications(userDataModel.employee, result);

                return JsonSettings.UserDataWithStatusMessage(userDataModel, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.userLogIn);
            }
            else
            {
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_203), userDataModel.Message);
            }
        }

        /// <summary>
        /// Register logged in user's device id after successful logged in from mobile APP
        /// </summary>
        /// <param name="userLoginModel"></param>
        /// <returns></returns>
        private int RegisterUserDeviceInfo(UserLoginModel userLoginModel)
        {
            UserLoginModel userData = _connectionContext.TriggerContext.UserLoginModelRepository.RegisterUserDeviceInfo(userLoginModel);
            return userData.result;
        }

        private void SendNotifications(EmployeeModel employee, int result)
        {
            if (result > 0)
            {
                Task.Run(async () => await _employee.InvokeNotificationAsync(employee));
            }
        }

        private UserDataModel SetUserData(UserDataModel userData, int empId, string apiVersion)
        {
            string employeeBlob = _storageAccountURL + Messages.slash + _blobContainerEmployee + Messages.slash;
            string companyBlob = _storageAccountURL + Messages.slash + _blobContainerCompany + Messages.slash;

            EmployeeModel employeeModel = new EmployeeModel() { empId = empId };
            EmployeeModel employee = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employeeModel);

            userData.empEmailId = employee.email;
            userData.employee = employee;
            userData.employee.empImgPath = string.IsNullOrEmpty(userData.employee.empImgPath) ? string.Empty : (employeeBlob + userData.employee.empImgPath);
            userData.employee.companyLogoPath = string.IsNullOrEmpty(userData.employee.companyLogoPath) ? string.Empty : (companyBlob + userData.employee.companyLogoPath);
            userData.dbConnection = "";


            if (userData.employee.roleId != Enums.DimensionElements.TriggerAdmin.GetHashCode() && userData.employee.roleId != Enums.DimensionElements.CompanyAdmin.GetHashCode())
            {
                userData.permission = _actionPermission.GetPermissionAsPerApiVersion(apiVersion, empId);
            }

            return userData;
        }

        #endregion

        #region Logout
        /// <summary>
        /// Delete logged in device info when user is logout from mobile App
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public JsonData InvokeDeleteDeviceInfo(string deviceId)
        {
            try
            {
                UserLoginModel userLoginModel = new UserLoginModel() { deviceID = deviceId };
                userLoginModel = _connectionContext.TriggerContext.UserLoginModelRepository.invokeDeleteDeviceInfo(userLoginModel);

                if (userLoginModel.result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.deleteDeviceInfo);
                }
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
        #endregion
    }
}
