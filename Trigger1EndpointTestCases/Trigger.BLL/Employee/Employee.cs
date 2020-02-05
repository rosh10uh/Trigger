using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;
using static Trigger.DAL.Shared.Enum.EmployeeEnums;

namespace Trigger.BLL.Employee
{
    /// <summary>
    /// Class to Manage employee details
    /// </summary>
    public class Employee
    {
        private readonly ILogger<Employee> _logger;
        private readonly IConnectionContext _connectionContext;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly Notification.Notification _notification;
        private readonly EmployeeSendEmail _employeeSendEmail;
        private readonly EmployeeCommon _employeeCommon;
        private readonly IActionPermission _actionPermission;

        /// <summary>
        /// Constructor of employee class
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        /// <param name="catalogDbContext"></param>
        /// <param name="employeeSendEmail"></param>
        /// <param name="employeeCommon"></param>
        /// <param name="notification"></param>
        public Employee(IConnectionContext connectionContext, ILogger<Employee> logger,
            TriggerCatalogContext catalogDbContext, EmployeeSendEmail employeeSendEmail,
            EmployeeCommon employeeCommon, Notification.Notification notification,
             IActionPermission actionPermission)
        {
            _connectionContext = connectionContext;
            _logger = logger;
            _employeeSendEmail = employeeSendEmail;
            _catalogDbContext = catalogDbContext;
            _notification = notification;
            _employeeCommon = employeeCommon;
            _actionPermission = actionPermission;
        }

        /// <summary>
        /// Method to add employee
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> InsertAsync(int userId, EmployeeModel employee)
        {
            try
            {
                if (_employeeCommon.IsPhoneNumberExistAspNetUser(new EmployeeModel { PhoneNumber = employee.PhoneNumber }, true))
                {
                    return GetResponseMessageForInsert(InsertResultType.PhoneNumberExist.GetHashCode());
                }
                _connectionContext.TriggerContext.BeginTransaction();
                employee.CreatedBy = userId;
                int empId = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.Insert(employee).ResultId);
                if (empId > 0)
                {
                    employee.EmpId = empId;
                    AddAuthUserDetails(employee);
                    _connectionContext.TriggerContext.Commit();
                    _notification.SendNotifications(employee.EmpId, employee);
                }
                else
                {
                    _connectionContext.TriggerContext.Commit();
                }
                return GetResponseMessageForInsert(empId > 0 ? InsertResultType.EmployeeSubmit.GetHashCode() : empId);
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to update employee details 
        /// update users details if user's role is Admin/Manager/Executive and send mail And send notification to logged in Users if any employee added/removed under it
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> UpdateAsync(int userId, EmployeeModel employee)
        {
            try
            {
                EmployeeModel existingEmployee = _employeeCommon.GetExistingEmployeeDetails(employee.EmpId);
                if (_employeeCommon.IsPhoneNumberExistAspNetUser(new EmployeeModel { PhoneNumber = employee.PhoneNumber, Email = existingEmployee.Email }))
                {
                    return _employeeCommon.GetResponseMessageForUpdate(UpdateResultType.PhoneNumberExist.GetHashCode());
                }
                _connectionContext.TriggerContext.BeginTransaction();
                employee.UpdatedBy = userId;
                int resultId = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.Update(employee).Result);
                if (resultId == UpdateResultType.EmployeeUpdate.GetHashCode())
                {
                    UpdateAuthUserDetails(employee, existingEmployee);
                }
                _connectionContext.TriggerContext.Commit();
                SendNotificationOnEmployeeUpdate(existingEmployee, employee, resultId);
                return _employeeCommon.GetResponseMessageForUpdate(resultId);
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to Delete employee
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="EmpId"></param>
        /// <param name="UpdatedBy"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> DeleteAsync(int CompanyId, int EmpId, int UpdatedBy)
        {
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();
                EmployeeModel employee = new EmployeeModel { CompanyId = CompanyId, EmpId = EmpId, UpdatedBy = UpdatedBy, CreatedBy = EmpId };
                employee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employee));
                employee.CreatedBy = employee.EmpId;
                var isEmpDelete = _connectionContext.TriggerContext.EmployeeRepository.DeleteEmployee(employee);

                if (isEmpDelete.Result > 0)
                {
                    _connectionContext.TriggerContext.EmployeeRepository.DeleteUser(employee); //delete user login deletes
                    _connectionContext.TriggerContext.Commit();
                    DeleteUserDetailsFromCatalog(employee);
                    _notification.SendNotifications(employee.EmpId, employee);
                }
                else
                {
                    _connectionContext.TriggerContext.Commit();
                }
                return GetResponseMessageForDelete(isEmpDelete.Result);
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   UpdateEmpSalaryAsync
        /// Manager will update employee's Salary 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="empSalary"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> UpdateEmpSalaryAsync(int userId, EmployeeSalary empSalary)
        {
            try
            {
                empSalary.UpdatedBy = userId;
                int result = await Task.FromResult(_connectionContext.TriggerContext.EmpSalaryRepository.Update(empSalary).Result);
                switch ((UpdateSalaryResultType)result)
                {
                    case UpdateSalaryResultType.EmployeeNotExist:
                        return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeNotExist);
                    case UpdateSalaryResultType.SalaryUpdated:
                        return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.empSalaryUpdated);
                    case UpdateSalaryResultType.EmployeeInActive:
                        return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.empIsInactive);
                    default:
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
        /// Method Name  :   GetEmployeeByIdAsync
        /// Method to get employee details by empid
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
<<<<<<< HEAD
        public async Task<CustomJsonData> GetCompanyWiseEmployeeAsync(int companyId)
        {
            try
            {
                var employeeModel = new EmployeeModel { companyid = companyId };

                List<EmployeeModel> allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetCompanyWiseEmployee(employeeModel));
                allEmployee?.ToList().ForEach(e => e.empImgPath = (e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + e.empImgPath));

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
=======
        public virtual async Task<CustomJsonData> GetEmployeeByIdAsync(int employeeId)
        {
            try
            {
                var employeeModel = new EmployeeModel { EmpId = employeeId };
                var employee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.Select(employeeModel));
                if (employee != null)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
                {
                    employee = _employeeCommon.SetEmployeeProfilePic(employee);
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
        /// Method Name  :   GetAllEmployeesAsync
        /// Get manager wise all employee details by passing companyid, managerid
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetAllEmployeesAsync(int companyId, int managerId)
        {
            try
            {
                var employeeModel = new EmployeeModel { CompanyId = companyId, ManagerId = managerId };
                List<EmployeeModel> allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployee(employeeModel));
                allEmployee = _employeeCommon.SetEmployeeProfilePic(allEmployee);
                return _employeeCommon.GetResponseWitCheckAccessDenied(allEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetCompanyWiseEmployeeAsync
        /// Get company wise employee list for reporting manager dropdown
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetCompanyWiseEmployeeAsync(int companyId)
        {
            try
            {
<<<<<<< HEAD
                int userRoleId = Convert.ToInt32(_iClaims["RoleId"].Value);
                var employeeModel = new EmployeeModel { companyid = CompanyId, managerId = ManagerId, pagenumber = pagenumber, pagesize = pagesize, searchstring = searchstring, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;

                if (ManagerId == 0 && CompanyId == 0 && userRoleId == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    allEmployee = await Task.FromResult(_catalogDbContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithPagination(employeeModel));

                }
                else if (ManagerId == 0 && CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerWithPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeWithPagination(employeeModel));
                    GetEmployeeListWithHierachy(employeeModel, allEmployee);
                    GetEmployeeListWithTeamConfig(employeeModel, allEmployee);
                }

                allEmployee?.ToList().ForEach(e => e.empImgPath = ((e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath)));

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
=======
                var employeeModel = new EmployeeModel { CompanyId = companyId };
                List<EmployeeModel> allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetCompanyWiseEmployee(employeeModel));
                allEmployee = _employeeCommon.SetEmployeeProfilePic(allEmployee);
                return _employeeCommon.GetResponseWitCheckAccessDenied(allEmployee);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetCompanyWiseEmployeeWithPaginationAsync
        /// Get companywise employee list with pagination for trigger admin login
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetCompanyWiseEmployeeWithPaginationAsync(EmployeeModel employeeModel)
        {
            try
            {
<<<<<<< HEAD
                int userRoleId = Convert.ToInt32(_iClaims["RoleId"].Value);
                var employeeModel = new EmployeeModel { yearId = YearId, companyid = CompanyId, managerId = ManagerId, pagenumber = pagenumber, pagesize = pagesize, searchstring = searchstring, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;

                if (ManagerId == 0 && CompanyId == 0 && Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    allEmployee = await Task.FromResult(_catalogDbContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithPagination(employeeModel));

                }
                else if (ManagerId == 0 && CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerYearwise(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeYearwise(employeeModel));
                }

                allEmployee?.ToList().ForEach(e => e.empImgPath = ((e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath)));
                GetRedirectEmpListWithHierachyForActions(employeeModel, allEmployee);
                GetEmployeeListWithTeamConfig(employeeModel, allEmployee);

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
=======
                List<EmployeeModel> allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetCompanyWiseEmployeeWithPagination(employeeModel));
                allEmployee = _employeeCommon.SetEmployeeProfilePic(allEmployee);
                return _employeeCommon.GetResponseWithCheckNoRecord(allEmployee);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetAllEmployeesWithPaginationAsync
        /// Get employee list with pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetAllEmployeesWithPaginationAsync(EmployeeModel employeeModel)
        {
            try
            {
<<<<<<< HEAD
                int userRoleId = Convert.ToInt32(_iClaims["RoleId"].Value);

                var employeeModel = new EmployeeModel { yearId = yearId, companyid = companyId, managerId = managerId, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;

                if (managerId == 0 && companyId == 0 && userRoleId == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    allEmployee = await Task.FromResult(_catalogDbContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithoutPagination(employeeModel));

                }
                else if (managerId == 0 && companyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerYearwiseWithoutPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeYearwiseWithoutPagination(employeeModel));
                }

                allEmployee?.ToList().ForEach(e => e.empImgPath = ((e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath)));

                GetRedirectEmpListWithHierachyForActions(employeeModel, allEmployee);
                GetEmployeeListWithTeamConfig(employeeModel, allEmployee);

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
=======
                employeeModel.RoleId = _employeeCommon.GetRoleIdFromClaims();
                List<EmployeeModel> allEmployee = await GetAllEmployeeByUserRole(employeeModel);
                return _employeeCommon.GetResponseWithCheckNoRecord(allEmployee);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetAllEmployeesWithoutPaginationAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   23 May 2019
        /// Purpose      :   Get employee list without pagination
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="departmentList"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetAllEmployeesWithoutPaginationAsync(int companyId, int managerId, string departmentList)
        {
            try
            {
                var employeeModel = new EmployeeModel { CompanyId = companyId, ManagerId = managerId, DepartmentList = departmentList };
                List<EmployeeModel> allEmployee;
<<<<<<< HEAD

                if (ManagerId == 0 && CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwise(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(employeeModel));
                }

                allEmployee?.ToList().ForEach(e => e.empImgPath = (e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath));
                GetRedirectEmpListWithHierachyForActions(employeeModel, allEmployee);
                GetEmployeeListWithTeamConfig(employeeModel, allEmployee);

                if (allEmployee?.Count > 0)
=======
                if (managerId == 0 && companyId == 0 && _employeeCommon.GetRoleIdFromClaims() == Enums.DimensionElements.TriggerAdmin.GetHashCode())
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
                {
                    allEmployee = await Task.FromResult(_catalogDbContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithoutPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeWithoutPagination(employeeModel));
                    GetEmployeeListWithHierachy(employeeModel, allEmployee);
                }
                allEmployee = _employeeCommon.SetEmployeeProfilePic(allEmployee);
                return _employeeCommon.GetResponseWithCheckNoRecord(allEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetCompanyWiseEmployeeWithoutPaginationAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   23 May 2019
        /// Purpose      :   Get employee list without pagination when triggeradmin redirect on perticular client
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="departmentList"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetCompanyWiseEmployeeWithoutPaginationAsync(int companyId, string departmentList)
        {
            try
            {
<<<<<<< HEAD
                int userRoleId = Convert.ToInt32(_iClaims["RoleId"].Value);
                var employeeModel = new EmployeeModel { yearId = YearId, companyid = CompanyId, managerId = ManagerId, month = month, grade = grade, pagenumber = pagenumber, pagesize = pagesize, searchstring = searchstring, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;

                if (ManagerId == 0 && CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthByManagerYearwise(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthYearwise(employeeModel));
                }

                allEmployee?.ToList().ForEach(e => e.empImgPath = (e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath));
                GetRedirectEmpListWithHierachyForActions(employeeModel, allEmployee);
                GetEmployeeListWithTeamConfig(employeeModel, allEmployee);

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
=======
                var employeeModel = new EmployeeModel { CompanyId = companyId, DepartmentList = departmentList };
                List<EmployeeModel> allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetCompanyWiseEmployeeWithoutPagination(employeeModel));
                allEmployee = _employeeCommon.SetEmployeeProfilePic(allEmployee);
                return _employeeCommon.GetResponseWithCheckNoRecord(allEmployee);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetTriggerEmpList
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   02 July 2019
        /// Purpose      :   Get employeeList for dropdown of Trigger Employee Page
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetTriggerEmpList(int companyId, int managerId)
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
                    ActionList allPermission = await Task.FromResult(_employeeCommon.GetAllPermission(managerId, Enums.Actions.TriggerEmployee.GetHashCode()));
                    var permission = allPermission.ActionPermissions.Where(x => x.CanAdd).ToList();
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
        /// Method Name  :   GetTriggerEmpListV2
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   02 July 2019
        /// Purpose      :   Get employeeList for dropdown of Trigger Employee Page
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetTriggerEmpListV2(int companyId, int managerId)
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
                    ActionList allPermission = await Task.FromResult(_actionPermission.GetPermissionsV2(managerId).FirstOrDefault(x => (x.ActionId == Enums.Actions.TriggerEmployee.GetHashCode())));
                    List<ActionwisePermissionModel> permission = null;

                    permission = allPermission.ActionPermissions.Where(x => x.CanAdd).ToList();

                    if (permission != null && permission.Count > 0)
                    {
                        allEmployee = GetEmployeeListForActionsV2(permission, companyId, managerId);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(new List<EmployeeListModel>(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                    }
                }

                if (allEmployee.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.accessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetDashboardEmpListV2
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   02 July 2019
        /// Purpose      :   Get employeeList for dropdown of Employee Dashboard Page
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetDashboardEmpListV2(int companyId, int managerId)
        {
            try
            {
                List<EmployeeListModel> allEmployee = null;

                if (_employeeCommon.GetRoleIdFromClaims() == Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    EmployeeListModel employeeModel = new EmployeeListModel { CompanyId = companyId, ManagerId = managerId };
                    allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                }
                else
                {
                    ActionList allPermission = await Task.FromResult(_actionPermission.GetPermissionsV2(managerId == 0 ? _employeeCommon.GetEmployeeIdFromClaims() : managerId).FirstOrDefault(x => (x.ActionId == Enums.Actions.EmployeeDashboard.GetHashCode())));

                    List<ActionwisePermissionModel> permission = null;
                    permission = allPermission.ActionPermissions.Where(x => x.CanView).ToList();

                    if (permission != null && permission.Count > 0)
                    {
                        allEmployee = GetEmployeeListForActionsV2(permission, companyId, managerId);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(new List<EmployeeListModel>(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                    }
                }

                if (allEmployee.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.accessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to add user login details send notification
        /// </summary>
        /// <param name="employee"></param>
        private void AddAuthUserDetails(EmployeeModel employee)
        {
            if (employee.RoleId != Convert.ToInt32(EmployeeModel.Emprole.NonManager))
            {
                AddLoginUserDetails(employee);
                AddUserDetailsInCatalog(employee);
            }
        }

        /// <summary>
        /// Method to update user details 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="existingEmployee"></param>
        private void UpdateAuthUserDetails(EmployeeModel employee, EmployeeModel existingEmployee)
        {
            UserDetails userDetails = new UserDetails() { CompId = employee.CompanyId, ExistingEmpId = employee.EmpId };
            UserDetails existingUserDetails = _connectionContext.TriggerContext.UserDetailRepository.GetUserDetails(userDetails);

            if (existingEmployee.RoleId != employee.RoleId)
            {
                ManageUserDetailsOnRoleChange(employee, existingEmployee, existingUserDetails);
            }
            else if (employee.RoleId != Convert.ToInt32(EmployeeModel.Emprole.NonManager))
            {
                UpdateUserLoginDetails(employee);
                UpdateUserDetailsInCatalog(employee, existingUserDetails, false);
            }
            UpdateCompanyAdminDetails(existingEmployee, employee);
        }

        /// <summary>
        /// Method to Update/Delete company admin details based on current role
        /// </summary>
        /// <param name="existingEmployee"></param>
        /// <param name="employee"></param>
        private void UpdateCompanyAdminDetails(EmployeeModel existingEmployee, EmployeeModel employee)
        {
            if (existingEmployee.RoleId == EmployeeModel.Emprole.Admin.GetHashCode()
                            && employee.RoleId != EmployeeModel.Emprole.Admin.GetHashCode())
            {
                _catalogDbContext.CompanyAdminRepository.DeleteCompanyAdminDetails(existingEmployee);
            }
            else
            {
                _employeeCommon.InsertCompanyAdmin(employee);
            }
        }

        /// <summary>
        /// Method to udpate employee details on tenant and catalog on role change
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="existingEmployee"></param>
        /// <param name="existingUserDetails"></param>
        private void ManageUserDetailsOnRoleChange(EmployeeModel employee, EmployeeModel existingEmployee, UserDetails existingUserDetails)
        {
            if (employee.RoleId == EmployeeModel.Emprole.NonManager.GetHashCode())
            {
                _connectionContext.TriggerContext.EmployeeRepository.DeleteUser(employee);
                DeleteUserDetailsFromCatalog(existingUserDetails, existingEmployee, employee.UpdatedBy);
            }
            else
            {
                if (employee.RoleId != Convert.ToInt32(EmployeeModel.Emprole.NonManager)
                    && existingEmployee.RoleId == EmployeeModel.Emprole.NonManager.GetHashCode())
                {
                    employee.CreatedBy = employee.UpdatedBy;
                    AddAuthUserDetails(employee);
                }
                else
                {
                    UpdateUserLoginDetails(employee);
                    UpdateUserDetailsInCatalog(employee, existingUserDetails, true);
                }
            }
        }

        /// <summary>
        /// Add Users details in tenant for Admin/Executive/Manager
        /// </summary>
        /// <param name="employee"></param>
        private void AddLoginUserDetails(EmployeeModel employee)
        {
            try
            {
                UserDetails userDetails = new UserDetails
                {
                    EmpId = employee.EmpId,
                    UserName = employee.Email,
                    BActive = true,
                    CreatedBy = employee.CreatedBy,
                    Password = ""
                };
                _connectionContext.TriggerContext.UserDetailRepository.AddUser(userDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Register Users details in Authority
        /// </summary>
        /// <param name="employee"></param>
        private void AddUserDetailsInCatalog(EmployeeModel employee)
        {
            try
            {
                AuthUserDetails authUserDetails = SetAuthUserDetails(employee.Email, employee.PhoneNumber);
                AddAuthClaimDetails(employee, authUserDetails);

                if (employee.RoleId == Convert.ToInt32(EmployeeModel.Emprole.Admin))
                {
                    _catalogDbContext.CompanyAdminRepository.AddCompanyAdminDetails(employee);
                }
                _employeeSendEmail.SendEmployeeRegistrationMail(employee, authUserDetails.Id, authUserDetails.SecurityStamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method to add auth claim details of users
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="authUserDetails"></param>
        private void AddAuthClaimDetails(EmployeeModel employee, AuthUserDetails authUserDetails)
        {
            AuthUserDetails existAuthUserDetails = new AuthUserDetails() { ExistingEmail = employee.Email };
            existAuthUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(existAuthUserDetails);

            if (existAuthUserDetails != null)
            {
                employee = _employeeCommon.GetExistingEmployeeDetails(employee.EmpId);
                UpdateAspNetUser(employee, employee.EmpStatus);
            }
            else
            {
                authUserDetails.EmailConfirmed = employee.EmpStatus; //user active status set as emailconform at aspnetuser table
                authUserDetails = _catalogDbContext.AuthUserDetailsRepository.Insert(authUserDetails);
                AddAuthClaims(employee, authUserDetails.Id.ToString());
            }
        }

        /// <summary>
        /// Method to delete user details from catalog on user role change (to non manager)
        /// </summary>
        /// <param name="existingUserDetails"></param>
        /// <param name="existingEmployee"></param>
        /// <param name="userId"></param>
        private void DeleteUserDetailsFromCatalog(UserDetails existingUserDetails, EmployeeModel existingEmployee, int userId)
        {
            AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = existingUserDetails.UserName };
            authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
            _catalogDbContext.AuthUserDetailsRepository.Delete(authUserDetails);
            existingEmployee.UpdatedBy = userId;

            if (existingEmployee.RoleId == Convert.ToInt32(EmployeeModel.Emprole.Admin))
                _catalogDbContext.CompanyAdminRepository.DeleteCompanyAdminDetails(existingEmployee);
        }

        /// <summary>
        /// Method to delete user details from catalog on user delete
        /// </summary>
        /// <param name="employee"></param>
        private void DeleteUserDetailsFromCatalog(EmployeeModel employee)
        {
            if (employee.RoleId == Convert.ToInt32(EmployeeModel.Emprole.Admin))
                _catalogDbContext.CompanyAdminRepository.DeleteCompanyAdminDetails(employee);

            if (employee.RoleId != EmployeeModel.Emprole.NonManager.GetHashCode())
            {
                UpdateAspNetUser(employee);
            }
        }

        /// <summary>
        /// Method which update Auth user details 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="existingUserDetails"></param>
        /// <param name="isClaims"></param>
        private void UpdateUserDetailsInCatalog(EmployeeModel employee, UserDetails existingUserDetails, bool isClaims)
        {
            if (existingUserDetails != null && existingUserDetails.UserName != null)
            {
                AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = existingUserDetails.UserName };
                authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
                if (isClaims)
                {
                    UpdateAuthUserClaim(Convert.ToString(employee.RoleId), authUserDetails.Id);
                }

                if (employee.Email != existingUserDetails.UserName)
                {
                    _employeeCommon.UpdateAuthUser(employee, authUserDetails.Id);
                    _employeeSendEmail.SendEmailOnUpdateEmployeeDetails(employee, authUserDetails.Id, authUserDetails.SecurityStamp);
                }
                _employeeCommon.UpdateAuthUser(employee, authUserDetails.Id);
            }
        }

        /// <summary>
        /// Method which update Auth user claims details 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="authUserDetailsSubId"></param>
        private void UpdateAuthUserClaim(string roleId, string authUserDetailsSubId)
        {
            Claims claims = new Claims
            {
                ClaimType = Enums.ClaimType.RoleId.ToString(),
                ClaimValue = roleId,
                AuthUserId = authUserDetailsSubId
            };
            _catalogDbContext.ClaimsCommonRepository.Update(claims);
        }

        /// <summary>
        /// Add Users' claims
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="authUserId"></param>
        private void AddAuthClaims(EmployeeModel employee, string authUserId)
        {
            try
            {
                var employeeDashboardModel = new EmployeeDashboardModel { companyId = employee.CompanyId };
                string tenantName = _catalogDbContext.EmployeeDashboardRepository.GetTenantNameByCompanyId(employeeDashboardModel);

                List<Claims> claims = new List<Claims>
                {
                    new Claims() { ClaimType =Enums.ClaimType.CompId.ToString(), ClaimValue = Convert.ToString(employee.CompanyId), AuthUserId = authUserId },
                    new Claims() { ClaimType =Enums.ClaimType.EmpId.ToString(), ClaimValue = Convert.ToString(employee.EmpId), AuthUserId = authUserId },
                    new Claims() { ClaimType =Enums.ClaimType.RoleId.ToString(), ClaimValue = Convert.ToString(employee.RoleId), AuthUserId = authUserId },
                    new Claims() { ClaimType =Enums.ClaimType.Key.ToString(), ClaimValue = tenantName, AuthUserId = authUserId }
                };

                _catalogDbContext.ClaimsRepository.Insert(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Update Users details for Admin/Executive/Manager
        /// </summary>
        /// <param name="employee"></param>
        private void UpdateUserLoginDetails(EmployeeModel employee)
        {
            try
            {
                UserDetails userDetails = new UserDetails
                {
                    EmpId = employee.EmpId,
                    UserName = employee.Email,
                    BActive = employee.EmpStatus,
                    UpdatedBy = employee.UpdatedBy
                };

                _connectionContext.TriggerContext.UserDetailRepository.UpdateUser(userDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Update Email Confirmed during employee delete/ add employee with existing auth details
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="employeStatus"></param>
        private void UpdateAspNetUser(EmployeeModel employee, bool employeStatus = false)
        {
            AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = employee.Email };
            authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
            employee.EmpStatus = employeStatus;
            _employeeCommon.UpdateAuthUser(employee, authUserDetails.Id);
        }

        /// <summary>
<<<<<<< HEAD
        /// Get Notification by logged in user id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userId"></param>
        /// <returns>notification object</returns>
        public List<Trigger.DTO.NotificationModel> GetNotificationById(int userId)
        {
            EmployeeModel employeeModel = new EmployeeModel() { managerId = userId };

            List<Trigger.DTO.NotificationModel> notification = _connectionContext.TriggerContext.EmployeeRepository.GetNotificationById(employeeModel);
            return notification;
        }

        /// <summary>
        /// Update Email Confirmed during employee delete/ add employee with existing auth details
        /// </summary>
        /// <param name="employee"></param>

        public void UpdateAspNetUser(EmployeeModel employee, bool employeStatus = false)
        {
            AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = employee.email };
            authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
            employee.empStatus = employeStatus;
            UpdateAuthUser(employee, authUserDetails.Id);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userid"></param>
        /// <param name="empIdList"></param>
        public void SendEmail(List<EmployeeModel> employeeModels)
        {
            try
            {
                List<AuthUserDetails> authUserDetails = GetAuthUserDetails(employeeModels);
                var sendMailUsers = employeeModels.Join(authUserDetails, a => a.email, b => b.Email, (a, b) => new { a.empId, a.companyid, a.updatedBy, b.Email, b.Id, b.SecurityStamp });

                foreach (var drwData in sendMailUsers)
                {

                    EmployeeModel employeeModel = new EmployeeModel { email = drwData.Email.ToString(), empId = drwData.empId, companyid = drwData.companyid, updatedBy = drwData.updatedBy };
                    string url = _authURL + Convert.ToString(drwData.Id) + Messages.code + GenerateToken(Convert.ToString(drwData.SecurityStamp));
                    _backGroundJobRequest.SendRegistrationEmail(employeeModel, url, _contextAccessor.HttpContext.Request.Scheme + Messages.doubleCollen + _contextAccessor.HttpContext.Request.Host.Value, null);
                    _employeeContext.UpdateEmpIsMailSent(employeeModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        public async Task<CustomJsonData> SendMailAndUpdateFlag(int userid, EmployeeModel employee)
        {
            try
            {
                List<EmployeeModel> employees = GetEmployeesDetail(employee.companyid, employee.empIdList);
                if (employees.Count > 0)
                {
                    employees.ForEach(s => s.updatedBy = userid);
                    await Task.Run(() => SendEmail(employees));
                    return GetResponseMessageForEmailSent();
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }


        private List<EmployeeModel> GetEmployeesDetail(int companyId, string empIdList)
        {
            EmployeeModel employeeModel = new EmployeeModel { companyid = companyId, empIdList = empIdList };
            return _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByEmpIdsForMails(employeeModel);
        }

        private List<AuthUserDetails> GetAuthUserDetails(List<EmployeeModel> employeeModels)
        {

            List<AuthUserDetails> authUserDetails = new List<AuthUserDetails>();
            foreach (var authDetails in employeeModels)
            {
                AuthUserDetails authUsersMail = new AuthUserDetails() { Email = authDetails.email };
                authUsersMail = _catalogDbContext.AuthUserDetailsRepository.GetAuthDetailsByEmail(authUsersMail);

                authUserDetails.Add(authUsersMail);
            }

            return authUserDetails;
        }

        private CustomJsonData GetResponseMessageForEmailSent()
        {
            return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.mailSent);
        }

        /// <summary>
        /// Method Name  :   UpdateEmpSalaryAsync
        /// Manager will update employee's Salary 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="empSalary"></param>
        /// <returns></returns>
        public async Task<JsonData> UpdateEmpSalaryAsync(int userid, EmployeeSalary empSalary)
        {
            try
            {

                empSalary.updatedBy = userid;

                int result = await Task.FromResult(_employeeContext.UpdateEmployeeSalary(empSalary));
                if (result == 1)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.empSalaryUpdated);
                }
                else if (result == 2)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.empIsInactive);
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
        /// Method Name  :   GetAllEmployeesWithoutPaginationAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   23 May 2019
        /// Purpose      :   Get employee list without pagination
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="ManagerId"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllEmployeesWithoutPaginationAsync(int CompanyId, int ManagerId, string departmentlist)
        {
            try
            {
                var employeeModel = new EmployeeModel { companyid = CompanyId, managerId = ManagerId, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;
                if (ManagerId == 0 && CompanyId == 0 && Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    allEmployee = await Task.FromResult(_catalogDbContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithoutPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeWithoutPagination(employeeModel));
                    GetEmployeeListWithHierachy(employeeModel, allEmployee);
                    GetEmployeeListWithTeamConfig(employeeModel, allEmployee);
                }
                allEmployee?.ToList().ForEach(e => e.empImgPath = ((e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath)));

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetEmployeeListWithHierachy
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   21 June 2019
        /// Purpose      :   Get employeeList with hierachy to get employee relation with logged in employee
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="allEmployee"></param>
        private void GetEmployeeListWithHierachy(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {
            List<EmployeeModel> allEmployeeHierachy;
            allEmployeeHierachy = _connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeWithHierachy(employeeModel);

            allEmployee.Where(x => x.managerId == Math.Abs(employeeModel.managerId)).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Direct.GetHashCode());
            var empids = allEmployeeHierachy.Select(x => x.empId).ToList();
            allEmployee.Where(x => empids.Contains(x.empId) && x.managerId != Math.Abs(employeeModel.managerId)).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Hierarchal.GetHashCode());

            allEmployee.Where(x => x.empRelation == 0).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Indirect.GetHashCode());
            allEmployee.Where(x => x.empId == Math.Abs(employeeModel.managerId)).ToList().ForEach(x => x.empRelation = 0);
        }

        /// <summary>
        /// Method Name  :   GetEmpListWithHierachyForActions
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   2 July 2019
        /// Purpose      :   Get employeeList with hierachy to get employee relation with logged in employee
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="allEmployee"></param>
        private void GetEmpListWithHierachyForActions(EmployeeListModel employeeModel, List<EmployeeListModel> allEmployee)
        {
            List<EmployeeListModel> allEmployeeHierachy = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployeeWithHierachy(employeeModel).Where(x => x.empStatus).ToList();

            allEmployee.Where(x => x.managerId == employeeModel.managerId).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Direct.GetHashCode());
            var empids = allEmployeeHierachy.Select(x => x.empId).ToList();
            allEmployee.Where(x => empids.Contains(x.empId) && x.managerId != Math.Abs(employeeModel.managerId)).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Hierarchal.GetHashCode());

            allEmployee.Where(x => x.empRelation == 0).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Indirect.GetHashCode());
        }

        /// <summary>
        /// Method Name  :   GetCompanyWiseEmployeeWithoutPaginationAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   23 May 2019
        /// Purpose      :   Get employee list without pagination when triggeradmin redirect on perticular client
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetCompanyWiseEmployeeWithoutPaginationAsync(int companyId, string departmentlist)
        {
            try
            {
                var employeeModel = new EmployeeModel { companyid = companyId, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;
                allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetCompanyWiseEmployeeWithoutPagination(employeeModel));
                allEmployee?.ToList().ForEach(e => e.empImgPath = ((e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + e.empImgPath)));

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.noRecords);
                }
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
        /// <param name="yearId"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="grade"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetEmployeeByGradeWithoutPaginationAsync(int yearId, int companyId, int managerId, string grade, string departmentlist)
        {
            try
            {
                int userRoleId = Convert.ToInt32(_iClaims["RoleId"].Value);
                var employeeModel = new EmployeeModel { yearId = yearId, companyid = companyId, managerId = managerId, grade = grade, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;

                if (managerId == 0 && companyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwiseWithoutPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(employeeModel));
                }

                allEmployee?.ToList().ForEach(e => e.empImgPath = (e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath));
                GetRedirectEmpListWithHierachyForActions(employeeModel, allEmployee);
                GetEmployeeListWithTeamConfig(employeeModel, allEmployee);

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
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
        /// <param name="yearId"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="month"></param>
        /// <param name="grade"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetEmployeeByGradeAndMonthWithoutPaginationAsync(int yearId, int companyId, int managerId, string month, string grade, string departmentlist)
        {
            try
            {
                int userRoleId = Convert.ToInt32(_iClaims["RoleId"].Value);
                var employeeModel = new EmployeeModel { yearId = yearId, companyid = companyId, managerId = managerId, month = month, grade = grade, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;

                if (managerId == 0 && companyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination(employeeModel));
                }
                else
                {
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmpByGradeAndMonthYearwiseWithoutPagination(employeeModel));
                }

                allEmployee?.ToList().ForEach(e => e.empImgPath = (e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath));
                GetRedirectEmpListWithHierachyForActions(employeeModel, allEmployee);
                GetEmployeeListWithTeamConfig(employeeModel, allEmployee);

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }


        /// <summary>
        /// Method Name  :   GetTriggerEmpList
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   02 July 2019
        /// Purpose      :   Get employeeList for dropdown of Trigger Employee Page
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetTriggerEmpList(int companyId, int managerId)
        {
            try
            {
                List<EmployeeListModel> allEmployee;
                if (Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    EmployeeListModel employeeModel = new EmployeeListModel { companyId = companyId, managerId = managerId };
                    allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                }
                else
                {
                    ActionList allPermission = await Task.FromResult(_actionPermission.GetPermissions(managerId).FirstOrDefault(x => (x.ActionId == Enums.Actions.TriggerEmployee.GetHashCode())));
                    var permission = allPermission.ActionPermissions.Where(x => x.CanAdd).ToList();

                    if (permission != null && permission.Count > 0)
                    {
                        allEmployee = GetEmployeeListForActions(permission, companyId, managerId);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(new List<EmployeeListModel>(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                    }
                }

                if (allEmployee.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.accessDenied);
                }
            }
            catch (Exception ex)
=======
        /// Method which returns Employee insertion message based on result
        /// </summary>
        /// <param name="resultId"></param>
        /// <returns></returns>
        private CustomJsonData GetResponseMessageForInsert(int resultId)
        {
            switch ((InsertResultType)resultId)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
            {
                case InsertResultType.EmployeeEmailIdExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeExist);
                case InsertResultType.EmployeeSubmit:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.employeeSubmit);
                case InsertResultType.EmailIdExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_209), Messages.employeeIdIsExists);
                case InsertResultType.PhoneNumberExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.phoneNumberIsExists);
                default:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToAddEmployee);
            }
        }

        /// <summary>
        /// Method to get response on employee delete
        /// </summary>
        /// <param name="resultId"></param>
        /// <returns></returns>
        private CustomJsonData GetResponseMessageForDelete(int resultId)
        {
            if (resultId > 0)
            {
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.deleteEmployee);
            }
            else if (resultId == 0)
            {
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeNotExist);
            }
            else
            {
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToDeleteEmployee);
            }
        }

<<<<<<< HEAD
=======
        /// <summary>
        /// Method to prepare auth user details
        /// </summary>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private AuthUserDetails SetAuthUserDetails(string email, string phoneNumber)
        {
            return new AuthUserDetails()
            {
                UserClient = Messages.triggerAPI,
                NormalizedEmail = email.ToUpper(),
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                UserName = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                LockoutEnd = DateTime.Now.AddYears(1),
                SecurityStamp = Guid.NewGuid().ToString(Enums.GuidType.D.ToString()),
                ConcurrencyStamp = Guid.NewGuid().ToString(Enums.GuidType.B.ToString()),
                PasswordHash = null,
                TokenExpiration = DateTime.UtcNow.AddDays(1),
                Email = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                PhoneNumber = phoneNumber
            };
        }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        /// <summary>
        /// Method to send notiication on employee update
        /// </summary>
        /// <param name="existingEmployee"></param>
        /// <param name="employee"></param>
        /// <param name="resultId"></param>
        private void SendNotificationOnEmployeeUpdate(EmployeeModel existingEmployee, EmployeeModel employee, int resultId)
        {
            if (resultId == UpdateResultType.EmployeeUpdate.GetHashCode())
            {
                _notification.SendNotifications(existingEmployee.EmpId, existingEmployee);
                _notification.SendNotifications(employee.EmpId, employee);
            }
        }

        /// <summary>
        /// Method Name  :   GetAllEmployeeByUserRole
        /// Purpose      :   Get Employee list based on current user role.
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        private async Task<List<EmployeeModel>> GetAllEmployeeByUserRole(EmployeeModel employeeModel)
        {
            List<EmployeeModel> allEmployee;
            if (employeeModel.ManagerId == 0 && employeeModel.CompanyId == 0 && employeeModel.RoleId == Enums.DimensionElements.TriggerAdmin.GetHashCode())
            {
                allEmployee = await Task.FromResult(_catalogDbContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithPagination(employeeModel));
            }
            else if (employeeModel.ManagerId == 0 && employeeModel.CompanyId > 0 && (employeeModel.RoleId == Enums.DimensionElements.Manager.GetHashCode() || employeeModel.RoleId > Enums.DimensionElements.NonManager.GetHashCode()))
            {
                employeeModel.ManagerId = _employeeCommon.GetEmployeeIdFromClaims();
                allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerWithPagination(employeeModel));
            }
            else
            {
                allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeWithPagination(employeeModel));
                GetEmployeeListWithHierachy(employeeModel, allEmployee);
            }
            allEmployee = _employeeCommon.SetEmployeeProfilePic(allEmployee);
            return allEmployee;
        }

        /// <summary>
        /// Method Name  :   GetEmployeeListWithHierachy
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   21 June 2019
        /// Purpose      :   Get employeeList with hierachy to get employee relation with logged in employee
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="allEmployee"></param>
        private void GetEmployeeListWithHierachy(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {
            List<EmployeeModel> allEmployeeHierachy;
            allEmployeeHierachy = _connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeWithHierachy(employeeModel);

            allEmployee.Where(x => x.ManagerId == Math.Abs(employeeModel.ManagerId)).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Direct.GetHashCode());
            var empids = allEmployeeHierachy.Select(x => x.EmpId).ToList();
            allEmployee.Where(x => empids.Contains(x.EmpId) && x.ManagerId != Math.Abs(employeeModel.ManagerId)).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Hierarchal.GetHashCode());

            allEmployee.Where(x => x.EmpRelation == 0).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Indirect.GetHashCode());
            allEmployee.Where(x => x.EmpId == Math.Abs(employeeModel.ManagerId)).ToList().ForEach(x => x.EmpRelation = 0);
        }



        /// <summary>
        /// Common method to get employee list as per action permission
        /// </summary>
        /// <param name="actionwisePermissionModel"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        private List<EmployeeListModel> GetEmployeeListForActionsV2(List<ActionwisePermissionModel> actionwisePermissionModel, int companyId, int managerId)
        {
            EmployeeListModel employeeModel;
            List<EmployeeListModel> allEmployee;
            List<EmployeeListModel> departmentEmployees;
            List<EmployeeListModel> relationEmployees;

            if (_employeeCommon.GetRoleIdFromClaims() == Enums.DimensionElements.CompanyAdmin.GetHashCode() || actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Role.GetHashCode()))
            {
                employeeModel = new EmployeeListModel { CompanyId = companyId, ManagerId = managerId };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                allEmployee.Where(x => x.ManagerId == (employeeModel.ManagerId == 0 ? _employeeCommon.GetEmployeeIdFromClaims() : employeeModel.ManagerId)).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Direct.GetHashCode());
            }
            else if (actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Department.GetHashCode() || x.DimensionId == Enums.DimensionType.Relation.GetHashCode()))
            {
                employeeModel = new EmployeeListModel { CompanyId = companyId, ManagerId = 0 };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);

                managerId = (managerId == 0 ? _employeeCommon.GetEmployeeIdFromClaims() : managerId);
                departmentEmployees = GetDepartmentDimensionWiseEmployees(managerId, actionwisePermissionModel, allEmployee);
                relationEmployees = GetRelationDimensionWiseEmployees(managerId, companyId, actionwisePermissionModel, allEmployee);

                allEmployee = departmentEmployees.Union(relationEmployees).ToList();
            }
            else
            {
                allEmployee = null;
            }
            return allEmployee;


        }
<<<<<<< HEAD

        /// <summary>
        /// Method Name  :   GetRelationDimensionWiseEmployees
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   22 Aug 2019
        /// Purpose      :   Get employee list as per Relation Dimension Configuration
        /// </summary>
        /// <param name="managerId"></param>
        /// <param name="companyId"></param>
        /// <param name="actionwisePermissionModel"></param>
        /// <param name="allEmployee"></param>
        /// <returns></returns>
        private List<EmployeeListModel> GetRelationDimensionWiseEmployees(int managerId, int companyId, List<ActionwisePermissionModel> actionwisePermissionModel, List<EmployeeListModel> allEmployee)
        {

            List<EmployeeListModel> relationEmployees;
            List<int> relationDimensionId = actionwisePermissionModel.Where(x => x.DimensionId == Enums.DimensionType.Relation.GetHashCode()).Select(x => x.DimensionValueid).ToList();
            if (relationDimensionId.Count > 0)
            {
                GetEmpListWithHierachyForActions(new EmployeeListModel { companyId = companyId, managerId = managerId }, allEmployee.Where(x => x.empId != managerId).ToList());
                relationEmployees = allEmployee.Where(x => relationDimensionId.Contains(x.empRelation)).ToList();
            }
            else
            {
                relationEmployees = new List<EmployeeListModel>();
            }
            return relationEmployees;
        }
=======
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        /// <summary>
        /// Method Name  :   GetDepartmentDimensionWiseEmployees
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   22 Aug 2019
        /// Purpose      :   Get employee list as per Department Dimension Configuration
        /// </summary>
        /// <param name="managerId"></param>
        /// <param name="actionwisePermissionModel"></param>
        /// <param name="allEmployee"></param>
        /// <returns></returns>
        private List<EmployeeListModel> GetDepartmentDimensionWiseEmployees(int managerId, List<ActionwisePermissionModel> actionwisePermissionModel, List<EmployeeListModel> allEmployee)
        {

            List<EmployeeListModel> departmentEmployees;
            List<int> departmentDimensionId = actionwisePermissionModel.Where(x => x.DimensionId == Enums.DimensionType.Department.GetHashCode()).Select(x => x.DimensionValueid).ToList();

            if (departmentDimensionId.Count > 0)
            {
                int departmentId = GetDepartmentFromEmpId(managerId);
                if (departmentDimensionId.Count == 1 && actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Department.GetHashCode() && x.DimensionValueid == Enums.DimensionElements.Inside.GetHashCode()))
                {
                    departmentEmployees = allEmployee.Where(x => x.DepartmentId == departmentId && x.EmpId != managerId).ToList();
                }
                else if (departmentDimensionId.Count == 1 && actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Department.GetHashCode() && x.DimensionValueid == Enums.DimensionElements.Outside.GetHashCode()))
                {
                    departmentEmployees = allEmployee.Where(x => x.DepartmentId != departmentId && x.EmpId != managerId).ToList();
                }
                else
                {
<<<<<<< HEAD
                    departmentEmployees = allEmployee.Where(x => x.empId != managerId).ToList();
=======
                    departmentEmployees = allEmployee.Where(x => x.EmpId != managerId).ToList();
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
                }
            }
            else
            {
                departmentEmployees = new List<EmployeeListModel>();
            }
            return departmentEmployees;
        }

        /// <summary>
        /// Method Name  :   GetRelationDimensionWiseEmployees
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   22 Aug 2019
        /// Purpose      :   Get employee list as per Relation Dimension Configuration
        /// </summary>
        /// <param name="managerId"></param>
        /// <param name="companyId"></param>
        /// <param name="actionwisePermissionModel"></param>
        /// <param name="allEmployee"></param>
        /// <returns></returns>
        private List<EmployeeListModel> GetRelationDimensionWiseEmployees(int managerId, int companyId, List<ActionwisePermissionModel> actionwisePermissionModel, List<EmployeeListModel> allEmployee)
        {

            List<EmployeeListModel> relationEmployees;
            List<int> relationDimensionId = actionwisePermissionModel.Where(x => x.DimensionId == Enums.DimensionType.Relation.GetHashCode()).Select(x => x.DimensionValueid).ToList();
            if (relationDimensionId.Count > 0)
            {
                _employeeCommon.GetEmpListWithHierachyForActions(new EmployeeListModel { CompanyId = companyId, ManagerId = managerId }, allEmployee.Where(x => x.EmpId != managerId).ToList());
                relationEmployees = allEmployee.Where(x => relationDimensionId.Contains(x.EmpRelation)).ToList();
            }
            else
            {
                relationEmployees = new List<EmployeeListModel>();
            }
            return relationEmployees;
        }

        /// <summary>
        /// Method Name  :   GetDepartmentFromEmpId
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   20 Aug 2019
        /// Purpose      :   Get departmentId of given empId
        /// </summary>
        /// <param name="managerI"></param>
        /// <returns></returns>
        private int GetDepartmentFromEmpId(int managerId)
        {
            EmployeeModel employee = new EmployeeModel() { EmpId = managerId };
            return employee.DepartmentId = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employee).DepartmentId;
        }

<<<<<<< HEAD

        /// <summary>
        /// Method Name  :   GetRedirectEmpListWithHierachyForActions
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   09 July 2019
        /// Purpose      :   Get employeeList with hierachy to get employee relation with logged in employee when redirect from dashboard
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="allEmployee"></param>
        private void GetRedirectEmpListWithHierachyForActions(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
=======
        //Unused api to respond with message of app version upgrade
        public virtual async Task<JsonData> GetEmployee()
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        {
            return await Task.FromResult(JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_426), Messages.appUpgradeMessage));
        }

        private void GetEmployeeListWithTeamConfig(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {
            employeeModel.managerId = employeeModel.managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : Math.Abs(employeeModel.managerId);
            var teamEmployees = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeTeamRelationByManagerId(employeeModel);
            if (teamEmployees.Count == 0)
            {
                teamEmployees = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeNotPartOfTeamByManagerId(new EmployeeModel { managerId = Math.Abs(employeeModel.managerId) });
            }
            var teamConnetedEmpId = teamEmployees.Where(x => x.TeamType == Enums.DimensionElements.Connected.GetHashCode()).Select(x => x.empId);
            var teamOversightEmpId = teamEmployees.Where(x => x.TeamType == Enums.DimensionElements.Oversight.GetHashCode()).Select(x => x.empId);

            allEmployee.Where(x => teamConnetedEmpId.Contains(x.empId)).ToList().ForEach(x => x.TeamType = Enums.DimensionElements.Connected.GetHashCode());
            allEmployee.Where(x => teamOversightEmpId.Contains(x.empId) && x.empId != employeeModel.managerId).ToList().ForEach(x => x.TeamType = Enums.DimensionElements.Oversight.GetHashCode());
        }

        private List<EmployeeListModel> GetEmployeeListWithTeamForAction(int managerId, List<EmployeeListModel> allEmployee)
        {
            var teamEmployee = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeTeamRelationByManagerId(new EmployeeModel { managerId = Math.Abs(managerId) });
            if (teamEmployee.Count == 0)
            {
                teamEmployee = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeNotPartOfTeamByManagerId(new EmployeeModel { managerId = Math.Abs(managerId) });
            }
            allEmployee = (from emp in allEmployee
                           join tmemp in teamEmployee on emp.empId equals tmemp.empId into em
                           from tmemp in em.DefaultIfEmpty()
                           select new EmployeeListModel
                           {
                               empId = emp.empId,
                               employeeId = emp.employeeId,
                               companyId = emp.companyId,
                               firstName = emp.firstName,
                               middleName = emp.middleName,
                               lastName = emp.lastName,
                               email = emp.email,
                               departmentId = emp.departmentId,
                               department = emp.department,
                               managerId = emp.managerId,
                               empStatus = emp.empStatus,
                               roleId = emp.roleId,
                               empRelation = emp.empRelation,
                               TeamType = tmemp == null ? 0 : tmemp.TeamType,
                               lastAssessedDate = emp.lastAssessedDate,
                               ratingCompleted = emp.ratingCompleted
                           }).ToList();

            return allEmployee;
        }

        public List<EmployeeListModel> GetEmployeeListForActionsV2_1(List<ActionwisePermissionModel> actionwisePermissionModel, int companyId, int managerId)
        {
            EmployeeListModel employeeModel;
            List<EmployeeListModel> allEmployee;
            List<EmployeeListModel> departmentEmployees;
            List<EmployeeListModel> relationEmployees;
            List<EmployeeListModel> teamEmployees;

            if (Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Role.GetHashCode()))
            {
                employeeModel = new EmployeeListModel { companyId = companyId, managerId = managerId };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                allEmployee.Where(x => x.managerId == (employeeModel.managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : employeeModel.managerId)).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Direct.GetHashCode());
            }
            else if (actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Department.GetHashCode() || x.DimensionId == Enums.DimensionType.Relation.GetHashCode() || x.DimensionId == Enums.DimensionType.Team.GetHashCode()))
            {
                employeeModel = new EmployeeListModel { companyId = companyId, managerId = 0 };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);

                managerId = (managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : managerId);
                departmentEmployees = GetDepartmentDimensionWiseEmployees(managerId, actionwisePermissionModel, allEmployee);
                relationEmployees = GetRelationDimensionWiseEmployees(managerId, companyId, actionwisePermissionModel, allEmployee);
                teamEmployees = GetTeamWiseEmployees(managerId, actionwisePermissionModel, allEmployee);

                allEmployee = (from emp in allEmployee
                               join tmemp in teamEmployees on emp.empId equals tmemp.empId into em
                               from tmemp in em.DefaultIfEmpty()
                               join rlemp in relationEmployees on emp.empId equals rlemp.empId into rl
                               from rlemp in rl.DefaultIfEmpty()
                               select new EmployeeListModel
                               {
                                   empId = emp.empId,
                                   employeeId = emp.employeeId,
                                   companyId = emp.companyId,
                                   firstName = emp.firstName,
                                   middleName = emp.middleName,
                                   lastName = emp.lastName,
                                   email = emp.email,
                                   departmentId = emp.departmentId,
                                   department = emp.department,
                                   managerId = emp.managerId,
                                   empStatus = emp.empStatus,
                                   roleId = emp.roleId,
                                   empRelation = rlemp == null ? 0 : rlemp.empRelation,
                                   TeamType = tmemp == null ? 0 : tmemp.TeamType,
                                   lastAssessedDate = emp.lastAssessedDate,
                                   ratingCompleted = emp.ratingCompleted
                               }).ToList();


                List<int> empIds = teamEmployees.Select(x => x.empId).ToList();
                empIds.AddRange(departmentEmployees.Select(x => x.empId));
                empIds.AddRange(relationEmployees.Select(x => x.empId));
                allEmployee = allEmployee.Where(x => empIds.Contains(x.empId)).ToList();

            }
            else
            {
                allEmployee = null;
            }
            return allEmployee;


        }

        public List<EmployeeListModel> GetTeamWiseEmployees(int managerId, List<ActionwisePermissionModel> actionwisePermissionModel, List<EmployeeListModel> allEmployee)
        {
            List<EmployeeListModel> teamEmployees;
            List<int> teamDimensionId = actionwisePermissionModel.Where(x => x.DimensionId == Enums.DimensionType.Team.GetHashCode()).Select(x => x.DimensionValueid).ToList();
            if (teamDimensionId.Count > 0)
            {
                allEmployee = GetEmployeeListWithTeamForAction(managerId, allEmployee);
                teamEmployees = allEmployee.Where(x => teamDimensionId.Contains(x.TeamType)).ToList();
            }
            else
            {
                teamEmployees = new List<EmployeeListModel>();
            }
            return teamEmployees;
        }

        /// <summary>
        /// Method Name  :   GetNonManagersListAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   13 September 2019
        /// Purpose      :   Get Non Managers List by companyId 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetNonManagersListAsync(int companyId)
        {
            try
            {
                List<EmployeeBasicModel> allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeBasicRepository.GetAllNonManagerList(new EmployeeBasicModel { Companyid = companyId }));

                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.accessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }


        /// <summary>
        /// Method Name  :   GetActiveManagerList
        /// Author       :   Mayur Patel
        /// Creation Date:   26 September 2019
        /// Purpose      :   Get Active Managers List which have send registration email
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetActiveManagerList(int companyId)
        {
            try
            {
                EmployeeBasicModel employeeModel = new EmployeeBasicModel { Companyid = companyId };
                var aspnetUsersEmail = _catalogDbContext.EmployeeBasicRepository.GetAspnetUserEmailList(employeeModel).Select(x => x.Email);
                List<EmployeeBasicModel> allManager = await Task.FromResult(_connectionContext.TriggerContext.EmployeeBasicRepository.GetActiveManagerList());
                var managerList = allManager.Where(x => aspnetUsersEmail.Contains(x.Email));

                if (managerList?.Count() > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(managerList, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.noDataFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
