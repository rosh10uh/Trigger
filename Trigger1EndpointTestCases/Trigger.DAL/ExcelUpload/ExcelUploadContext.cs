using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Trigger.BLL.Shared;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.DAL.ExcelUpload
{
    /// <summary>
    /// Contains method to manage multiple operation.
    /// </summary>
    public class ExcelUploadContext
    {
        private readonly ILogger<ExcelUploadContext> _iLogger;
        private readonly IConnectionContext _connectionContext;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly BackgroundJobRequest _backGroundJobRequest;
        private readonly EmployeeContext _employeeContext;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Initializes a new instance of the roles class using IDaoContextFactory and Connection.
        /// </summary>
        /// <param name="iLogger"></param>
        /// <param name="connectionContext"></param>
        /// <param name="catalogDbContext"></param>
        /// <param name="employeeContext"></param>
        /// <param name="appSettings"></param>
        /// <param name="backGroundJobRequest"></param>
        public ExcelUploadContext(ILogger<ExcelUploadContext> iLogger, IConnectionContext connectionContext,
            TriggerCatalogContext catalogDbContext, EmployeeContext employeeContext,
            IOptions<AppSettings> appSettings, BackgroundJobRequest backGroundJobRequest)
        {
            _iLogger = iLogger;
            _connectionContext = connectionContext;
            _catalogDbContext = catalogDbContext;
            _backGroundJobRequest = backGroundJobRequest;
            _employeeContext = employeeContext;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Add/Edit employee details based on employee details througth Excel upload 
        /// </summary>
        /// <param name="countRecordModel">Uploaded excel records of employee list and company Id </param>
        /// <returns> result </returns>
        public int AddNewDataToEmpData(CountRecordModel countRecordModel)
        {
            int result = 0;
            try
            {
                List<ExcelEmployeesModel> excelEmployeesModels = countRecordModel.LstNewExcelUpload;
                _connectionContext.TriggerContext.BeginTransaction();
                var companyId = Convert.ToInt32(countRecordModel.CompanyId);
                var existEmployee = _connectionContext.TriggerContext.EmployeeRepository.GetExcelEmployees(new EmployeeModel { CompanyId = companyId });
                var newEmployee = ConvertToDataTable.ToDataTable(countRecordModel.LstNewExcelUpload);
                var emp = _connectionContext.TriggerContext.EmployeeExcelRepository.AddEmpolyee(new EmployeeExcelModel { TblEmployee = newEmployee });
                result = emp.Result;

                if (result > 0)
                {
                    bool anyMistMacthData = excelEmployeesModels.Any(x => x.empId != 0);
                    if (anyMistMacthData)
                    {
                        UploadMistMatchEmployee(companyId, countRecordModel.LstNewExcelUpload, existEmployee);
                    }
                    else
                    {
                        UploadNewEmployees(excelEmployeesModels, companyId);
                    }
                }
                _connectionContext.TriggerContext.Commit();
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _iLogger.LogError(ex.Message);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Upload mistache employee details througth Excel upload 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="lstNewExcelUpload">Excel uploadedemployee list</param>
        /// <param name="existEmployee">Exist employee list of current company</param>
        private void UploadMistMatchEmployee(int companyId, List<ExcelEmployeesModel> lstNewExcelUpload, List<EmployeeModel> existEmployee)
        {
            try
            {
                var companyDetailsModel = _catalogDbContext.CompanyRepository.Select<CompanyDetailsModel>(new CompanyDetailsModel { compId = companyId });
                UploadRoleChangedEmployees(lstNewExcelUpload, existEmployee, companyDetailsModel);// same empid with changed roleid
                var emailChangedEmployeeLst = GetEmailChangedEmployeesList(lstNewExcelUpload, existEmployee); // For email id change
                if (emailChangedEmployeeLst != null)
                {
                    UpdateAuthUserDetails(emailChangedEmployeeLst, companyId, false);  
                }
                UpdateStatusPhnChangedNewUsersDetails(lstNewExcelUpload, existEmployee);
                UpdateEmailConfimOfDeletedUser(lstNewExcelUpload, existEmployee);
                AddCompanyAdminDetails(lstNewExcelUpload, existEmployee);
                if (companyDetailsModel != null && emailChangedEmployeeLst != null)
                {
                    var employeeLstForSentEml = GetMismatchEmployeesForSentEmail(lstNewExcelUpload, existEmployee);
                    if (employeeLstForSentEml != null && employeeLstForSentEml.Any())
                    {
                        SendEmailToEmployee(companyDetailsModel, employeeLstForSentEml);
                    }
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Upload EmailConfirm Flag set as true if upload deleted user details
        /// </summary>
        /// <param name="lstNewExcelUpload">Excel uploadedemployee list</param>
        /// <param name="existEmployee">Exist employee list of current company</param>
        private void UpdateEmailConfimOfDeletedUser(List<ExcelEmployeesModel> lstNewExcelUpload, List<EmployeeModel> existEmployee)
        {
            try
            {
                List<int> deletedEmployeeIds = existEmployee.Where(x => !x.BActive).Select(x => x.EmpId).ToList();

                if (deletedEmployeeIds != null && deletedEmployeeIds.Any())
                {
                    var deletedNewUsers = lstNewExcelUpload.Where(x => deletedEmployeeIds.Contains(x.empId));
                    if (deletedNewUsers != null && deletedNewUsers.Any())
                    {
                        foreach (var emp in deletedNewUsers)
                        {
                            EmployeeModel authEmployee = new EmployeeModel { Email = emp.email, EmpStatus = emp.empStatus, PhoneNumber = emp.phonenumber };
                            AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = emp.email };
                            authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
                            UpdateAuthUser(authEmployee, authUserDetails.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Update phone number status PhoneConfirmed as false on phone number change,
        /// </summary>
        /// <param name="lstNewExcelUpload">Excel uploaded employee list</param>
        /// <param name="existEmployee">Exist employee list of current company</param>
        private void UpdateStatusPhnChangedNewUsersDetails(List<ExcelEmployeesModel> lstNewExcelUpload, List<EmployeeModel> existEmployee)
        {
            try
            {
                var statusChangedNewUsers = GetStatusphnChangedNewUsers(lstNewExcelUpload, existEmployee);
                if (statusChangedNewUsers != null && statusChangedNewUsers.Any())
                {
                    foreach (var emp in statusChangedNewUsers)
                    {
                        EmployeeModel authEmployee = new EmployeeModel { Email = emp.email, EmpStatus = emp.empStatus, PhoneNumber = emp.phonenumber };
                        AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = emp.email };
                        authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
                        UpdateAuthUser(authEmployee, authUserDetails.Id);
                        authEmployee.UpdatedBy = emp.updatedBy;
                        authEmployee.EmpId = emp.empId;
                        UpdateUser(authEmployee);
                    }
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Send email updation mail or employee registration mail based on company contract start date
        /// </summary>
        /// <param name="companyDetails">Company details</param>
        /// <param name="employeeForSentMail">employee list for send email</param>
        private void SendEmailToEmployee(CompanyDetailsModel companyDetails, List<EmployeeModel> employeeForSentMail)
        {
            try
            {
                if (companyDetails.contractStartDate <= DateTime.Now.Date && companyDetails.contractEndDate.Date.AddDays(companyDetails.gracePeriod) >= DateTime.Now.Date)
                {
                    _backGroundJobRequest.SendEmailToUpdatedUser(employeeForSentMail, companyDetails.compId);
                }
                else
                {
                    if (companyDetails.contractStartDate.Date > DateTime.Now.Date)
                    {
                        foreach (EmployeeModel employee in employeeForSentMail)
                        {
                            AuthUserDetails authUser = new AuthUserDetails() { Email = employee.Email };
                            authUser = _catalogDbContext.AuthUserDetailsRepository.GetAuthDetailsByEmail(authUser);

                            int empId = employeeForSentMail.FirstOrDefault(x => x.Email == authUser.Email).EmpId;
                            SendRegistrationMail(authUser.Id, empId, authUser.Email, authUser.SecurityStamp, companyDetails.compId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Update aspnetuser details
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="subId"></param>
        private void UpdateAuthUser(EmployeeModel employee, string subId)
        {
            try
            {
                AuthUserDetails authUserDetails = new AuthUserDetails
                {
                    NormalizedEmail = employee.Email.ToUpper(),
                    UserName = employee.Email.ToUpper(),
                    NormalizedUserName = employee.Email.ToUpper(),
                    Email = employee.Email,
                    SubId = subId,
                    EmailConfirmed = employee.EmpStatus,
                    PhoneNumber = employee.PhoneNumber,
                    PhoneNumberConfirmed = false
                };
                _employeeContext.Update1AuthUser(authUserDetails);
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update auth user claim details
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
            _employeeContext.UpdateAuthUserClaims(claims);
        }

        /// <summary>
        /// Update Users details for Admin/Executive/Manager
        /// </summary>
        /// <param name="employee"></param>
        public void UpdateUser(EmployeeModel employee)
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
                _employeeContext.UpdateUser(userDetails);
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update auth Users details in catalog  for Admin/Executive/Manager
        /// Add/Update/Delete company admin details
        /// Add/Update/Delete aspnetusers details
        /// Add/Update/Delete user claims details
        /// </summary>
        /// <param name="employeeList"></param>
        /// <param name="companyId"></param>
        /// <param name="isClaims"></param>
        private void UpdateAuthUserDetails(List<EmployeeModel> employeeList, int companyId, bool isClaims)
        {
            try
            {
                foreach (EmployeeModel employeeModel in employeeList)
                {
                    employeeModel.CompanyId = companyId;
                    UserDetails userDetails = new UserDetails() { CompId = companyId, ExistingEmpId = employeeModel.EmpId };
                    var existingUserDetails = _connectionContext.TriggerContext.UserDetailRepository.GetUserDetails(userDetails);

                    if (employeeModel.RoleId != Enums.DimensionElements.CompanyAdmin.GetHashCode())
                    {
                        _catalogDbContext.CompanyAdminRepository.DeleteCompanyAdminDetails(employeeModel);
                    }

                    AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = existingUserDetails.UserName };
                    authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
                    if (employeeModel.RoleId == Enums.DimensionElements.NonManager.GetHashCode())
                    {
                        _connectionContext.TriggerContext.EmployeeRepository.DeleteUser(employeeModel);
                        _employeeContext.DeleteAuthUser(authUserDetails);
                    }
                    else
                    {
                        UpdateUser(employeeModel);
                        UpdateAuthUser(employeeModel, authUserDetails.Id);
                        if (isClaims)
                        {
                            UpdateAuthUserClaim(Convert.ToString(employeeModel.RoleId), authUserDetails.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message.ToString());
                throw;
            }
        }

        /// <summary>
        /// Add newly added employee details in catalog
        /// Add company admin details
        /// Add aspnetusers details
        /// Add auth user claims details
        /// </summary>
        /// <param name="excelEmployeesList"></param>
        /// <param name="companyId"></param>
        private void UploadNewEmployees(List<ExcelEmployeesModel> excelEmployeesList, int companyId)
        {
            try
            {
                var newlyAddedEmployee = GetLatestDetailsOfUploadeEmployee(excelEmployeesList, companyId);
                var loginEmployeeList = newlyAddedEmployee.Where(x => x.RoleId != Enums.DimensionElements.TriggerAdmin.GetHashCode()
                                                             && x.RoleId != Enums.DimensionElements.NonManager.GetHashCode()
                                                             && x.Email != "");
                if (loginEmployeeList.Any())
                {
                    List<EmployeeModel> employeeDetails = loginEmployeeList.ToList();
                    var authUserLogin = AddAuthUserDetails(employeeDetails, companyId);
                    AddCompanyAdminDetails(excelEmployeesList, employeeDetails);
                    ScheduleToSendRegistrationmail(authUserLogin, employeeDetails, companyId);
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Schedule hangfire job for send registration mail based on company contract start date
        /// </summary>
        /// <param name="authUserLogin"></param>
        /// <param name="employeeDetails"></param>
        /// <param name="companyId"></param>
        private void ScheduleToSendRegistrationmail(DataTable authUserLogin, List<EmployeeModel> employeeDetails, int companyId)
        {
            var companyDetailsModels = _catalogDbContext.CompanyRepository.Select<List<CompanyDetailsModel>>(new CompanyDetailsModel { compId = companyId });
            if (companyDetailsModels != null && companyDetailsModels.Count > 0 && companyDetailsModels[0].contractStartDate.Date > DateTime.Now.Date)
            {
                foreach (DataRow dr in authUserLogin.Rows)
                {
                    int empid = employeeDetails.FirstOrDefault(x => x.Email == dr["email"].ToString()).EmpId;
                    SendRegistrationMail(dr["Id"].ToString(), empid, dr["Email"].ToString(), dr["SecurityStamp"].ToString(), companyId);
                }
            }
        }

        /// <summary>
        /// Update employee details which have change role 
        /// </summary>
        /// <param name="excelEmployeeList"></param>
        /// <param name="existingEmployeesList"></param>
        /// <param name="companyDetails"></param>
        private void UploadRoleChangedEmployees(List<ExcelEmployeesModel> excelEmployeeList, List<EmployeeModel> existingEmployeesList, CompanyDetailsModel companyDetails)
        {
            try
            {
                int companyId = Convert.ToInt32(companyDetails.companyId);
                var existDetailsOfUploadedEmployee = GetExistDetailsOfUploadedEmployee(excelEmployeeList, existingEmployeesList);
                if (existDetailsOfUploadedEmployee != null)
                {
                    UpdateUserLoginDetails(existDetailsOfUploadedEmployee, excelEmployeeList, companyDetails);
                }

                var roleChangeRecords = GetNewUsersForRoleChange(excelEmployeeList, existingEmployeesList);
                if (roleChangeRecords != null)
                {
                    UpdateAuthUserDetails(roleChangeRecords, companyId, true);  
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add/Update user login details in tenant or catalog
        /// Send employee registration mail based on company contract start date.
        /// </summary>
        /// <param name="existEmployeeList"></param>
        /// <param name="excelEmployeeList"></param>
        /// <param name="companyDetails"></param>
        private void UpdateUserLoginDetails(List<EmployeeModel> existEmployeeList, List<ExcelEmployeesModel> excelEmployeeList, CompanyDetailsModel companyDetails)
        {
            int companyId = Convert.ToInt32(companyDetails.companyId);
            foreach (EmployeeModel employee in existEmployeeList)
            {
                var newEmp = excelEmployeeList.FirstOrDefault(x => x.empId == employee.EmpId);
                employee.Email = newEmp.email;
                employee.RoleId = Convert.ToInt32(newEmp.roleId);
            }

            var authUserLogin = AddAuthUserDetails(existEmployeeList, companyId);
            if (companyDetails.contractStartDate.Date > DateTime.Now.Date)
            {
                foreach (DataRow dr in authUserLogin.Rows)
                {
                    int empid = existEmployeeList.FirstOrDefault(x => x.Email == dr["email"].ToString()).EmpId;
                    SendRegistrationMail(dr["Id"].ToString(), empid, dr["email"].ToString(), dr["SecurityStamp"].ToString(), companyId);
                }
            }
        }

        /// <summary>
        /// Add/Update userlogin details(tenant) and aspnetuser details(catalog)
        /// Add/Update claims details (catalog)
        /// </summary>
        /// <param name="employeeList"></param>
        /// <param name="companyId"></param>
        private DataTable AddAuthUserDetails(List<EmployeeModel> employeeList, int companyId)
        {
            var authUserLogin = AddAuthUserLogin(employeeList);
            var employeeDashboardModel = new EmployeeDashboardModel() { companyId = companyId };
            var tenantName = _catalogDbContext.EmployeeDashboardRepository.GetTenantNameByCompanyId(employeeDashboardModel);
            var authDetail = _catalogDbContext.AuthUserExcelRepository.GetListAuthUser();

            DataTable claimDetails = GetClaimDetails(GetAuthUserClaims(employeeList, authUserLogin, authDetail.ToList()), tenantName); // prepare claims details
            _catalogDbContext.AuthUserClaimExcelRepository.AddAuthClaims(new AuthUserClaimExcelModel { TblAuthClaims = claimDetails });
            return authUserLogin;
        }

        /// <summary>
        /// Get latest uplaoded employee details
        /// </summary>
        /// <param name="excelEmployeeList"></param>
        /// <param name="companyId"></param>
        /// <returns>employee List</returns>
        private List<EmployeeModel> GetLatestDetailsOfUploadeEmployee(List<ExcelEmployeesModel> excelEmployeeList, int companyId)
        {
            var employeeModel = new EmployeeModel { CompanyId = companyId };
            var employees = _connectionContext.TriggerContext.EmployeeRepository.GetExcelEmployees(employeeModel);
            List<string> newEmployeeEmails = excelEmployeeList.Select(x => x.email).ToList();
            return employees.Where(x => newEmployeeEmails.Contains(x.Email)).ToList();
        }

        /// <summary>
        /// Get email changed employee list
        /// </summary>
        /// <param name="excelEmployeeList"></param>
        /// <param name="existingEmployees"></param>
        /// <returns>employee List</returns>
        private List<EmployeeModel> GetEmailChangedEmployeesList(List<ExcelEmployeesModel> excelEmployeeList, List<EmployeeModel> existingEmployees)
        {
            List<EmployeeModel> mismatchData = null;

            var returnData = (from a in existingEmployees.AsEnumerable()
                              join b in excelEmployeeList.AsEnumerable()
                              on a.EmpId equals b.empId
                              where (a.Email != b.email)
                              && Convert.ToInt32(b.roleId) != Enums.DimensionElements.NonManager.GetHashCode()
                              && b.email != "" && a.Email != ""
                              select new EmployeeModel
                              {
                                  EmpId = b.empId,
                                  EmployeeId = b.employeeId,
                                  Email = b.email,
                                  EmpStatus = b.empStatus,
                                  UpdatedBy = b.updatedBy,
                                  PhoneNumber = b.phonenumber,
                                  RoleId = Convert.ToInt32(b.roleId),
                              });

            if (returnData.Any())
            {
                mismatchData = returnData.ToList();
            }
            return mismatchData;
        }

        /// <summary>
        /// Get employee list for send/sechdule registration mail 
        /// </summary>
        /// <param name="excelEmployeeList"></param>
        /// <param name="existingEmployees"></param>
        /// <returns>employee List</returns>
        private List<EmployeeModel> GetMismatchEmployeesForSentEmail(List<ExcelEmployeesModel> excelEmployeeList, List<EmployeeModel> existingEmployees)
        {
            List<EmployeeModel> employeeModels = null;
            var returnData = from a in existingEmployees
                             join b in excelEmployeeList
                             on a.EmpId equals b.empId
                             where (a.Email != b.email)
                             && Convert.ToInt32(b.roleId) != Enums.DimensionElements.NonManager.GetHashCode()
                             && a.RoleId != Enums.DimensionElements.NonManager.GetHashCode()
                             && b.email != "" && a.Email != ""
                             && b.empStatus
                             && a.IsMailSent
                             select new EmployeeModel
                             {
                                 EmpId = b.empId,
                                 EmployeeId = b.employeeId,
                                 Email = b.email,
                                 EmpStatus = b.empStatus,
                                 UpdatedBy = b.updatedBy,
                                 PhoneNumber = b.phonenumber,
                                 RoleId = Convert.ToInt32(b.roleId),
                             };

            if (returnData.Any())
            {
                employeeModels = returnData.ToList();
            }
            return employeeModels;
        }

        /// <summary>
        /// Get employee list which have role change
        /// </summary>
        /// <param name="excelEmployeeList"></param>
        /// <param name="existingEmployees"></param>
        /// <returns>employee List</returns>
        private List<EmployeeModel> GetNewUsersForRoleChange(List<ExcelEmployeesModel> excelEmployeeList, List<EmployeeModel> existingEmployees) 
        {
            List<EmployeeModel> userDetails = null;
            var returnData = from a in existingEmployees
                             join b in excelEmployeeList
                             on a.EmpId equals b.empId
                             where a.RoleId != Convert.ToInt32(b.roleId)
                             && Convert.ToInt32(b.roleId) > Enums.DimensionElements.TriggerAdmin.GetHashCode()
                             && a.RoleId != Enums.DimensionElements.NonManager.GetHashCode()
                             select new EmployeeModel
                             {
                                 EmpId = b.empId,
                                 EmployeeId = b.employeeId,
                                 Email = b.email,
                                 EmpStatus = b.empStatus,
                                 UpdatedBy = b.updatedBy,
                                 PhoneNumber = b.phonenumber,
                                 RoleId = Convert.ToInt32(b.roleId),
                             };
            if (returnData.Any())
            {
                userDetails = returnData.ToList();
            }
            return userDetails;
        }

        /// <summary>
        /// Get exist details of uploaded employee
        /// </summary>
        /// <param name="excelEmployeeList"></param>
        /// <param name="existingEmployees"></param>
        /// <returns>employee List</returns>
        private List<EmployeeModel> GetExistDetailsOfUploadedEmployee(List<ExcelEmployeesModel> excelEmployeeList, List<EmployeeModel> existingEmployees)
        {
            List<EmployeeModel> userDetails = null;
            var returnData = from a in existingEmployees.AsEnumerable()
                             join b in excelEmployeeList.AsEnumerable()
                             on a.EmpId equals b.empId
                             where a.RoleId != Convert.ToInt32(b.roleId)
                             && Convert.ToInt32(b.roleId) > Enums.DimensionElements.TriggerAdmin.GetHashCode()
                             && a.RoleId == Enums.DimensionElements.NonManager.GetHashCode()
                             select a;
            if (returnData.Any())
            {
                userDetails = returnData.ToList();
            }
            return userDetails;
        }

        /// <summary>
        /// Get employee list which have employee status change
        /// </summary>
        /// <param name="excelEmployeeList"></param>
        /// <param name="existingEmployees"></param>
        /// <returns>excel employee List</returns>
        private List<ExcelEmployeesModel> GetStatusphnChangedNewUsers(List<ExcelEmployeesModel> excelEmployeeList, List<EmployeeModel> existingEmployees)
        {
            List<ExcelEmployeesModel> userDetails = null;

            var returnData = (from a in existingEmployees
                              join b in excelEmployeeList
                              on a.EmpId equals b.empId
                              where (a.EmpStatus != b.empStatus || a.PhoneNumber != b.phonenumber)
                              && Convert.ToInt32(b.roleId) != Enums.DimensionElements.NonManager.GetHashCode()
                              select b);
            if (returnData.Any())
            {
                userDetails = returnData.ToList();
            }
            return userDetails;
        }

        /// <summary>
        /// Add User login details in tenant and aspnetusers detail in catalog
        /// </summary>
        /// <param name="employeeList"></param>
        /// <returns></returns>
        private DataTable AddAuthUserLogin(List<EmployeeModel> employeeList)
        {
            _connectionContext.TriggerContext.UserExcelRepository.AddUserLogin(new UserExcelModel { TblUserLogin = GetUserLoginDetails(employeeList) });
            var authUserLogin = GetAuthUserLogin(employeeList);
            _catalogDbContext.AuthUserExcelRepository.AddAuthLogin(new AuthUserExcelModel() { TblAspNetUsers = authUserLogin });
            return authUserLogin;
        }

        /// <summary>
        /// Get user login details in datatable form
        /// </summary>
        /// <param name="employeeList"></param>
        /// <returns>Datatable</returns>
        private DataTable GetUserLoginDetails(List<EmployeeModel> employeeList)
        {
            var userLogin = from users in employeeList
                            select new
                            {
                                username = users.Email,
                                password = "",
                                empid = users.EmpId,
                                bactive = users.EmpStatus,
                                createdby = users.CreatedBy,
                                updateby = users.UpdatedBy
                            };

            return ConvertToDataTable.ToDataTable(userLogin.ToList());
        }

        /// <summary>
        /// Use to get authorized User details in datatble form 
        /// </summary>
        /// <param name="employeeList"></param>
        /// <returns></returns>
        private DataTable GetAuthUserLogin(List<EmployeeModel> employeeList)
        {
            var authUserLogin = from authUsers in employeeList
                                select new
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    ConcurrencyStamp = Guid.NewGuid().ToString(Enums.GuidType.B.ToString()),
                                    Email = authUsers.Email,
                                    EmailConfirmed = authUsers.EmpStatus,
                                    LockoutEnabled = false,
                                    NormalizedEmail = authUsers.Email.ToUpper(),
                                    NormalizedUserName = authUsers.Email.ToUpper(),
                                    PasswordHash = "",
                                    PhoneNumber = authUsers.PhoneNumber,
                                    PhoneNumberConfirmed = false,
                                    SecurityStamp = Guid.NewGuid().ToString(Enums.GuidType.D.ToString()),
                                    TwoFactorEnabled = false,
                                    UserName = authUsers.Email.ToUpper(),
                                    AccessFailedCount = 0
                                };

            return ConvertToDataTable.ToDataTable(authUserLogin.ToList());
        }

        /// <summary>
        /// Use to get claim details (compId,RoleId,EmpId,connectionstring)
        /// </summary>
        /// <param name="authUserClaim"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private DataTable GetClaimDetails(DataTable authUserClaim, string connectionString)
        {
            List<Claims> claims = new List<Claims>();
            for (int i = 0; i < authUserClaim.Rows.Count; i++)
            {
                claims.Add(GenerateClaimDetails(Enums.ClaimType.CompId.ToString(), Convert.ToString(authUserClaim.Rows[i]["CompanyId"]), Convert.ToString(authUserClaim.Rows[i]["UserId"])));
                claims.Add(GenerateClaimDetails(Enums.ClaimType.RoleId.ToString(), Convert.ToString(authUserClaim.Rows[i]["RoleId"]), Convert.ToString(authUserClaim.Rows[i]["UserId"])));
                claims.Add(GenerateClaimDetails(Enums.ClaimType.EmpId.ToString(), Convert.ToString(authUserClaim.Rows[i]["EmpId"]), Convert.ToString(authUserClaim.Rows[i]["UserId"])));
                claims.Add(GenerateClaimDetails(Enums.ClaimType.Key.ToString(), connectionString, Convert.ToString(authUserClaim.Rows[i]["UserId"])));
            }
            return ConvertToDataTable.ToDataTable(claims);
        }

        /// <summary>
        /// Use to generate claim details
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="value"></param>
        /// <param name="authUserId"></param>
        /// <returns></returns>
        private static Claims GenerateClaimDetails(string keyType, string value, string authUserId)
        {
            return new Claims
            {
                Id = Messages.id,
                AuthUserId = authUserId,
                ClaimType = keyType,
                ClaimValue = value
            };
        }

        /// <summary>
        /// Use to get Authorized user claims
        /// </summary>
        /// <param name="employeeList"></param>
        /// <param name="authUserLogin"></param>
        /// <param name="authDetails"></param>
        /// <returns></returns>
        private static DataTable GetAuthUserClaims(List<EmployeeModel> employeeList, DataTable authUserLogin, List<AuthUserDetails> authDetails)
        {
            var authUserClaims = from users in employeeList
                                 join aspnetUsers in authUserLogin.AsEnumerable()
                                     on users.Email equals aspnetUsers["email"].ToString()
                                 join authExists in authDetails.AsEnumerable()
                                     on users.Email equals authExists.Email
                                 select new
                                 {
                                     Id = (authExists.Id != string.Empty) ? authExists.Id : aspnetUsers.Field<string>("Id").ToString(),
                                     UserId = (authExists.Id != string.Empty) ? authExists.Id : aspnetUsers.Field<string>("Id").ToString(),
                                     CompanyID = users.CompanyId,
                                     RoleID = users.RoleId,
                                     EmpID = users.EmpId
                                 };
            return ConvertToDataTable.ToDataTable(authUserClaims.ToList());
        }

        /// <summary>
        /// Schedule hangfire event for send registration mail to employee on company contract start date
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="emapId"></param>
        /// <param name="email"></param>
        /// <param name="securityStamp"></param>
        /// <param name="compId"></param>
        private void SendRegistrationMail(string Id, int emapId, string email, string securityStamp, int compId)
        {
            try
            {
                var code = GenerateToken(securityStamp);
                var callbackUrl = _appSettings.AuthUrl + Messages.confirmEmailPath + Id + Messages.code + code;
                EmployeeModel employeeModel = new EmployeeModel { EmpId = emapId, Email = email, CompanyId = compId };
                _backGroundJobRequest.SendEmail(employeeModel, callbackUrl);
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Use to generate token
        /// </summary>
        /// <param name="securityStamp"></param>
        /// <returns></returns>
        public string GenerateToken(string securityStamp)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(securityStamp));
        }

        /// <summary>
        /// Use to add company admin details
        /// </summary>
        /// <param name="excelEmployeesList"></param>
        /// <param name="employeeList"></param>
        private void AddCompanyAdminDetails(List<ExcelEmployeesModel> excelEmployeesList, List<EmployeeModel> employeeList)        
        {
            var companyAdmin = excelEmployeesList.Where(x => x.roleId == Enums.DimensionElements.CompanyAdmin.GetHashCode().ToString());
            if (companyAdmin.Any())
            {
                foreach (ExcelEmployeesModel admin in companyAdmin.Where(x => x.empId == 0)) 
                {
                    admin.empId = employeeList.FirstOrDefault(x => x.Email == admin.email).EmpId;
                    admin.employeeId = employeeList.FirstOrDefault(x => x.Email == admin.email).EmployeeId;
                }

                EmployeeExcelModel excelEmployee = new EmployeeExcelModel { TblEmployee = ConvertToDataTable.ToDataTable(companyAdmin.ToList()) };
                _catalogDbContext.EmployeeExcelRepository.AddCompanyAdminDetails(excelEmployee);
            }
        }
    }
}
