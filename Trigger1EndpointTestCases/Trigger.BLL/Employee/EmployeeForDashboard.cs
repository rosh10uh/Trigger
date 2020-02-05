using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.BLL.Employee
{
    /// <summary>
    /// Class to get employee list from dashboard
    /// </summary>
    public class EmployeeForDashboard
    {
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly ILogger<EmployeeForDashboard> _logger;
        private readonly IConnectionContext _connectionContext;
        private readonly EmployeeCommon _employeeCommon;

        /// <summary>
        /// Constructor of EmployeeForDashboard class
        /// </summary>
        /// <param name="catalogDbContext"></param>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        /// <param name="employeeCommon"></param>
        public EmployeeForDashboard(TriggerCatalogContext catalogDbContext, IConnectionContext connectionContext,
            ILogger<EmployeeForDashboard> logger, EmployeeCommon employeeCommon)
        {
            _catalogDbContext = catalogDbContext;
            _logger = logger;
            _connectionContext = connectionContext;
            _employeeCommon = employeeCommon;
        }

        /// <summary>
        /// Method Name  :   GetAllEmployeesWithPaginationYearWiseAsync
        /// get year wise employee list with pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetAllEmployeesWithPaginationYearWiseAsync(EmployeeModel employeeModel)
        {
            try
            {
                int userRoleId = _employeeCommon.GetRoleIdFromClaims();
                employeeModel.RoleId = userRoleId;
                List<EmployeeModel> allEmployee;

                if (employeeModel.ManagerId == 0 && employeeModel.CompanyId == 0 && userRoleId == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    allEmployee = await Task.FromResult(_catalogDbContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithPagination(employeeModel));
                }
                else if (employeeModel.ManagerId == 0 && employeeModel.CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.ManagerId = _employeeCommon.GetEmployeeIdFromClaims();
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerYearwise(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeYearwise(employeeModel));
                }
                return GetResponse(allEmployee, employeeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// get year wise grade wise employee list without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetEmployeeByGradeWithPaginationAsync(EmployeeModel employeeModel)
        {
            try
            {
                int userRoleId = _employeeCommon.GetRoleIdFromClaims();
                employeeModel.RoleId = userRoleId;
                List<EmployeeModel> allEmployee;
                if (employeeModel.ManagerId == 0 && employeeModel.CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.ManagerId = _employeeCommon.GetEmployeeIdFromClaims();
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwise(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(employeeModel));
                }
                return GetResponse(allEmployee, employeeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// get year wise, month wise, grade wise employee list without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetEmployeeByGradeAndMonthWithPaginationAsync(EmployeeModel employeeModel)
        {
            try
            {
                int userRoleId = _employeeCommon.GetRoleIdFromClaims();
                List<EmployeeModel> allEmployee;

                if (employeeModel.ManagerId == 0 && employeeModel.CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.ManagerId = _employeeCommon.GetEmployeeIdFromClaims();
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthByManagerYearwise(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthYearwise(employeeModel));
                }
                return GetResponse(allEmployee, employeeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetEmployeeByMonthAndGradeWithoutPaginationAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   27 May 2019
        /// Purpose      :   Get employee list without pagination when redirected from bar graph
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetEmployeeByGradeAndMonthWithoutPaginationAsync(EmployeeModel employeeModel)
        {
            try
            {
                int userRoleId = _employeeCommon.GetRoleIdFromClaims();
                List<EmployeeModel> allEmployee;

                if (employeeModel.ManagerId == 0 && employeeModel.CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.ManagerId = _employeeCommon.GetEmployeeIdFromClaims();
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmpByGradeAndMonthYearwiseWithoutPagination(employeeModel));
                }
                return GetResponse(allEmployee, employeeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetAllEmployeesWithoutPaginationYearWiseAsync
        /// get year wise employee list without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetAllEmployeesWithoutPaginationYearWiseAsync(EmployeeModel employeeModel)
        {
            try
            {
                int userRoleId = _employeeCommon.GetRoleIdFromClaims();
                List<EmployeeModel> allEmployee;

                if (employeeModel.ManagerId == 0 && employeeModel.CompanyId == 0 && userRoleId == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    allEmployee = await Task.FromResult(_catalogDbContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithoutPagination(employeeModel));

                }
                else if (employeeModel.ManagerId == 0 && employeeModel.CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.ManagerId = _employeeCommon.GetEmployeeIdFromClaims();
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerYearwiseWithoutPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeYearwiseWithoutPagination(employeeModel));
                }
                allEmployee = _employeeCommon.SetEmployeeProfilePic(allEmployee);
                return GetResponse(allEmployee, employeeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetEmployeeByGradeWithoutPaginationAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   24 May 2019
        /// Purpose      :   Get employee list without pagination when redirected from graph
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetEmployeeByGradeWithoutPaginationAsync(EmployeeModel employeeModel)
        {
            try
            {
                int userRoleId = _employeeCommon.GetRoleIdFromClaims();
                List<EmployeeModel> allEmployee;

                if (employeeModel.ManagerId == 0 && employeeModel.CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.ManagerId = _employeeCommon.GetEmployeeIdFromClaims();
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwiseWithoutPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(employeeModel));
                }
                return GetResponse(allEmployee, employeeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetDashboardEmpList
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   02 July 2019
        /// Purpose      :   Get employeeList for dropdown of Employee Dashboard Page
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetDashboardEmpList(int companyId, int managerId)
        {
            try
            {
                List<EmployeeListModel> allEmployee;
                if (_employeeCommon.GetRoleIdFromClaims() == Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    EmployeeListModel employeeModel = new EmployeeListModel { CompanyId = companyId, ManagerId = managerId };
                    allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                }
                else
                {
                    ActionList allPermission = await Task.FromResult(_employeeCommon.GetAllPermission(managerId == 0 ? _employeeCommon.GetEmployeeIdFromClaims() : managerId, Enums.Actions.EmployeeDashboard.GetHashCode()));
                    var permission = allPermission.ActionPermissions.Where(x => x.CanView).ToList();
                    if (permission != null && permission.Count > 0)
                    {
                        allEmployee = _employeeCommon.GetEmployeeListForActions(permission, companyId, managerId);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(new List<EmployeeListModel>(), Convert.ToInt32(Enums.StatusCodes.status_403), Messages.accessDenied);
                    }
                }
                return _employeeCommon.GetResponseWitCheckAccessDenied(allEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
        
        /// <summary>
        /// Method to get response of all methods
        /// </summary>
        /// <param name="allEmployee"></param>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        private CustomJsonData GetResponse(List<EmployeeModel> allEmployee, EmployeeModel employeeModel)
        {
            allEmployee = _employeeCommon.SetEmployeeProfilePic(allEmployee);
            _employeeCommon.GetRedirectEmpListWithHierachyForActions(employeeModel, allEmployee);
            return _employeeCommon.GetResponseWithCheckNoRecord(allEmployee);
        }
    }
}
