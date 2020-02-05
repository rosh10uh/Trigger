using Microsoft.AspNetCore.Http;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.BLL.Employee
{
    public class Employee
    {
        private readonly string _blobContainer = Messages.profilePic;
        private readonly string _storageAccountURL;
        private readonly ILogger<Employee> _logger;
        private readonly IClaims _iClaims;
        private readonly IConnectionContext _connectionContext;
        private readonly TriggerCatalogContext _catalogDbContext;

        private readonly BackgroundJobRequest _backGroundJobRequest;

        private readonly EmployeeContext _employeeContext;

        private readonly string _fromMailId;
        private readonly string _clientHost;
        private readonly int _portNo;
        private readonly string _password;

        private readonly string _storageAccountName;
        private readonly string _storageAccountAccessKey;
        private readonly string _landingURL;
        private readonly string _authURL;

        private readonly string _nHubConnection;
        private readonly string _nHubConnectionString;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IActionPermission _actionPermission;

        public Employee(IConnectionContext connectionContext, ILogger<Employee> logger, IClaims Claims, TriggerCatalogContext catalogDbContext,
                        EmployeeContext employeeContext, IHttpContextAccessor contextAccessor, BackgroundJobRequest backgroundJobRequest, IActionPermission actionPermission)
        {
            _connectionContext = connectionContext;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _iClaims = Claims;
            _catalogDbContext = catalogDbContext;
            _storageAccountURL = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.StorageAccountURL.ToString()];
            _backGroundJobRequest = backgroundJobRequest;
            _employeeContext = employeeContext;

            _fromMailId = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.SenderEmailID.ToString()];
            _clientHost = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.SmtpServer.ToString()];
            _portNo = Convert.ToInt32(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.SmtpPort.ToString()]);
            _password = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.SenderPassword.ToString()];

            _storageAccountName = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.StorageAccountName.ToString()];
            _storageAccountAccessKey = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.StorageAccountAccessKey.ToString()];
            _storageAccountURL = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.StorageAccountURL.ToString()];

            _authURL = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.AuthUrl.ToString()] + Messages.confirmEmailPath;
            _landingURL = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.LandingURL.ToString()];

            _nHubConnection = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.HubConnection.ToString()];
            _nHubConnectionString = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.HubConnectionString.ToString()];

            _actionPermission = actionPermission;
        }

        /// <summary>
        /// Method Name  :   GetEmployeeByIdAsync
        /// Method to get employee details by empid
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetEmployeeByIdAsync(int employeeId)
        {
            try
            {
                var employeeModel = new EmployeeModel { empId = employeeId };
                var employee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.Select(employeeModel));
                if (employee != null)
                {
                    employee.empImgPath = (employee.empImgPath == null || employee.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + employee.empImgPath);
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
        /// Method Name  :   GetEmployeeByIdForEditProfileAsync
        /// Method to get employee profile details by empid 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetEmployeeByIdForEditProfileAsync(int employeeId)
        {
            try
            {
                var employeeModel = new EmployeeProfile { empId = employeeId };
                var employee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeProfileRepository.Select(employeeModel));
                if (employee != null)
                {
                    employee.empImgPath = (employee.empImgPath == null || employee.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + employee.empImgPath);
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
        /// <param name="CompanyId"></param>
        /// <param name="ManagerId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllEmployeesAsync(int companyId, int managerId)
        {
            try
            {
                List<EmployeeModel> allEmployee;

                var employeeModel = new EmployeeModel { companyid = companyId, managerId = managerId };
                allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployee(employeeModel));

                allEmployee?.ToList().ForEach(e => e.empImgPath = (e.empImgPath == null || e.empImgPath == string.Empty) ? string.Empty : (_storageAccountURL + Messages.slash + _blobContainer + Messages.slash + e.empImgPath));

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

        //Unused api to respond with message of app version upgrade
        public async Task<JsonData> GetEmployee()
        {
            return await Task.FromResult(JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_426), Messages.appUpgradeMessage));
        }

        /// <summary>
        /// Method Name  :   GetCompanyWiseEmployeeAsync
        /// Get company wise employee list for reporting manager dropdown
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
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
        /// Method Name  :   GetCompanyWiseEmployeeWithPaginationAsync
        /// Get companywise employee list with pagination for trigger admin login
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetCompanyWiseEmployeeWithPaginationAsync(int CompanyId, int pagenumber, int pagesize, string searchstring, string departmentlist)
        {
            try
            {
                var employeeModel = new EmployeeModel { companyid = CompanyId, pagenumber = pagenumber, pagesize = pagesize, searchstring = searchstring, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetCompanyWiseEmployeeWithPagination(employeeModel));
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
        /// Method Name  :   GetAllEmployeesWithPaginationAsync
        /// Get employee list with pagination
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="ManagerId"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllEmployeesWithPaginationAsync(int CompanyId, int ManagerId, int pagenumber, int pagesize, string searchstring, string departmentlist)
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetAllEmployeesWithPaginationYearWiseAsync
        /// get year wise employee list with pagination
        /// </summary>
        /// <param name="YearId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="ManagerId"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllEmployeesWithPaginationYearWiseAsync(int YearId, int CompanyId, int ManagerId, int pagenumber, int pagesize, string searchstring, string departmentlist)
        {
            try
            {
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
                    if (ManagerId > 0)
                    {
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeYearwise(employeeModel));
                    }
                    else
                    {
                        employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllOrgEmployeeYearwise(employeeModel));
                    }
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
        /// <param name="yearId"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllEmployeesWithoutPaginationYearWiseAsync(int yearId, int companyId, int managerId, string departmentlist)
        {
            try
            {
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
                    if (managerId > 0)
                    {
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeYearwiseWithoutPagination(employeeModel));
                    }
                    else
                    {
                        employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllOrgEmployeeYearwiseWithoutPagination(employeeModel));
                    }
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
        /// <param name="YearId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="ManagerId"></param>
        /// <param name="grade"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetEmployeeByGradeWithPaginationAsync(int YearId, int CompanyId, int ManagerId, string grade, int pagenumber, int pagesize, string searchstring, string departmentlist)
        {
            try
            {
                int userRoleId = Convert.ToInt32(_iClaims["RoleId"].Value);
                var employeeModel = new EmployeeModel { yearId = YearId, companyid = CompanyId, managerId = ManagerId, grade = grade, pagenumber = pagenumber, pagesize = pagesize, searchstring = searchstring, departmentlist = departmentlist };
                List<EmployeeModel> allEmployee;

                if (ManagerId == 0 && CompanyId > 0 && (userRoleId == Enums.DimensionElements.Manager.GetHashCode() || userRoleId > Enums.DimensionElements.NonManager.GetHashCode()))
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwise(employeeModel));
                }
                else
                {
                    if (ManagerId > 0)
                    {
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithPagination(employeeModel));
                    }
                    else
                    {
                        employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetOrgEmployeeByGradeYearwiseWithPagination(employeeModel));
                    }
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
        /// get year wise, month wise, grade wise employee list without pagination
        /// </summary>
        /// <param name="YearId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="ManagerId"></param>
        /// <param name="month"></param>
        /// <param name="grade"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetEmployeeByGradeAndMonthWithPaginationAsync(int YearId, int CompanyId, int ManagerId, string month, string grade, int pagenumber, int pagesize, string searchstring, string departmentlist)
        {
            try
            {
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
                    if (ManagerId > 0)
                    {
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthYearwise(employeeModel));
                    }
                    else
                    {
                        employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetOrgEmployeeByGradeAndMonthYearwise(employeeModel));
                    }
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

        public async Task<CustomJsonData> GetEmployeeListRediectFromManagerDashboardAsync(EmployeeModel employeeModel)
        {
            try
            {
                List<EmployeeModel> allEmployee;

                allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeWithoutPagination(employeeModel));

                if (employeeModel.yearId > 0 && employeeModel.grade == string.Empty && employeeModel.month == string.Empty)
                {
                    allEmployee= GetYearWiseFilterEmployeesWithPagination(employeeModel,allEmployee);
                }
                else if (employeeModel.yearId > 0 && employeeModel.grade != string.Empty && employeeModel.month != null)
                {
                    allEmployee = GetEmployeeByGradeAndMonthWithPaginationAsync(employeeModel,allEmployee);
                }
                else if (employeeModel.yearId > 0 && employeeModel.grade != string.Empty && employeeModel.month == null)
                {
                    allEmployee = GetFilterEmployeeByGradeWithPagination(employeeModel,allEmployee);
                }
                
                if (allEmployee?.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        public  List<EmployeeModel> GetYearWiseFilterEmployeesWithPagination(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {
           
                if (employeeModel.managerId > 0)
                {
                allEmployee = _connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeYearwiseWithoutPagination(employeeModel);
                }
                else
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = _connectionContext.TriggerContext.EmployeeRepository.GetAllOrgEmployeeYearwiseWithoutPagination(employeeModel);
                }

            employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
            employeeModel.departmentId = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(new EmployeeModel { empId = employeeModel.managerId }).departmentId;

                return GetDimensionFilterWiseEmployeeList( employeeModel,allEmployee);
           
        }

        public List<EmployeeModel> GetEmployeeByGradeAndMonthWithPaginationAsync(EmployeeModel employeeModel,List<EmployeeModel> allEmployee)
        {

                if (employeeModel.managerId > 0)
                {
                    allEmployee = _connectionContext.TriggerContext.EmployeeRepository.GetEmpByGradeAndMonthYearwiseWithoutPagination(employeeModel);
                }
                else
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee = _connectionContext.TriggerContext.EmployeeRepository.GetOrgEmpByGradeAndMonthYearwiseWithoutPagination(employeeModel);
                }

            employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
            employeeModel.departmentId = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(new EmployeeModel { empId = employeeModel.managerId }).departmentId;

            return GetDimensionFilterWiseEmployeeList(employeeModel, allEmployee);
        }

        public List<EmployeeModel> GetFilterEmployeeByGradeWithPagination(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {

            employeeModel.managerId = Math.Abs(employeeModel.managerId);
            if (employeeModel.managerId > 0)
                {
                    allEmployee = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(employeeModel);
                }
                else
                {
                    employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                    allEmployee =_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseCompAdminWithoutPagination(employeeModel);
                }

            employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
            employeeModel.departmentId = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(new EmployeeModel { empId = employeeModel.managerId }).departmentId;

                return GetDimensionFilterWiseEmployeeList(employeeModel, allEmployee);
        }

        private List<EmployeeModel> GetDimensionFilterWiseEmployeeList(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {
            if (employeeModel.searchstring != "all records")
            {
                allEmployee = allEmployee.Where(s => s.firstName.Contains(employeeModel.searchstring) || s.lastName.Contains(employeeModel.searchstring)).ToList();
            }

            allEmployee = GetFilterEmployeeList(employeeModel, allEmployee);

            allEmployee = allEmployee.Skip((employeeModel.pagenumber - 1) * employeeModel.pagesize).Take(employeeModel.pagesize).ToList();
            return allEmployee;
        }

        /// <summary>
        /// Method to add employee
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> InsertAsync(int userid, EmployeeModel employee)
        {
            try
            {
                if (!string.IsNullOrEmpty(employee.phoneNumber) && IsPhoneNumberExistAspNetUser(new EmployeeModel { phoneNumber = employee.phoneNumber }, true))
                {
                    return GetResponseMessageForInsert(Enums.ResultTypeForInset.PhoneNumberExist.GetHashCode());
                }
                _connectionContext.TriggerContext.BeginTransaction();
                employee.createdBy = userid;
                int empId = await Task.FromResult(_employeeContext.InsertEmployee(employee));

                if (empId > 0)
                {
                    AddUserDetails(empId, employee);
                    _connectionContext.TriggerContext.Commit();

                    AddAdmin(employee);
                    SendNotifications(empId, employee);
                    return GetResponseMessageForInsert(1);
                }

                _connectionContext.TriggerContext.Commit();
                return GetResponseMessageForInsert(empId);

            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();

                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to check given phone number exists or not
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        public Boolean IsPhoneNumberExistAspNetUser(EmployeeModel employee, Boolean isAddMode = false)
        {
            if (isAddMode)
                employee.email = string.Empty;
            int PhonumberCount = _catalogDbContext.EmployeeRepository.GetAspNetUserCountByPhone(employee);
            return PhonumberCount > 0;
        }

        /// <summary>
        /// Method to add user details if role is Admin/Executive/Manager
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="employee"></param>
        private void AddUserDetails(int empId, EmployeeModel employee)
        {
            if (employee.roleId != Convert.ToInt32(EmployeeModel.Emprole.NonManager))
            {
                employee.empId = empId;
                AddUser(employee);
                AddAuthUser(employee);
            }
        }

        /// <summary>
        /// Method to send notification to reporting person if any employee is assigned
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="employee"></param>
        private void SendNotifications(int empId, EmployeeModel employee)
        {
            if (employee.managerId > 0)
            {
                employee.empId = empId;
                InvokeNotificationAsync(employee).Wait();

                EmployeeModel manager = new EmployeeModel() { empId = employee.managerId };
                manager = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(manager);

                if (manager != null)
                {
                    manager.createdBy = manager.empId;
                    if (manager.empId != 0 && manager.managerId > 0)
                    {
                        InvokeNotificationAsync(manager).Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Method to add admin in Catalog Database for Company admin list
        /// </summary>
        /// <param name="employee"></param>
        private void AddAdmin(EmployeeModel employee)
        {
            if (employee.roleId == Convert.ToInt32(EmployeeModel.Emprole.Admin))
            {
                _backGroundJobRequest.AddCompanyAdmin(employee, null, _employeeContext);
            }
        }

        /// <summary>
        /// Method to update employee details 
        /// update users details if user's role is Admin/Manager/Executive and send mail And send notification to logged in Users if any employee added/removed under it
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> UpdateAsync(int userid, EmployeeModel employee)
        {
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();

                EmployeeModel existingEmployee = GetExistingEmployeeDetails(employee.empId);

                if (!string.IsNullOrEmpty(employee.phoneNumber) && IsPhoneNumberExistAspNetUser(new EmployeeModel { phoneNumber = employee.phoneNumber, email = existingEmployee.email }))
                {
                    _connectionContext.TriggerContext.Commit();
                    return GetResponseMessageForUpdate(Enums.ResultTypeForUpdate.PhoneNumberExist.GetHashCode());
                }

                employee.updatedBy = userid;
                int resultId = await Task.FromResult(_employeeContext.UpdateEmployee(employee));

                if (resultId == 1)
                {
                    UpdateUserDetails(employee, existingEmployee);
                    _connectionContext.TriggerContext.Commit();

                    SendNotifications(existingEmployee.empId, existingEmployee);
                    SendNotifications(employee.empId, employee);
                    if (existingEmployee.roleId == EmployeeModel.Emprole.Admin.GetHashCode() && employee.roleId != EmployeeModel.Emprole.Admin.GetHashCode())
                    {
                        DeleteCompanyAdminDetails(existingEmployee);
                    }
                    else
                    {
                        InsertCompanyAdmin(employee);
                    }
                }
                else
                {
                    _connectionContext.TriggerContext.Commit();
                }

                return GetResponseMessageForUpdate(resultId);
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();

                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to update employee profile details 
        /// user can update details itself
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> UpdateProfileAsync(int userid, EmployeeProfile employee)
        {
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();

                EmployeeModel existingEmployee = GetExistingEmployeeDetails(employee.empId);

                if (existingEmployee != null)
                {
                    if (!string.IsNullOrEmpty(employee.phoneNumber) && IsPhoneNumberExistAspNetUser(new EmployeeModel { phoneNumber = employee.phoneNumber, email = existingEmployee.email }))
                    {
                        return GetResponseMessageForUpdate(Enums.ResultTypeForUpdate.PhoneNumberExist.GetHashCode());
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noDataFound);
                }

                employee.updatedBy = userid;
                var result = await Task.FromResult(_connectionContext.TriggerContext.EmployeeProfileRepository.Update(employee));

                switch (result.result)
                {
                    case 1:
                        return await UpdateProfile(existingEmployee, employee);
                    case 2:
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.empIsInactive);
                    case 4:
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.phoneNumberIsExists);
                    case 0:
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.employeeNotExist);
                    default:
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_401.GetHashCode(), Messages.unauthorizedToUpdateEmployee);
                }

            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
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
        private async Task<CustomJsonData> UpdateProfile(EmployeeModel existingEmployee, EmployeeProfile employeeProfile)
        {
            Boolean isPhoneNumberChange = employeeProfile.phoneNumber != existingEmployee.phoneNumber;
            existingEmployee.workCity = employeeProfile.workCity;
            existingEmployee.workState = employeeProfile.workState;
            existingEmployee.workZipcode = employeeProfile.workZipcode;
            existingEmployee.phoneNumber = employeeProfile.phoneNumber;

            AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = existingEmployee.email };
            authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
            if (authUserDetails != null)
            {
                if (isPhoneNumberChange)
                {
                    UpdateAuthUser(existingEmployee, authUserDetails.Id);
                }
                if (existingEmployee.roleId == EmployeeModel.Emprole.Admin.GetHashCode())
                {
                    InsertCompanyAdmin(existingEmployee);
                }
            }

            _connectionContext.TriggerContext.Commit();
            EmployeeProfile updatedEmpProfile = await Task.FromResult(_connectionContext.TriggerContext.EmployeeProfileRepository.Select(employeeProfile));
            employeeProfile.phoneConfirmed = updatedEmpProfile.phoneConfirmed;
            employeeProfile.optForSms = updatedEmpProfile.optForSms;

            return JsonSettings.UserCustomDataWithStatusMessage(employeeProfile, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.empProfileoUpdated);
        }

        /// <summary>
        /// Method to update allow sms reciveing
        /// user can update details itself
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> UpdateAllowSmsAsync(int userid, EmployeeProfile employee)
        {
            try
            {
                employee.updatedBy = userid;
                var emp = await Task.FromResult(_connectionContext.TriggerContext.EmployeeProfileRepository.UpdateAllowSMS(employee));

                if (emp.result == 1)
                {
                    EmployeeSMSNotification empSMSNotification = new EmployeeSMSNotification { empId = employee.empId, optForSms = employee.optForSms };
                    return JsonSettings.UserCustomDataWithStatusMessage(empSMSNotification, Convert.ToInt32(Enums.StatusCodes.status_200), string.Empty);
                }
                else if (emp.result == 2)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.empIsInactive);
                }
                else if (emp.result == 3)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.phoneNumberIsNotVerified);
                }
                else if (emp.result == 4)
                {
                    EmployeeSMSNotification empSMSNotification = new EmployeeSMSNotification { empId = employee.empId, optForSms = employee.optForSms };
                    return JsonSettings.UserCustomDataWithStatusMessage(empSMSNotification, Convert.ToInt32(Enums.StatusCodes.status_304), (employee.optForSms ? Messages.smsserviceallowed : Messages.smsservicenotallowed));
                }
                else if (emp.result == 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.employeeNotExist);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToUpdateEmployee);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method which returns Employee updation message based on result
        /// </summary>
        /// <param name="resultId"></param>
        /// <returns></returns>
        private CustomJsonData GetResponseMessageForUpdate(int resultId)
        {
            switch ((Enums.ResultTypeForUpdate)resultId)
            {
                case Enums.ResultTypeForUpdate.EmployeeNotExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeNotExist);
                case Enums.ResultTypeForUpdate.EmployeeUpdate:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.updateEmployee);
                case Enums.ResultTypeForUpdate.EmailIdExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeEmailIdIsExists);
                case Enums.ResultTypeForUpdate.EmployeeIdExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_209), Messages.employeeIdIsExists);
                case Enums.ResultTypeForUpdate.PhoneNumberExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.phoneNumberIsExists);
                default:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToUpdateEmployee);
            }
        }

        /// <summary>
        /// Method which returns Employee insertion message based on result
        /// </summary>
        /// <param name="resultId"></param>
        /// <returns></returns>
        private CustomJsonData GetResponseMessageForInsert(int resultId)
        {
            switch ((Enums.ResultTypeForInset)resultId)
            {
                case Enums.ResultTypeForInset.EmployeeEmailIdExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeExist);
                case Enums.ResultTypeForInset.EmployeeSubmit:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.employeeSubmit);
                case Enums.ResultTypeForInset.EmailIdExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_209), Messages.employeeIdIsExists);
                case Enums.ResultTypeForInset.PhoneNumberExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.phoneNumberIsExists);
                default:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToAddEmployee);
            }

        }

        /// <summary>
        /// Method to Company Admin 
        /// </summary>
        /// <param name="employee"></param>
        private void InsertCompanyAdmin(EmployeeModel employee)
        {
            if (employee.roleId == Convert.ToInt32(EmployeeModel.Emprole.Admin))
            {
                employee.createdBy = employee.updatedBy;
                _backGroundJobRequest.AddCompanyAdmin(employee, null, _employeeContext);
            }
        }

        /// <summary>
        /// Mthod to update user details 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="existingEmployee"></param>
        private void UpdateUserDetails(EmployeeModel employee, EmployeeModel existingEmployee)
        {
            Trigger.DTO.UserDetails existingUserDetails = GetExistingUserDetails(employee.companyid, employee.empId);

            if (existingEmployee.roleId != employee.roleId)
            {
                if (employee.roleId == EmployeeModel.Emprole.NonManager.GetHashCode())
                {
                    _connectionContext.TriggerContext.EmployeeRepository.DeleteUser(employee);

                    AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = existingUserDetails.userName };
                    authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
                    _employeeContext.DeleteAuthUser(authUserDetails);
                    existingEmployee.updatedBy = employee.updatedBy;
                    DeleteCompanyAdminDetails(existingEmployee);
                }
                else
                {
                    if (employee.roleId != Convert.ToInt32(EmployeeModel.Emprole.NonManager)
                        && existingEmployee.roleId == EmployeeModel.Emprole.NonManager.GetHashCode())
                    {
                        employee.createdBy = employee.updatedBy;
                        AddUser(employee);
                        AddAuthUser(employee);
                    }
                    else
                    {
                        UpdateAuthUserDetails(employee, existingUserDetails, true);
                    }
                }
            }
            else if (employee.roleId != Convert.ToInt32(EmployeeModel.Emprole.NonManager))
            {
                UpdateAuthUserDetails(employee, existingUserDetails, false);
            }
        }

        /// <summary>
        /// Method which get employee details before updation of employee data
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        private EmployeeModel GetExistingEmployeeDetails(int empId)
        {
            EmployeeModel employeeModel = new EmployeeModel() { empId = empId };
            return _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employeeModel);
        }

        /// <summary>
        /// Method which get user details before updation of user data
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="empId"></param>
        /// <returns></returns>
        private Trigger.DTO.UserDetails GetExistingUserDetails(int companyId, int empId)
        {
            UserDetails userDetails = new UserDetails() { CompId = companyId, existingEmpId = empId };
            return _connectionContext.TriggerContext.UserDetailRepository.GetUserDetails(userDetails);
        }

        /// <summary>
        /// Method which update Auth user details 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="existingUserDetails"></param>
        /// <param name="isClaims"></param>
        private void UpdateAuthUserDetails(EmployeeModel employee, Trigger.DTO.UserDetails existingUserDetails, bool isClaims)
        {
            UpdateUser(employee);

            if (existingUserDetails != null && existingUserDetails.userName != null)
            {
                AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = existingUserDetails.userName };
                authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
                if (isClaims)
                {
                    UpdateAuthUserClaim(Convert.ToString(employee.roleId), authUserDetails.Id);
                }

                if (employee.email != existingUserDetails.userName)
                {
                    UpdateAuthUser(employee, authUserDetails.Id);
                    var companyDetailsModel = _catalogDbContext.CompanyRepository.Select<CompanyDetailsModel>(new CompanyDetailsModel { compId = employee.companyid });

                    if (companyDetailsModel.contractStartDate.Date > DateTime.Now.Date && employee.empStatus)
                    {
                        _backGroundJobRequest.SendEmail(employee, _authURL + authUserDetails.Id + Messages.code + GenerateToken(authUserDetails.SecurityStamp), null);
                        employee.updatedBy = employee.createdBy;
                    }
                    else
                    {
                        if (companyDetailsModel.contractStartDate.Date <= DateTime.Now.Date && companyDetailsModel.contractEndDate.Date.AddDays(companyDetailsModel.gracePeriod) >= DateTime.Now.Date && employee.isMailSent && employee.empStatus)
                        {
                            SendEmailToUpdatedUser(employee);
                        }
                    }
                }
                UpdateAuthUser(employee, authUserDetails.Id);
            }
        }

        /// <summary>
        /// Method which update Auth user claims details 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="authUserDetailsSubId"></param>
        private void UpdateAuthUserClaim(string roleId, string authUserDetailsSubId)
        {
            Trigger.DTO.Claims claims = new Trigger.DTO.Claims
            {
                ClaimType = Enums.ClaimType.RoleId.ToString(),
                ClaimValue = roleId,
                AuthUserId = authUserDetailsSubId
            };

            _employeeContext.UpdateAuthUserClaims(claims);
        }

        /// <summary>
        /// Method to update employee profile pic 
        /// </summary>
        /// <returns>URL where profile pic is uploaded</returns>
        public async Task<JsonData> UpdateProfilePicAsync(int userid, EmpProfile empProfile)
        {
            try
            {
                empProfile.updatedBy = userid;
                Trigger.DTO.EmpProfilePic empProfilePic = GetEmployeeProfilePic(empProfile);
                empProfile.empImgPath = empProfilePic.empImgPath;

                int result = await Task.FromResult(_employeeContext.UpdateEmployeeProfilePic(empProfile));
                if (result == 1)
                {
                    //Delete Existing Pic Pending
                    UploadProfilePic(empProfilePic);

                    empProfile.empImage = string.Empty;
                    empProfile.empImgPath = Convert.ToString(empProfile.empImgPath == string.Empty ? string.Empty : _storageAccountURL + Messages.slash + _blobContainer + Messages.slash + empProfile.empImgPath);

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
        /// Method to get profile pic of logged in user 
        /// </summary>
        /// <param name="empProfile"></param>
        /// <returns></returns>
        private Trigger.DTO.EmpProfilePic GetEmployeeProfilePic(Trigger.DTO.EmpProfile empProfile)
        {
            Trigger.DTO.EmpProfilePic empProfilePic = new Trigger.DTO.EmpProfilePic
            {
                empImage = empProfile.empImage,
                empImgPath = (empProfile.empImgPath == string.Empty ? string.Empty : Guid.NewGuid().ToString() + empProfile.empImgPath),
                empFolderPath = empProfile.empFolderPath
            };
            return empProfilePic;
        }

        /// <summary>
        /// Method to Delete employee
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="EmpId"></param>
        /// <param name="UpdatedBy"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> DeleteAsync(int CompanyId, int EmpId, int UpdatedBy)
        {
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();

                EmployeeModel employee = new EmployeeModel { companyid = CompanyId, empId = EmpId, updatedBy = UpdatedBy, createdBy = EmpId };
                employee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employee));

                employee.createdBy = employee.empId;

                var isEmpDelete = _connectionContext.TriggerContext.EmployeeRepository.DeleteEmployee(employee);

                if (isEmpDelete.result > 0)
                {
                    _connectionContext.TriggerContext.EmployeeRepository.DeleteUser(employee);
                    _connectionContext.TriggerContext.Commit();

                    DeleteCompanyAdminDetails(employee);
                    if (employee.roleId != EmployeeModel.Emprole.NonManager.GetHashCode())
                    {
                        UpdateAspNetUser(employee);
                    }
                    SendNotifications(employee.empId, employee);

                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.deleteEmployee);
                }
                else if (isEmpDelete.result == 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeNotExist);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToDeleteEmployee);
                }
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();

                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Mthod to delete company admin details from reporting table from catalog database
        /// </summary>
        /// <param name="employee"></param>
        private void DeleteCompanyAdminDetails(EmployeeModel employee)
        {
            if (employee.roleId == Convert.ToInt32(EmployeeModel.Emprole.Admin))
            {
                employee.createdBy = employee.updatedBy;
                _backGroundJobRequest.DeleteCompanyAdmin(employee, null, _employeeContext);
            }
        }

        /// <summary>
        /// Register Users details in Authority
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="employee"></param>
        public void AddAuthUser(EmployeeModel employee)
        {
            try
            {
                AuthUserDetails authExistUserDetails = new AuthUserDetails() { ExistingEmail = employee.email };
                authExistUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authExistUserDetails);

                Trigger.DTO.AuthUserDetails authUserDetails = SetAuthUserDetails(employee.email, employee.phoneNumber);
                string securityStamp = authUserDetails.SecurityStamp;

                if (authExistUserDetails != null)
                {
                    employee = GetExistingEmployeeDetails(employee.empId);
                    UpdateAspNetUser(employee, employee.empStatus);
                }
                else
                {
                    authUserDetails.EmailConfirmed = employee.empStatus; //user active status set as emailconform at aspnetuser table
                    authUserDetails = _employeeContext.Insert1AuthUser(authUserDetails);
                    AddAuthClaims(employee, authUserDetails.Id.ToString());
                }

                if (employee.empStatus)
                {
                    var companyDetailsModels = _catalogDbContext.CompanyRepository.Select<List<CompanyDetailsModel>>(new CompanyDetailsModel { compId = employee.companyid });

                    if (companyDetailsModels != null && companyDetailsModels.Count > 0 && companyDetailsModels[0].contractStartDate.Date > DateTime.Now.Date)
                    {
                        _backGroundJobRequest.SendEmail(employee, _authURL + authUserDetails.Id + Messages.code + GenerateToken(securityStamp), null);
                        employee.updatedBy = employee.createdBy;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        private Trigger.DTO.AuthUserDetails SetAuthUserDetails(string email, string phoneNumber)
        {
            return new Trigger.DTO.AuthUserDetails()
            {
                UserClient = Messages.triggerAPI,
                NormalizedEmail = email.ToUpper(),
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                UserName = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                LockoutEnd = DateTime.Now.AddYears(1),
                SecurityStamp = Guid.NewGuid().ToString(Enums.Guid.D.ToString()),
                ConcurrencyStamp = Guid.NewGuid().ToString(Enums.Guid.B.ToString()),
                PasswordHash = null,
                TokenExpiration = DateTime.UtcNow.AddDays(1),
                Email = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                PhoneNumber = phoneNumber
            };
        }

        /// <summary>
        /// Add Users' claims
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="authUserId"></param>
        public void AddAuthClaims(EmployeeModel employee, string authUserId)
        {
            try
            {
                var employeeDashboardModel = new EmployeeDashboardModel { companyId = employee.companyid };
                string tenantName = _catalogDbContext.EmployeeDashboardRepository.GetTenantNameByCompanyId(employeeDashboardModel);

                List<Trigger.DTO.Claims> claims = new List<Trigger.DTO.Claims>
                {
                    new Trigger.DTO.Claims() { ClaimType =Enums.ClaimType.CompId.ToString(), ClaimValue = Convert.ToString(employee.companyid), AuthUserId = authUserId },
                    new Trigger.DTO.Claims() { ClaimType =Enums.ClaimType.EmpId.ToString(), ClaimValue = Convert.ToString(employee.empId), AuthUserId = authUserId },
                    new Trigger.DTO.Claims() { ClaimType =Enums.ClaimType.RoleId.ToString(), ClaimValue = Convert.ToString(employee.roleId), AuthUserId = authUserId },
                    new Trigger.DTO.Claims() { ClaimType =Enums.ClaimType.Key.ToString(), ClaimValue = tenantName, AuthUserId = authUserId }
                };

                _employeeContext.Insert1AuthClaims(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        public string GenerateToken(string securityStamp)
        {
            var b = System.Text.ASCIIEncoding.ASCII.GetBytes(securityStamp);
            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// Add Users details for Admin/Executive/Manager
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="password"></param>
        public void AddUser(EmployeeModel employee)
        {
            try
            {
                Trigger.DTO.UserDetails userDetails = new Trigger.DTO.UserDetails
                {
                    empId = employee.empId,
                    userName = employee.email,
                    bActive = true,
                    createdBy = employee.createdBy,
                    password = ""
                };

                _employeeContext.InsertUser(userDetails);
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
        /// <param name="password"></param>
        public void UpdateUser(EmployeeModel employee)
        {
            try
            {
                Trigger.DTO.UserDetails userDetails = new Trigger.DTO.UserDetails
                {
                    empId = employee.empId,
                    userName = employee.email,
                    bActive = employee.empStatus,
                    updatedBy = employee.updatedBy
                };

                _employeeContext.UpdateUser(userDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Update Users details in Authority
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="password"></param>
        public void UpdateAuthUser(EmployeeModel employee, string subId)
        {
            try
            {
                Trigger.DTO.AuthUserDetails authUserDetails = new Trigger.DTO.AuthUserDetails
                {
                    NormalizedEmail = employee.email.ToUpper(),
                    UserName = employee.email.ToUpper(),
                    NormalizedUserName = employee.email.ToUpper(),
                    Email = employee.email,
                    SubId = subId,
                    EmailConfirmed = employee.empStatus,
                    PhoneNumber = employee.phoneNumber,
                    PhoneNumberConfirmed = false
                };

                _employeeContext.Update1AuthUser(authUserDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Send Mail to register users using email template
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="password"></param>
        public void SendEmailToUpdatedUser(EmployeeModel employee)
        {
            string body = _backGroundJobRequest.GetTemplateByName(Messages.userUpdateEmailTemplate, employee.companyid);
            body = body.Replace(Messages.headingtext, employee.firstName);
            body = body.Replace(Messages.userId, employee.email);
            body = body.Replace(Messages.landingURL, _landingURL);
            EmailConfiguration.SendMail(_fromMailId, _password, _clientHost, _portNo, employee.email, Messages.registration, body, string.Empty, true);
        }


        /// <summary>
        /// Upload employee profile pic on Azure blob storage
        /// </summary>
        /// <param name="empProfilePic"></param>
        public void UploadProfilePic(Trigger.DTO.EmpProfilePic empProfilePic)
        {
            try
            {
                if (empProfilePic.empImage != null && empProfilePic.empImage != "" && empProfilePic.empImgPath != "" && empProfilePic.empImgPath != null)
                {
                    FileActions.UploadtoBlobAsync(empProfilePic.empImgPath, empProfilePic.empImage, _storageAccountName, _storageAccountAccessKey, _blobContainer).Wait();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Notification send to reporting person if any employee added/removed or changed 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task InvokeNotificationAsync(EmployeeModel employee)
        {
            NotificationHubClient Hub = NotificationHubClient.CreateClientFromConnectionString(_nHubConnectionString, _nHubConnection);
            List<Trigger.DTO.UserLoginModel> lstUserLogin = GetDeviceInfoById(GetUserLoginInfo(employee.managerId));
            DataTable registrationId = ConvertToDataTable.ToDataTable(lstUserLogin);

            var regId = registrationId.AsEnumerable().Where(dr => dr.Field<string>(Messages.deviceType) == Enums.DeviceType.Android.ToString()).Select(dr => dr.Field<string>(Messages.deviceId)).FirstOrDefault();
            var registrations1 = await Hub.GetAllRegistrationsAsync(500);
            var tag = registrations1.Where(x => x.RegistrationId == regId).Select(x => x.Tags);

            var tagname = new List<string>();
            foreach (var tags in tag)
            {
                tagname.Add(tags.FirstOrDefault());
            }

            registrationId.DefaultView.RowFilter = Messages.deviceTypeiOS;
            int iosTagCount = registrationId.DefaultView.ToTable().Rows.Count;
            await SendPushNotifications(Hub, tagname, employee.managerId, iosTagCount, registrationId.AsEnumerable().Where(dr => dr.Field<string>(Messages.deviceType) == Enums.DeviceType.iOS.ToString()).Select(dr => dr.Field<string>(Messages.deviceId)));

        }

        private async Task SendPushNotifications(NotificationHubClient Hub, List<string> tagName, int managerId, int iosTagCount, IEnumerable<string> tags)
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
                    else if (iosTagCount > 0)
                    {
                        outcome = await SendIOSNotifications(tags, iosTagCount, notification, Hub);
                    }
                    if (outcome != null)
                    {
                        notificationIds = GetNotificationString(outcome, notificationIds, notification.id);
                    }
                }
            }
            InvokeUpdateNotificationFlagIsSent(notificationIds.ToString());
        }

        private async Task<NotificationOutcome> SendGCMNotifications(List<string> tagName, Trigger.DTO.NotificationModel notification, NotificationHubClient Hub)
        {
            if (tagName.Count > 0)
            {

                var notif = Messages.notifyForEmployee.ToString().Replace("[0]", notification.message).Replace("[1]", notification.type).Replace("[2]", notification.id.ToString());
                return await Hub.SendGcmNativeNotificationAsync(notif, tagName);
            }

            return null;
        }

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

        private StringBuilder GetNotificationString(NotificationOutcome outcome, StringBuilder notificationIds, int notificationId)
        {
            if ((!((outcome.State == NotificationOutcomeState.Abandoned) ||
                            (outcome.State == NotificationOutcomeState.Unknown))))
            {
                if (notificationIds.Length == 0)
                    notificationIds.Append(notificationId);
                else
                    notificationIds.Append(Messages.comma + notificationId);
            }
            return notificationIds;
        }

        private int GetUserLoginInfo(int managerId)
        {
            UserLogin userLogin = new UserLogin() { existingEmpId = managerId };
            userLogin = _connectionContext.TriggerContext.UserLoginRepository.GetUserDetails(userLogin);
            return userLogin.userId;
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
        /// Get device info by logged in user id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userId"></param>
        /// <returns>list of users</returns>
        public List<Trigger.DTO.UserLoginModel> GetDeviceInfoById(int userId)
        {
            EmployeeModel employeeModel = new EmployeeModel() { userId = userId };

            List<Trigger.DTO.UserLoginModel> lstUsers = _connectionContext.TriggerContext.EmployeeRepository.GetDeviceInfoById(employeeModel);
            return lstUsers;
        }

        /// <summary>
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
                    if (managerId > 0)
                    {
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(employeeModel));
                    }
                    else
                    {
                        employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseCompAdminWithoutPagination(employeeModel));
                    }
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
                    if (managerId > 0)
                    {
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetEmpByGradeAndMonthYearwiseWithoutPagination(employeeModel));
                    }
                    else
                    {
                        employeeModel.managerId = Convert.ToInt32(_iClaims["EmpId"].Value);
                        allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetOrgEmpByGradeAndMonthYearwiseWithoutPagination(employeeModel));
                    }
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
        public async Task<CustomJsonData> GetTriggerEmpListV2(int companyId, int managerId)
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


        private List<EmployeeListModel> GetEmployeeListForActions(List<ActionwisePermissionModel> actionwisePermissionModel, int companyId, int managerId)
        {
            EmployeeListModel employeeModel;
            List<EmployeeListModel> allEmployee;

            if (Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Role.GetHashCode() || x.DimensionId == Enums.DimensionType.Department.GetHashCode()))
            {
                employeeModel = new EmployeeListModel { companyId = companyId, managerId = managerId };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                allEmployee.Where(x => x.managerId == (employeeModel.managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : employeeModel.managerId)).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Direct.GetHashCode());
            }
            else
            {
                employeeModel = new EmployeeListModel { companyId = companyId, managerId = 0 };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);

                int empId = managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : managerId;
                GetEmpListWithHierachyForActions(new EmployeeListModel { companyId = companyId, managerId = empId }, allEmployee.Where(x => x.empId != empId).ToList());

                List<int> dimensionValues = actionwisePermissionModel.Where(x => x.DimensionId == Enums.DimensionType.Relation.GetHashCode()).Select(x => x.DimensionValueid).ToList();
                allEmployee = allEmployee.Where(x => dimensionValues.Contains(x.empRelation)).ToList();
            }

            return allEmployee;
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

            if (Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Role.GetHashCode()))
            {
                employeeModel = new EmployeeListModel { companyId = companyId, managerId = managerId };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                allEmployee.Where(x => x.managerId == (employeeModel.managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : employeeModel.managerId)).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Direct.GetHashCode());
            }
            else if (actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Department.GetHashCode() || x.DimensionId == Enums.DimensionType.Relation.GetHashCode()))
            {
                employeeModel = new EmployeeListModel { companyId = companyId, managerId = 0 };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);

                managerId = (managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : managerId);
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
                    departmentEmployees = allEmployee.Where(x => x.departmentId == departmentId && x.empId != managerId).ToList();
                }
                else if (departmentDimensionId.Count == 1 && actionwisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Department.GetHashCode() && x.DimensionValueid == Enums.DimensionElements.Outside.GetHashCode()))
                {
                    departmentEmployees = allEmployee.Where(x => x.departmentId != departmentId && x.empId != managerId).ToList();
                }
                else
                {
                    departmentEmployees = allEmployee.Where(x => x.empId != managerId).ToList();
                }
            }
            else
            {
                departmentEmployees = new List<EmployeeListModel>();
            }
            return departmentEmployees;
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
            EmployeeModel employee = new EmployeeModel() { empId = managerId };
            return employee.departmentId = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employee).departmentId;
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
        public async Task<CustomJsonData> GetDashboardEmpList(int companyId, int managerId)
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
                    ActionList allPermission = await Task.FromResult(_actionPermission.GetPermissions(managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : managerId).FirstOrDefault(x => (x.ActionId == Enums.Actions.EmployeeDashboard.GetHashCode())));
                    var permission = allPermission.ActionPermissions.Where(x => x.CanView).ToList();

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
        public async Task<CustomJsonData> GetDashboardEmpListV2(int companyId, int managerId)
        {
            try
            {
                List<EmployeeListModel> allEmployee = null;

                if (Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    EmployeeListModel employeeModel = new EmployeeListModel { companyId = companyId, managerId = managerId };
                    allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                }
                else
                {
                    ActionList allPermission = await Task.FromResult(_actionPermission.GetPermissionsV2(managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : managerId).FirstOrDefault(x => (x.ActionId == Enums.Actions.EmployeeDashboard.GetHashCode())));

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
        /// Method Name  :   GetRedirectEmpListWithHierachyForActions
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   09 July 2019
        /// Purpose      :   Get employeeList with hierachy to get employee relation with logged in employee when redirect from dashboard
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="allEmployee"></param>
        private void GetRedirectEmpListWithHierachyForActions(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {
            employeeModel.managerId = employeeModel.managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : employeeModel.managerId;

            List<EmployeeModel> allEmployeeHierachy = _connectionContext.TriggerContext.EmployeeRepository.GetYearwiseEmployeeWithHierachy(employeeModel).Where(x => x.empStatus).ToList();

            allEmployee.Where(x => x.managerId == employeeModel.managerId).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Direct.GetHashCode());
            var empids = allEmployeeHierachy.Select(x => x.empId).ToList();
            allEmployee.Where(x => empids.Contains(x.empId) && x.managerId != Math.Abs(employeeModel.managerId)).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Hierarchal.GetHashCode());

            allEmployee.Where(x => x.empRelation == 0).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Indirect.GetHashCode());
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
                managerId = (managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : managerId);
                employeeModel = new EmployeeListModel { managerId = managerId };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployeeForActionBymanager(employeeModel);

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
                                   ManagerLastAssessedDate = emp.ManagerLastAssessedDate,
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

        /// <summary>
        /// Method Name  :   GetAllEmployeesWithPagination
        /// Author       :   Roshan Patel
        /// Creation Date:   22 November 2019
        /// Purpose      :   Get filter employee list with pagination
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="departmentlist"></param>
        /// <param name="searchstring"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllEmployeesWithPagination(int companyId, int managerId, int dimensionType, int dimensionValues, int pagenumber, int pagesize, string departmentlist, string searchString)
        {
            try
            {
                List<EmployeeModel> allEmployee;
                var employeeModel = new EmployeeModel { companyid = companyId, empId = managerId, managerId = managerId, searchstring = searchString, departmentlist = departmentlist };

                allEmployee = await Task.FromResult(_connectionContext.TriggerContext.EmployeeRepository.GetAllEmployeeWithoutPagination(employeeModel));

                managerId = Math.Abs(managerId);
                var departmentId = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(new EmployeeModel { empId = managerId }).departmentId;

                if (searchString != "all records")
                {
                    allEmployee = allEmployee.Where(s => s.firstName.Contains(searchString) || s.lastName.Contains(searchString)).ToList();
                }

                allEmployee = GetFilterEmployeeList(companyId, managerId, dimensionType, dimensionValues, allEmployee, employeeModel, departmentId);

                allEmployee = allEmployee.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();

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


        private List<EmployeeModel> GetFilterEmployeeList(int companyId, int managerId, int dimensionType, int dimensionValues, List<EmployeeModel> allEmployee, EmployeeModel employeeModel, int departmentId)
        {
            if (dimensionType == Enums.DimensionType.Relation.GetHashCode())
            {
                allEmployee = GetRelationList(companyId, managerId, dimensionValues, allEmployee);
            }
            else if (dimensionType == Enums.DimensionType.Department.GetHashCode())
            {
                switch ((Enums.DimensionElements)dimensionValues)
                {
                    case Enums.DimensionElements.Inside:
                        allEmployee = allEmployee.Where(x => x.departmentId == departmentId).ToList();
                        break;
                    case Enums.DimensionElements.Outside:
                        allEmployee = allEmployee.Where(x => x.departmentId != departmentId).ToList();
                        break;
                }
            }
            else if (dimensionType == Enums.DimensionType.Team.GetHashCode())
            {
                allEmployee = GetTeamList(managerId, dimensionValues, allEmployee, employeeModel);

            }

            return allEmployee;
        }

        /// <summary>
        /// Method Name  :   GetFilterEmployeeList
        /// Author       :   Roshan Patel
        /// Creation Date:   22 November 2019
        /// Purpose      :   Get filter employee list 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="dimensionType"></param>
        /// <param name="dimensionValues"></param>
        /// <param name="allEmployee"></param>
        /// <param name="employeeModel"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        private List<EmployeeModel> GetFilterEmployeeList(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {
            
            if (employeeModel.dimensionType == Enums.DimensionType.Relation.GetHashCode())
            {
                allEmployee = GetRelationList(employeeModel.companyid, employeeModel.managerId, employeeModel.dimensionValues,allEmployee);
            }
            else if (employeeModel.dimensionType == Enums.DimensionType.Department.GetHashCode())
            {
                switch ((Enums.DimensionElements)employeeModel.dimensionValues)
                {
                    case Enums.DimensionElements.Inside:
                        allEmployee = allEmployee.Where(x => x.departmentId == employeeModel.departmentId).ToList();
                        break;
                    case Enums.DimensionElements.Outside:
                        allEmployee = allEmployee.Where(x => x.departmentId != employeeModel.departmentId).ToList();
                        break;
                }
            }
            else if (employeeModel.dimensionType == Enums.DimensionType.Team.GetHashCode())
            {
                allEmployee = GetTeamList(employeeModel.managerId, employeeModel.dimensionValues, allEmployee, employeeModel);

            }

            return allEmployee;
        }

        /// <summary>
        /// Method Name  :   GetTeamList
        /// Author       :   Roshan Patel
        /// Creation Date:   22 November 2019
        /// Purpose      :   Get filter team list 
        /// </summary>
        /// <param name="managerId"></param>
        /// <param name="dimensionValues"></param>
        /// <param name="allEmployee"></param>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        private List<EmployeeModel> GetTeamList(int managerId, int dimensionValues, List<EmployeeModel> allEmployee, EmployeeModel employeeModel)
        {
            employeeModel.managerId = employeeModel.managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : Math.Abs(employeeModel.managerId);
            var teamEmployees = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeTeamRelationByManagerId(employeeModel);

            var teamConnetedEmpId = teamEmployees.Where(x => x.TeamType == Enums.DimensionElements.Connected.GetHashCode()).Select(x => x.empId);
            var teamOversightEmpId = teamEmployees.Where(x => x.TeamType == Enums.DimensionElements.Oversight.GetHashCode()).Select(x => x.empId);

            allEmployee.Where(x => teamConnetedEmpId.Contains(x.empId)).ToList().ForEach(x => x.TeamType = Enums.DimensionElements.Connected.GetHashCode());
            allEmployee.Where(x => teamOversightEmpId.Contains(x.empId) && x.empId != managerId).ToList().ForEach(x => x.TeamType = Enums.DimensionElements.Oversight.GetHashCode());

            allEmployee = allEmployee.Where(x => x.TeamType == dimensionValues).ToList();
            return allEmployee;
        }

        /// <summary>
        /// Method Name  :   GetRelationList
        /// Author       :   Roshan Patel
        /// Creation Date:   22 November 2019
        /// Purpose      :   Get filter relation list 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="dimensionValues"></param>
        /// <param name="allEmployee"></param>
        /// <returns></returns>
        private List<EmployeeModel> GetRelationList(int companyId, int managerId, int dimensionValues, List<EmployeeModel> allEmployee)
        {
            switch ((Enums.DimensionElements)dimensionValues)
            {
                case Enums.DimensionElements.Direct:
                    allEmployee.Where(x => x.managerId == Math.Abs(managerId)).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Direct.GetHashCode());
                    break;
                case Enums.DimensionElements.Hierarchal:
                    allEmployee = GetEmployeeHierachyList(companyId, managerId, allEmployee);
                    break;
                case Enums.DimensionElements.Indirect:
                    allEmployee = GetEmployeeHierachyList(companyId, managerId, allEmployee);
                    allEmployee.Where(x => x.managerId == managerId).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Direct.GetHashCode());
                    allEmployee.Where(x => x.empRelation == 0).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Indirect.GetHashCode());
                    break;
            }

            allEmployee = allEmployee.Where(x => x.empRelation == dimensionValues).ToList();
            return allEmployee;
        }

        /// <summary>
        /// Method Name  :   GetEmployeeHierachyList
        /// Author       :   Roshan Patel
        /// Creation Date:   22 November 2019
        /// Purpose      :   Get filter employee hierachy list 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="allEmployee"></param>
        /// <returns></returns>
        private List<EmployeeModel> GetEmployeeHierachyList(int companyId, int managerId, List<EmployeeModel> allEmployee)
        {
            List<EmployeeListModel> allEmployeeHierachy = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployeeWithHierachy(new EmployeeListModel { companyId = companyId, managerId = managerId }).Where(x => x.empStatus).ToList();
            var empIds = allEmployeeHierachy.Select(x => x.empId).ToList();
            allEmployee.Where(x => empIds.Contains(x.empId) && x.managerId != managerId).ToList().ForEach(x => x.empRelation = Enums.DimensionElements.Hierarchal.GetHashCode());
            return allEmployee;
        }
    }
}
