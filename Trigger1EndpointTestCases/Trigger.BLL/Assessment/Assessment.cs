using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.Assessment;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.BLL.Assessment
{
    public class Assessment
    {
        private readonly IConnectionContext _connectionContext;

        /// <summary>
		/// Use to get service of DAL.AssessmentContext
		/// </summary>        
		private readonly AssessmentContext _assessmentContext;
        private readonly ILogger<Assessment> _logger;
        private readonly IActionPermission _permission;
        private readonly IClaims _iClaims;

        /// <summary>
        /// To initialize assessment 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="assessmentContext"></param>
        /// <param name="logger"></param>
        public Assessment(IConnectionContext connectionContext, AssessmentContext assessmentContext, ILogger<Assessment> logger, IClaims claims, IActionPermission permission)
        {
            _connectionContext = connectionContext;
            _assessmentContext = assessmentContext;
            _logger = logger;
            _iClaims = claims;
            _permission = permission;
        }

        /// <summary>
        /// Use to get assessment score from company id and employee id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> GetAssessmentScore(int CompanyId, int EmpId)
        {
            try
            {
                AssessmentScoreModel assessmentScoreModel = new AssessmentScoreModel() { companyId = CompanyId, empId = EmpId };
                var result = await Task.FromResult(_connectionContext.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(assessmentScoreModel));
                if (result == null)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.notTriggerEmployee);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(result, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }

        }

        /// <summary>
        /// Use to get assessment score from company id, employee id and inserted pk
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="EmpId"></param>
        /// <param name="inserted"></param>
        /// <returns></returns>
        private async Task<JsonData> GetAssessmentScore(int CompanyId, int EmpId, int inserted)
        {
            try
            {
                AssessmentScoreModel assessmentScoreModel = new AssessmentScoreModel() { companyId = CompanyId, empId = EmpId };
                var result = await Task.FromResult(_connectionContext.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(assessmentScoreModel));
                if (inserted > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(result, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else if (inserted == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(result, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.allReadTriggerEmployee);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
        
        /// <summary>
        /// Method Name  :   AddAssessmentAsyncV1
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   10 September 2019
        /// Purpose      :   Version 1 : Add assessment detail asynchronously   with version V1
        /// </summary>
        /// <param name="empAssessmentModel"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> AddAssessmentAsyncV1(EmpAssessmentModel empAssessmentModel)
        {
            try
            {
                if (Convert.ToInt32(_iClaims["EmpId"].Value) == empAssessmentModel.empId)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.unauthorizedSelfAssessment);
                }

<<<<<<< HEAD
                var result = _assessmentContext.AddAssessmentMainV1(empAssessmentModel);
                EmployeeModel employeeModel = new EmployeeModel() { empId = empAssessmentModel.empId };
=======
                var result = InvokeInsertV1(empAssessmentModel);
                EmployeeModel employeeModel = new EmployeeModel() { EmpId = empAssessmentModel.empId };
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
                var employeeResult = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employeeModel);
                return await GetAssessmentScore(employeeResult.CompanyId, empAssessmentModel.empId, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   AddAssessmentAsyncV2
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   10 September 2019
        /// Purpose      :   Version 2 : Add assessment detail asynchronously   with version V2
        /// </summary>
        /// <param name="empAssessmentModel"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> AddAssessmentAsyncV2(EmpAssessmentModel empAssessmentModel)
        {
            try
            {
                bool canAdd = false;
                if (Convert.ToInt32(_iClaims["RoleId"].Value) != Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    var permissions = _permission.GetPermissions(Convert.ToInt32(_iClaims["EmpId"].Value)).Where(x => x.ActionId == Enums.Actions.TriggerEmployee.GetHashCode()).ToList();
                    if (permissions.Count > 0)
                    {
                        canAdd = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TriggerEmployee.GetHashCode()).ActionPermissions.Any(x => x.CanAdd);

                    }
                }

                if (Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || canAdd)
                {
                    if (Convert.ToInt32(_iClaims["EmpId"].Value) == empAssessmentModel.empId)
                    {
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.unauthorizedSelfAssessment);
                    }

<<<<<<< HEAD
                    var result = await _assessmentContext.AddAssessmentMainV2(empAssessmentModel);
                    EmployeeModel employeeModel = new EmployeeModel() { empId = empAssessmentModel.empId };
                    var employeeResult = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employeeModel);
                    return await GetAssessmentScore(employeeResult.companyid, empAssessmentModel.empId, result);
=======
                    var result = InvokeInsertV2Async(empAssessmentModel);
                    EmployeeModel employeeModel = new EmployeeModel() { EmpId = empAssessmentModel.empId };
                    var employeeResult = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employeeModel);
                    return await GetAssessmentScore(employeeResult.CompanyId, empAssessmentModel.empId, result.Result);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   AddAssessmentAsyncV2_1
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   10 September 2019
        /// Purpose      :   Version 2.1 : Add assessment detail asynchronously   with version V2.1
        /// </summary>
        /// <param name="empAssessmentModel"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> AddAssessmentAsyncV2_1(EmpAssessmentModel empAssessmentModel)
        {
            try
            {
                if (_permission.CheckActionPermission(_permission.GetPermissionParameters(Enums.Actions.TriggerEmployee, Enums.PermissionType.CanAdd)))
                {
                    if (Convert.ToInt32(_iClaims["EmpId"].Value) == empAssessmentModel.empId)
                    {
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.unauthorizedSelfAssessment);
                    }

<<<<<<< HEAD
                    var result = _assessmentContext.AddAssessmentMainV2(empAssessmentModel);
                    EmployeeModel employeeModel = new EmployeeModel() { empId = empAssessmentModel.empId };
=======
                    var result = InvokeInsertV2Async(empAssessmentModel);
                    EmployeeModel employeeModel = new EmployeeModel() { EmpId = empAssessmentModel.empId };
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
                    var employeeResult = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employeeModel);
                    return await GetAssessmentScore(employeeResult.CompanyId, empAssessmentModel.empId, result.Result);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   AddAssessmentAsyncV2_2
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   10 September 2019
        /// Purpose      :   Version 2.2 : Add assessment detail asynchronously   with version V2.2
        /// </summary>
        /// <param name="empAssessmentModel"></param>
        /// <returns></returns>
        public async Task<JsonData> AddAssessmentAsyncV2_2(EmpAssessmentModel empAssessmentModel)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.TriggerEmployee, Enums.PermissionType.CanAdd)))
                {
                    if (Convert.ToInt32(_iClaims["EmpId"].Value) == empAssessmentModel.empId)
                    {
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.unauthorizedSelfAssessment);
                    }

                    var result = await _assessmentContext.AddAssessmentMainV2(empAssessmentModel);
                    EmployeeModel employeeModel = new EmployeeModel() { empId = empAssessmentModel.empId };
                    var employeeResult = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employeeModel);
                    return await GetAssessmentScore(employeeResult.companyid, empAssessmentModel.empId, result);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Use to delete assessment attachment doc from azure blob
        /// </summary>
        /// <param name="documentDetails"></param>
        /// <returns></returns>
        public async Task<JsonData> DeleteAssessmentAttachment(EmpAssessmentDet empAssessmentDet)
        {
            try
            {

                var result = await Task.FromResult(_assessmentContext.DeleteAssessmentDocument(empAssessmentDet));

                switch (result.Result.status)
                {
                    case 0:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                    case 1:
                        return JsonSettings.UserDataWithStatusMessage(result.Result, Enums.StatusCodes.status_200.GetHashCode(), Messages.AttachmentDeleted);
                    case -1:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                    default:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Author : Vivek Bhavsar
        /// Created Date : 19-07-2018
        /// Purpose : Update general & category wise comments
        /// </summary>
        /// <param name="empAssessmentDet"></param>
        /// <returns></returns>
        public async Task<JsonData> UpdateAssessmentComment(EmpAssessmentDet empAssessmentDet)
        {
            try
            {

                var result = await Task.FromResult(_assessmentContext.UpdateAssessmentComment(empAssessmentDet));

                switch (result.Result.status)
                {
                    case 0:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                    case 1:
                        return JsonSettings.UserDataWithStatusMessage(result.Result, Enums.StatusCodes.status_200.GetHashCode(), Messages.AssessmentCommentUpdated);
                    case -1:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                    default:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to delete assessment comment by assessmentid & remarkid with attachment
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="remarkid"></param>
        /// <returns></returns>
        public async Task<JsonData> DeleteAssessmentComment(int assessmentId, int remarkId, int userId)
        {
            try
            {
                var result = await Task.FromResult(_assessmentContext.DeleteAssessmentComment(new EmpAssessmentDet { assessmentId = assessmentId, remarkId = remarkId, updatedBy = userId }));

                switch (result.Result)
                {
                    case 0:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                    case 1:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.AssessmentCommentDelete);
                    case -1:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                    default:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        ///Version 2.0
        /// <summary>
        /// Use to delete assessment attachment doc from azure blob
        /// </summary>
        /// <param name="documentDetails"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> DeleteAssessmentAttachmentV2(EmpAssessmentDet empAssessmentDet)
        {
            try
            {
                if (_permission.CheckActionPermission(_permission.GetPermissionParameters(Enums.Actions.Comment, Enums.PermissionType.CanDelete)))
                {
                    var result = await Task.FromResult(_assessmentContext.DeleteAssessmentDocument(empAssessmentDet));

                    switch (result.Result.status)
                    {
                        case 0:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                        case 1:
                            return JsonSettings.UserDataWithStatusMessage(result.Result, Enums.StatusCodes.status_200.GetHashCode(), Messages.AttachmentDeleted);
                        case -1:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                        default:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        //Version 2.1
        /// <summary>
        /// Method Name  :   DeleteAssessmentAttachmentV2_1
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   17 September 2019
        /// Purpose      :   Version 2.1 : Use to delete assessment attachment doc from azure blob
        /// </summary>
        /// <param name="empAssessmentDet"></param>
        /// <returns></returns>
        public async Task<JsonData> DeleteAssessmentAttachmentV2_1(EmpAssessmentDet empAssessmentDet)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.Comment, Enums.PermissionType.CanDelete)))
                {
                    var result = await Task.FromResult(_assessmentContext.DeleteAssessmentDocument(empAssessmentDet));

                    switch (result.Result.status)
                    {
                        case 0:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                        case 1:
                            return JsonSettings.UserDataWithStatusMessage(result.Result, Enums.StatusCodes.status_200.GetHashCode(), Messages.AttachmentDeleted);
                        case -1:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                        default:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Author : Vivek Bhavsar
        /// Created Date : 19-07-2018
        /// Purpose : Update general & category wise comments
        /// </summary>
        /// <param name="empAssessmentDet"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> UpdateAssessmentCommentV2(EmpAssessmentDet empAssessmentDet)
        {
            try
            {
                if (_permission.CheckActionPermission(_permission.GetPermissionParameters(Enums.Actions.Comment, Enums.PermissionType.CanEdit)))
                {
                    var result = await Task.FromResult(_assessmentContext.UpdateAssessmentComment(empAssessmentDet));

                    switch (result.Result.status)
                    {
                        case 0:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                        case 1:
                            return JsonSettings.UserDataWithStatusMessage(result.Result, Enums.StatusCodes.status_200.GetHashCode(), Messages.AssessmentCommentUpdated);
                        case -1:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                        default:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   UpdateAssessmentCommentV2_1
        /// Author : Bhumika Bhavsar
        /// Created Date : 17 September 2019
        /// Purpose : Version 2.1 : Update general & category wise comments
        /// </summary>
        /// <param name="empAssessmentDet"></param>
        /// <returns></returns>
        public async Task<JsonData> UpdateAssessmentCommentV2_1(EmpAssessmentDet empAssessmentDet)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.Comment, Enums.PermissionType.CanEdit)))
                {
                    var result = await Task.FromResult(_assessmentContext.UpdateAssessmentComment(empAssessmentDet));

                    switch (result.Result.status)
                    {
                        case 0:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                        case 1:
                            return JsonSettings.UserDataWithStatusMessage(result.Result, Enums.StatusCodes.status_200.GetHashCode(), Messages.AssessmentCommentUpdated);
                        case -1:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                        default:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to delete assessment comment by assessmentid & remarkid with attachment
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="remarkid"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> DeleteAssessmentCommentV2(int assessmentId, int remarkId, int userId)
        {
            try
            {
                if (_permission.CheckActionPermission(_permission.GetPermissionParameters(Enums.Actions.Comment, Enums.PermissionType.CanDelete)))
                {
                    var result = await Task.FromResult(_assessmentContext.DeleteAssessmentComment(new EmpAssessmentDet { assessmentId = assessmentId, remarkId = remarkId, updatedBy = userId }));

                    switch (result.Result)
                    {
                        case 0:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                        case 1:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.AssessmentCommentDelete);
                        case -1:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                        default:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   DeleteAssessmentCommentV2_1
        /// Author : Bhumika Bhavsar
        /// Created Date : 17 September 2019
        /// Purpose : Version 2.1 : Method to delete assessment comment by assessmentid & remarkid with attachment
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="remarkid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<JsonData> DeleteAssessmentCommentV2_1(int assessmentId, int remarkId, int userId)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.Comment, Enums.PermissionType.CanDelete)))
                {
                    var result = await Task.FromResult(_assessmentContext.DeleteAssessmentComment(new EmpAssessmentDet { assessmentId = assessmentId, remarkId = remarkId, updatedBy = userId }));

                    switch (result.Result)
                    {
                        case 0:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.InvalidDocumentDetails);
                        case 1:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.AssessmentCommentDelete);
                        case -1:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedForAssessment);
                        default:
                            return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                    }
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Author : Mayur Patel
        /// Created Date : 20-09-2019
        /// Purpose : Update assessment score feedback
        /// </summary>
        /// <param name="assessmentScoreModel"></param>
        /// <returns></returns>
        public async Task<JsonData> UpdateAssessmentScoreFeddback(AssessmentScoreModel assessmentScoreModel)
        {
            try
            {
                var result = await Task.FromResult(_connectionContext.TriggerContext.AssessmentScoreRepository.UpdateAssessmentScoreFeedback(assessmentScoreModel));

                if (result.Result == 1)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.AssessmentScoreFeedbackUpdated);
                }
                else if (result.Result == -1)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.AssessmentDetailsNotFound);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }
    }
}
