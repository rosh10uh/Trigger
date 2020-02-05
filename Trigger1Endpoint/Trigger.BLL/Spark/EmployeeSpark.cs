using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.Spark;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Utility;
using static Trigger.BLL.Shared.Enums;

namespace Trigger.BLL.Spark
{
    /// <summary>
    /// Class Name   :   EmployeeSpark
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   16 Aug 2019
    /// Purpose      :   Buisness logic for spark operations
    /// Revision     :  
    /// </summary>
    public class EmployeeSpark
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<EmployeeSpark> _logger;
        private readonly EmployeeSparkContext _employeeSparkContext;
        private readonly AppSettings _appSettings;
        private readonly IActionPermission _permission;
        private readonly IClaims _iClaims;
        private readonly ISmsSender _smsSender;

        /// <summary>
        /// Constructor for employee spark
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        public EmployeeSpark(IConnectionContext connectionContext, EmployeeSparkContext employeeSparkContext,
            IOptions<AppSettings> appSettings, IActionPermission permission, ISmsSender smsSender, ILogger<EmployeeSpark> logger, IClaims Claims)
        {
            _connectionContext = connectionContext;
            _employeeSparkContext = employeeSparkContext;
            _appSettings = appSettings.Value;
            _permission = permission;
            _smsSender = smsSender;
            _logger = logger;
            _iClaims = Claims;
        }

        /// <summary>
        /// Method to get list of sparks for selected employee
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAsync(int empId)
        {
            try
            {
                List<EmployeeSparkModel> employeeSparkModel = await Task.FromResult(_connectionContext.TriggerContext.EmployeeSparkRepository.GetEmployeeSparkDetails(new EmployeeSparkModel { EmpId = empId }));
                employeeSparkModel?.ForEach(e => e.DocumentName = (e.DocumentName == null || e.DocumentName == string.Empty) ? string.Empty : string.Concat(_appSettings.StorageAccountURL, Messages.slash, Messages.AssessmentDocPath, Messages.slash, e.DocumentName));
                employeeSparkModel?.ForEach(e => e.SparkByImgPath = (e.SparkByImgPath == null || e.SparkByImgPath == string.Empty) ? string.Empty : string.Concat(_appSettings.StorageAccountURL, Messages.slash, Messages.profilePic, Messages.slash, e.SparkByImgPath));

                if (employeeSparkModel?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(employeeSparkModel, StatusCodes.status_200.GetHashCode(), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(new List<EmployeeSparkModel>(), Enums.StatusCodes.status_100.GetHashCode(), Messages.noRecords);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to get unapproved spark list by logger user
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetUnApprovedSparkAsync()
        {
            try
            {
                int empId = Convert.ToInt32(_iClaims["EmpId"].Value);

                List<EmployeeSparkModel> employeeSparkModel = await Task.FromResult(_connectionContext.TriggerContext.EmployeeSparkRepository.GetUnApprovedSparkDetails(new EmployeeSparkModel { EmpId = empId }));
                employeeSparkModel?.ForEach(e => e.DocumentName = (e.DocumentName == null || e.DocumentName == string.Empty) ? string.Empty : string.Concat(_appSettings.StorageAccountURL, Messages.slash, Messages.AssessmentDocPath, Messages.slash, e.DocumentName));
                employeeSparkModel?.ForEach(e => e.SparkByImgPath = (e.SparkByImgPath == null || e.SparkByImgPath == string.Empty) ? string.Empty : string.Concat(_appSettings.StorageAccountURL, Messages.slash, Messages.profilePic, Messages.slash, e.SparkByImgPath));

                if (employeeSparkModel?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(employeeSparkModel, StatusCodes.status_200.GetHashCode(), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(new List<EmployeeSparkModel>(), StatusCodes.status_100.GetHashCode(), Messages.noRecords);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to add spark for an employee
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> PostAsync(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                if (_permission.CheckActionPermission(_permission.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanAdd)))
                {
                    employeeSparkModel.ApprovalStatus = SparkStatus.Approved.GetHashCode();
                    employeeSparkModel.ApprovalBy = employeeSparkModel.CreatedBy;

                    var result = await _employeeSparkContext.AddEmployeeSpark(employeeSparkModel);

                    switch (result.Result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(result, StatusCodes.status_200.GetHashCode(), Messages.AddEmployeeSpark);
                        case -1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.UnauthorizedForSpark);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to approve or reject spark 
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> UpdateSparkApprovalStatus(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                int empId = Convert.ToInt32(_iClaims["EmpId"].Value);

                var employeeModel = new EmployeeModel { empId = employeeSparkModel.EmpId };
                var employee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.Select(employeeModel));
                if (employee.managerId == empId)
                {
                    employeeSparkModel.ApprovalBy = employeeSparkModel.CreatedBy;

                    var result = await Task.FromResult(_connectionContext.TriggerContext.EmployeeSparkRepository.UpdateSparkApprovalStatus(employeeSparkModel));

                    switch (result.Result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_200.GetHashCode(), Messages.SparkApproved);
                        case 2:
                            SendSparkRejectionSMS(employeeSparkModel);
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_200.GetHashCode(), Messages.SparkRejected);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.unauthorizedToApproveSpark);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to send sms to user who had sparked employee to intimate rejection of spark by Reporting Manager of Employee
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        private void SendSparkRejectionSMS(EmployeeSparkModel employeeSparkModel)
        {
            SparkRejectionDetails sparkRejectionDetails = _connectionContext.TriggerContext.EmployeeSparkRepository.GetSparkRejectionDetails(employeeSparkModel);
            if (!string.IsNullOrEmpty(sparkRejectionDetails.SenderPhoneNumber))
            {
                _smsSender.SendSmsAsync(sparkRejectionDetails.SenderPhoneNumber, string.Format(Messages.SparkRejectionMessage, sparkRejectionDetails.EmployeeName, sparkRejectionDetails.RejectedByName, sparkRejectionDetails.RejectionRemark));
            }
        }

        /// <summary>
        /// Method to update spark for an employee
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> PutAsync(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                if (_permission.CheckActionPermission(_permission.GetPermissionParameters(Enums.Actions.SparkEmployee, PermissionType.CanEdit)))
                {
                    int result;

                    var employeeSparkResponse = new EmployeeSparkModel();

                    if (employeeSparkModel.DocumentName.Length == 0 && employeeSparkModel.Remarks.Length == 0 && employeeSparkModel.CloudFilePath.Length == 0)
                    {
                        result = await _employeeSparkContext.DeleteAsync(new EmployeeSparkModel { EmpId = employeeSparkModel.EmpId, SparkId = employeeSparkModel.SparkId, SparkBy = employeeSparkModel.SparkBy, UpdatedBy = employeeSparkModel.UpdatedBy });
                    }
                    else
                    {
                        employeeSparkResponse = ( await _employeeSparkContext.UpdateEmployeeSpark(employeeSparkModel));
                        result = employeeSparkResponse.Result;
                    }
                    switch (result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(employeeSparkResponse, StatusCodes.status_200.GetHashCode(), Messages.UpdateEmployeeSpark);
                        case -1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.UnauthorizedForSpark);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to delete spark details of an employee
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="sparkId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> DeleteAsync(int empId, int sparkId, int userId)
        {
            try
            {
                if (_permission.CheckActionPermission(_permission.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanDelete)))
                {
                    var result = await _employeeSparkContext.DeleteAsync(new EmployeeSparkModel { EmpId = empId, SparkId = sparkId, SparkBy = userId, UpdatedBy = userId });

                    switch (result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_200.GetHashCode(), Messages.DeleteEmployeeSpark);
                        case -1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.UnauthorizedForSpark);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to delete only attachment of spark for an employee
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="sparkId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> DeleteAttachmentAsync(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                if (_permission.CheckActionPermission(_permission.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanDelete)))
                {
                    var result = await _employeeSparkContext.DeleteAttachmentAsync(employeeSparkModel);

                    switch (result.Result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(result, StatusCodes.status_200.GetHashCode(), Messages.AttachmentDeleted);
                        case -1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.UnauthorizedForSpark);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }


        /// <summary>
        /// Method to add spark for an employee version v2
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> AddEmployeeSparkV2(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanAdd)))
                {
                    employeeSparkModel.ApprovalStatus = SparkStatus.Approved.GetHashCode();
                    employeeSparkModel.ApprovalBy = employeeSparkModel.CreatedBy;

                    var result = await _employeeSparkContext.AddEmployeeSpark(employeeSparkModel);

                    switch (result.Result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(result, StatusCodes.status_200.GetHashCode(), Messages.AddEmployeeSpark);
                        case -1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.UnauthorizedForSpark);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to update spark for an employee
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> UpdateEmployeeSparkV2(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanEdit)))
                {
                    var result = await _employeeSparkContext.UpdateEmployeeSpark(employeeSparkModel);

                    switch (result.Result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(result, StatusCodes.status_200.GetHashCode(), Messages.UpdateEmployeeSpark);
                        case -1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.UnauthorizedForSpark);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to delete spark details of an employee
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="sparkId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> DeleteEmployeeSparkV2(int empId, int sparkId, int userId)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanDelete)))
                {
                    var result = await _employeeSparkContext.DeleteAsync(new EmployeeSparkModel { EmpId = empId, SparkId = sparkId, SparkBy = userId, UpdatedBy = userId });

                    switch (result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_200.GetHashCode(), Messages.DeleteEmployeeSpark);
                        case -1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.UnauthorizedForSpark);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to delete only attachment of spark for an employee 
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="sparkId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> DeleteSparkAttachmentV2(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanDelete)))
                {
                    var result = await _employeeSparkContext.DeleteAttachmentAsync(employeeSparkModel);

                    switch (result.Result)
                    {
                        case 0:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_208.GetHashCode(), Messages.InvalidSparkDetails);
                        case 1:
                            return JsonSettings.UserCustomDataWithStatusMessage(result, StatusCodes.status_200.GetHashCode(), Messages.AttachmentDeleted);
                        case -1:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_204.GetHashCode(), Messages.UnauthorizedForSpark);
                        default:
                            return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }
    }
}
