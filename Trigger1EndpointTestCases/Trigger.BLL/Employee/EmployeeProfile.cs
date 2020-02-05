using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.Shared.Enum;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Employee
{
    /// <summary>
    /// Class to manage employee profile
    /// </summary>
    public class EmployeeProfile
    {
        private readonly string _blobContainer = Messages.profilePic;
        private readonly EmployeeCommon _employeeCommon;
        private readonly ILogger<EmployeeProfile> _logger;
        private readonly IConnectionContext _connectionContext;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Constructor of EmployeeProfile class
        /// </summary>
        /// <param name="employeeCommon"></param>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        /// <param name="catalogDbContext"></param>
        /// <param name="appSettings"></param>
        public EmployeeProfile(EmployeeCommon employeeCommon, IConnectionContext connectionContext,
            ILogger<EmployeeProfile> logger, TriggerCatalogContext catalogDbContext,
            IOptions<AppSettings> appSettings)
        {
            _employeeCommon = employeeCommon;
            _connectionContext = connectionContext;
            _logger = logger;
            _catalogDbContext = catalogDbContext;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Method Name  :   GetEmployeeByIdForEditProfileAsync
        /// Method to get employee profile details by empid 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetEmployeeByIdForEditProfileAsync(int employeeId)
        {
            try
            {
                var employeeModel = new EmployeeProfileModel { EmpId = employeeId };
                var employee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeProfileRepository.Select(employeeModel));
                if (employee != null)
                {
                    employee.EmpImgPath = (employee.EmpImgPath == null || employee.EmpImgPath == string.Empty) ? string.Empty : (_appSettings.StorageAccountURL + Messages.slash + _blobContainer + Messages.slash + employee.EmpImgPath);
                }
                return JsonSettings.UserCustomDataWithStatusMessage(employee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to update employee profile details 
        /// user can update details itself
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> UpdateProfileAsync(int userId, DTO.EmployeeProfileModel employee)
        {
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();
                EmployeeModel existingEmployee = _employeeCommon.GetExistingEmployeeDetails(employee.EmpId);
                if (existingEmployee == null)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noDataFound);
                }
                bool isPhoneNumberExist = _employeeCommon.IsPhoneNumberExistAspNetUser(new EmployeeModel { PhoneNumber = employee.PhoneNumber, Email = existingEmployee.Email });
                if (isPhoneNumberExist)
                {
                    return _employeeCommon.GetResponseMessageForUpdate(EmployeeEnums.UpdateResultType.PhoneNumberExist.GetHashCode());
                }
                employee.UpdatedBy = userId;
                var result = await Task.FromResult(_connectionContext.TriggerContext.EmployeeProfileRepository.Update(employee));
                return await GetUpdateProfileResponse(result.Result, existingEmployee, employee);
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to update allow sms reciveing
        /// user can update details itself
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> UpdateAllowSmsAsync(int userId, EmployeeProfileModel employee)
        {
            try
            {
                employee.UpdatedBy = userId;
                var emp = await Task.FromResult(_connectionContext.TriggerContext.EmployeeProfileRepository.UpdateAllowSMS(employee));

                switch ((EmployeeEnums.SmsServiceResultType)emp.Result)
                {
                    case EmployeeEnums.SmsServiceResultType.ActiveSMSNotification:
                        {
                            EmployeeSmsNotification empSMSNotification = new EmployeeSmsNotification { EmpId = employee.EmpId, OptForSms = employee.OptForSms };
                            return JsonSettings.UserCustomDataWithStatusMessage(empSMSNotification, Convert.ToInt32(Enums.StatusCodes.status_200), string.Empty);
                        }
                    case EmployeeEnums.SmsServiceResultType.EmpIsInActive:
                        {
                            return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.empIsInactive);
                        }
                    case EmployeeEnums.SmsServiceResultType.PhoneNumberNotVerify:
                        {
                            return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.phoneNumberIsNotVerified);
                        }
                    case EmployeeEnums.SmsServiceResultType.EmployeeNotExist:
                        {
                            return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.employeeNotExist);
                        }
                    case EmployeeEnums.SmsServiceResultType.AlreadyDone:
                        {
                            EmployeeSmsNotification empSMSNotification = new EmployeeSmsNotification { EmpId = employee.EmpId, OptForSms = employee.OptForSms };
                            return JsonSettings.UserCustomDataWithStatusMessage(empSMSNotification, Convert.ToInt32(Enums.StatusCodes.status_304), (employee.OptForSms ? Messages.smsserviceallowed : Messages.smsservicenotallowed));
                        }
                    default:
                        {
                            return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToUpdateEmployee);
                        }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to update employee profile pic 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="empProfile"></param>
        /// <returns>URL where profile pic is uploaded</returns>
        public virtual async Task<JsonData> UpdateProfilePicAsync(int userId, EmployeeProfilePicture empProfile)
        {
            try
            {
                empProfile.UpdatedBy = userId;
                EmployeeImage empProfilePic = GetEmployeeProfilePic(empProfile);
                empProfile.EmpImgPath = empProfilePic.EmpImgPath;
                
                int result = await Task.FromResult(_connectionContext.TriggerContext.EmpProfileRepository.Update(empProfile).Result);
                if (result == 1)
                {
                    //Delete Existing Pic Pending
                    UploadProfilePic(empProfilePic);

                    empProfile.EmpImage = string.Empty;
                    empProfile.EmpImgPath = Convert.ToString(empProfile.EmpImgPath == string.Empty ? string.Empty : _appSettings.StorageAccountURL + Messages.slash + _blobContainer + Messages.slash + empProfile.EmpImgPath);

                    return JsonSettings.UserDataWithStatusMessage(empProfile, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.profilePhotoUpdated);
                }
                else if (result == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeNotExist);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToUpdateEmployee);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   UpdateProfile
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   04 June 2019
        /// Purpose      :   Update profile details for employee
        /// </summary>
        /// <param name="existingEmployee"></param>
        /// <param name="employeeProfile"></param>
        /// <returns></returns>
        private async Task<CustomJsonData> UpdateProfile(EmployeeModel existingEmployee, EmployeeProfileModel employeeProfile)
        {
            Boolean isPhoneNumberChange = employeeProfile.PhoneNumber != existingEmployee.PhoneNumber;

            existingEmployee.WorkCity = employeeProfile.WorkCity;
            existingEmployee.WorkState = employeeProfile.WorkState;
            existingEmployee.WorkZipCode = employeeProfile.WorkZipcode;
            existingEmployee.PhoneNumber = employeeProfile.PhoneNumber;

            UpdateAuthUser(existingEmployee, isPhoneNumberChange);

            _connectionContext.TriggerContext.Commit();
            EmployeeProfileModel updatedEmpProfile = await Task.FromResult(_connectionContext.TriggerContext.EmployeeProfileRepository.Select(employeeProfile));
            employeeProfile.PhoneConfirmed = updatedEmpProfile.PhoneConfirmed;
            employeeProfile.OptForSms = updatedEmpProfile.OptForSms;

            return JsonSettings.UserCustomDataWithStatusMessage(employeeProfile, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.empProfileoUpdated);
        }

        /// <summary>
        /// Method to get response of Update employee profile
        /// </summary>
        /// <param name="result"></param>
        /// <param name="employee"></param>
        /// <param name="employeeProfile"></param>
        /// <returns></returns>
        private async Task<CustomJsonData> GetUpdateProfileResponse(int result, EmployeeModel employee, EmployeeProfileModel employeeProfile)
        {
            switch ((EmployeeEnums.ProfileUpdateResultType)result)
            {
                case EmployeeEnums.ProfileUpdateResultType.ProfileUpdated:
                    {
                        return await UpdateProfile(employee, employeeProfile);
                    }
                case EmployeeEnums.ProfileUpdateResultType.EmpIsInActive:
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.empIsInactive);
                    }
                case EmployeeEnums.ProfileUpdateResultType.PhoneNumberIsExist:
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.phoneNumberIsExists);
                    }
                case EmployeeEnums.ProfileUpdateResultType.EmployeeNotExist:
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.employeeNotExist);
                    }
                default:
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_401.GetHashCode(), Messages.unauthorizedToUpdateEmployee);
                    }
            }
        }

        /// <summary>
        /// Method to Update auth user details 
        /// Update company admin details
        /// </summary>
        /// <param name="employeeProfile"></param>
        /// <param name="isPhoneNumberChange"></param>
        /// <returns></returns>
        private void UpdateAuthUser(EmployeeModel employeeProfile, bool isPhoneNumberChange)
        {
            AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = employeeProfile.Email };
            authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
            if (authUserDetails != null)
            {
                if (isPhoneNumberChange)
                {
                    _employeeCommon.UpdateAuthUser(employeeProfile, authUserDetails.Id);
                }
                if (employeeProfile.RoleId == EmployeeModel.Emprole.Admin.GetHashCode())
                {
                    _employeeCommon.InsertCompanyAdmin(employeeProfile);
                }
            }
        }

        /// <summary>
        /// Method to get profile pic of logged in user 
        /// </summary>
        /// <param name="empProfile"></param>
        /// <returns></returns>
        private EmployeeImage GetEmployeeProfilePic(EmployeeProfilePicture empProfile)
        {
            EmployeeImage empProfilePic = new EmployeeImage
            {
                EmpImage = empProfile.EmpImage,
                EmpImgPath = (empProfile.EmpImgPath == string.Empty ? string.Empty : Guid.NewGuid().ToString() + empProfile.EmpImgPath),
                EmpFolderPath = empProfile.EmpFolderPath
            };
            return empProfilePic;
        }

        /// <summary>
        /// Upload employee profile pic on Azure blob storage
        /// </summary>
        /// <param name="empProfilePic"></param>
        private void UploadProfilePic(EmployeeImage empProfilePic)
        {
            try
            {
                if (empProfilePic.EmpImage != null && empProfilePic.EmpImage != "" && empProfilePic.EmpImgPath != "" && empProfilePic.EmpImgPath != null)
                {
                    FileActions.UploadtoBlobAsync(empProfilePic.EmpImgPath, empProfilePic.EmpImage, _appSettings.StorageAccountName, _appSettings.StorageAccountAccessKey, _blobContainer).Wait();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }
    }
}
