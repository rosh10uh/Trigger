using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
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

        /// <summary>
        /// Use to get service of IConnectionContext
        /// </summary>
        private readonly IConnectionContext _connectionContext;

        /// <summary>
        /// Use to get service of CatalogDbContext
        /// </summary>
        private readonly TriggerCatalogContext _catalogDbContext;

        private readonly string _authUrl;
      
        private readonly BackgroundJobRequest _backGroundJobRequest;
        private readonly EmployeeContext _employeeContext;

        /// <summary>
        /// Initializes a new instance of the roles class using IDaoContextFactory and Connection.
        /// </summary>
        /// <param name="iLogger"></param>
        /// <param name="connectionContext"></param>
        /// <param name="triggerContext"></param>
        /// <param name="catalogDbContext"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="iPasswordGenerator"></param>
        public ExcelUploadContext(ILogger<ExcelUploadContext> iLogger, IConnectionContext connectionContext, TriggerContext triggerContext,  TriggerCatalogContext catalogDbContext,
            IHostingEnvironment hostingEnvironment,  BackgroundJobRequest backGroundJobRequest, EmployeeContext employeeContext)
        {
            _iLogger = iLogger;
            _connectionContext = connectionContext;
            _catalogDbContext = catalogDbContext;
            _authUrl = Shared.Dictionary.ConfigDictionary[Shared.Dictionary.ConfigurationKeys.AuthUrl.ToString()] + Messages.confirmEmailPath;
            _backGroundJobRequest = backGroundJobRequest;
            _employeeContext = employeeContext;

        }

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
                _iLogger.LogError(ex.Message.ToString());
            }
        }

        private Trigger.DTO.UserDetails GetExistingUserDetails(int companyId, int empId)
        {
            UserDetails userDetails = new UserDetails() { CompId = companyId, existingEmpId = empId };
            return _connectionContext.TriggerContext.UserDetailRepository.GetUserDetails(userDetails);
        }

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
                _iLogger.LogError(ex.Message.ToString());
            }
        }

        private void UpdateAuthUserDetails(DataTable commonRecords, int companyId, bool isClaims)
        {
            foreach (DataRow dr in commonRecords.Rows)
            {
                EmployeeModel authEmployee = new EmployeeModel
                {
                    empId = Convert.ToInt32(dr["empid"]),
                    email = dr["email"].ToString(),
                    companyid = companyId,
                    empStatus = Convert.ToBoolean(dr["empStatus"]),
                    updatedBy = Convert.ToInt32(dr["updatedBy"]),
                    phoneNumber = Convert.ToString(dr["phonenumber"])
                };

                Trigger.DTO.UserDetails existingUserDetails = GetExistingUserDetails(companyId, Convert.ToInt32(dr["empid"]));

                if (Convert.ToInt32(dr["roleid"]) != EmployeeModel.Emprole.Admin.GetHashCode())
                {
                    _backGroundJobRequest.DeleteCompanyAdmin(authEmployee, null, _employeeContext);
                }

                AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = existingUserDetails.userName };
                authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
                if (Convert.ToInt32(dr["roleid"]) == EmployeeModel.Emprole.NonManager.GetHashCode())
                {
                    _connectionContext.TriggerContext.EmployeeRepository.DeleteUser(authEmployee);
                    _employeeContext.DeleteAuthUser(authUserDetails);
                }
                else
                {
                    UpdateUser(authEmployee);
                    UpdateAuthUser(authEmployee, authUserDetails.Id);
                    if (isClaims)
                    {
                        UpdateAuthUserClaim(Convert.ToString(dr["roleid"]), authUserDetails.Id);
                    }
                }
            }
        }

        /// <summary>
        /// add employee & user to database 
        /// </summary>
        /// <param name="countRecordModel">list of employee </param>
        /// <returns> result </returns>
        public int AddNewDataToEmpData(CountRecordModel countRecordModel)
        {
            int result = 0;
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();

                var companyId = Convert.ToInt32(countRecordModel.lstNewCsvUpload.Select(x => x.companyId).FirstOrDefault());
                var employeeDTO = _connectionContext.TriggerContext.EmployeeRepository.GetCsvEmployees(new EmployeeModel { companyid = companyId });
                DataTable existingEmployees = ToDataTable(employeeDTO);

                var newEmployee = ToDataTable(countRecordModel.lstNewCsvUpload);
                var emp = _connectionContext.TriggerContext.EmployeeCsvRepository.AddEmpolyee(new EmployeeCsvModel { tblEmployee = newEmployee });
                result = emp.result;

                if (result > 0)
                {
                    newEmployee.DefaultView.RowFilter = Messages.empIdFilter;
                    DataTable mistMatchData = newEmployee.DefaultView.ToTable();
                    if (mistMatchData.Rows.Count > 0)
                    {
                        var companyDetailsModel = _catalogDbContext.CompanyRepository.Select<CompanyDetailsModel>(new CompanyDetailsModel { compId = companyId });
                        UploadMismatchNewUsers(newEmployee, existingEmployees, companyDetailsModel, companyId);// same empid with changed roleid

                        var commonRecords = GetMismatchEmployees(newEmployee, existingEmployees, false); // For email id change
                        if (commonRecords != null)
                        {
                            UpdateAuthUserDetails(commonRecords, companyId, false);
                        }

                        var statusPhoneNumberChange = GetNewUsersStatusPhnChanged(countRecordModel.lstNewCsvUpload, employeeDTO);
                        if (statusPhoneNumberChange != null)
                        {
                            updateAuthUserEmlPhnConfirm(statusPhoneNumberChange);
                            updateStatusPhnCompanyAdmin(newEmployee, companyId);
                        }

                        commonRecords = GetMismatchEmployees(newEmployee, existingEmployees, true);

                        if (companyDetailsModel != null && commonRecords != null)
                        {

                            if (companyDetailsModel.contractStartDate <= DateTime.Now.Date && companyDetailsModel.contractEndDate.Date.AddDays(companyDetailsModel.gracePeriod) >= DateTime.Now.Date)
                            {
                                _backGroundJobRequest.SendEmailToUpdatedUser(commonRecords, companyId);
                            }
                            else
                            {
                                if (companyDetailsModel.contractStartDate.Date > DateTime.Now.Date)
                                {
                                    var authUserLogin = GetAuthCommonUsers(commonRecords);
                                    authUserLogin.Columns.Add("empid", typeof(int));
                                    foreach (DataRow dr in authUserLogin.Rows)
                                    {
                                        dr["empid"] = commonRecords.Select("email='" + dr["email"].ToString() + "'")[0]["empid"];
                                    }
                                    SendRegistrationMail(authUserLogin, companyDetailsModel.compId);
                                }
                            }
                        }
                    }
                    else
                    {
                        newEmployee.DefaultView.RowFilter = "";
                        UploadEmployees(newEmployee, companyId);
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

        private void updateStatusPhnCompanyAdmin(DataTable newEmployee, int companyId)
        {
            var commonRecords = GetCommonRecords(newEmployee, companyId);
            commonRecords.DefaultView.RowFilter = Messages.dbMessage;
            commonRecords = commonRecords.DefaultView.ToTable();
            AddCompanyAdminDetails(newEmployee, commonRecords);
        }

        private DataTable GetAuthCommonUsers(DataTable employeeModels)
        {

            List<AuthUserDetails> authUserDetails = new List<AuthUserDetails>();
            foreach (DataRow authDetails in employeeModels.Rows)
            {
                AuthUserDetails authUsersMail = new AuthUserDetails() { Email = Convert.ToString(authDetails["email"]) };
                authUsersMail = _catalogDbContext.AuthUserDetailsRepository.GetAuthDetailsByEmail(authUsersMail);

                authUserDetails.Add(authUsersMail);
            }

            return ConvertToDataTable.ToDataTable(authUserDetails);


        }

        private void UploadEmployees(DataTable newEmployee, int companyId)
        {
            var commonRecords = GetCommonRecords(newEmployee, companyId);
            commonRecords.DefaultView.RowFilter = Messages.dbMessage;
            commonRecords = commonRecords.DefaultView.ToTable();
            if (commonRecords.Rows.Count > 0)
            {
                UploadNewEmployees(newEmployee, commonRecords.DefaultView.ToTable(), companyId);
            }
        }

        private void UploadNewEmployees(DataTable newEmployee, DataTable commonRecords, int companyId)
        {
            var authUserLogin = AddAuthUserDetails(commonRecords, companyId);
            AddCompanyAdminDetails(newEmployee, commonRecords);
            var companyDetailsModels = _catalogDbContext.CompanyRepository.Select<List<CompanyDetailsModel>>(new CompanyDetailsModel { compId = companyId });

            authUserLogin.Columns.Add("empid", typeof(int));
            foreach (DataRow dr in authUserLogin.Rows)
            {
                dr["empid"] = commonRecords.Select("email='" + dr["email"].ToString() + "'")[0]["empid"];
            }
            if (companyDetailsModels != null && companyDetailsModels.Count > 0 && companyDetailsModels[0].contractStartDate.Date > DateTime.Now.Date)
            {
                SendRegistrationMail(authUserLogin, companyId);
            }
        }

        private void UploadMismatchNewUsers(DataTable newEmployee, DataTable existingEmployees, CompanyDetailsModel companyDetailsModels, int companyId)
        {
            var commonRecords = GetNewUsers(newEmployee, existingEmployees, false);
            if (commonRecords != null)
            {
                foreach (DataRow emails in commonRecords.Rows)
                {
                    emails["email"] = newEmployee.Select("empid=" + emails["empid"].ToString()).FirstOrDefault()["email"].ToString();
                    emails["roleid"] = newEmployee.Select("empid=" + emails["empid"].ToString()).FirstOrDefault()["roleid"].ToString();
                }

                var authUserLogin = AddAuthUserDetails(commonRecords, companyId);
                AddCompanyAdminDetails(newEmployee, commonRecords);

                if (companyDetailsModels != null && companyDetailsModels.contractStartDate.Date > DateTime.Now.Date)
                {
                    authUserLogin.Columns.Add("empid", typeof(int));
                    foreach (DataRow dr in authUserLogin.Rows)
                    {
                        dr["empid"] = commonRecords.Select("email='" + dr["email"].ToString() + "'")[0]["empid"];
                    }
                    SendRegistrationMail(authUserLogin, companyId);
                }
            }

            var roleChangeRecords = GetNewUsers(newEmployee, existingEmployees, true);
            if (roleChangeRecords != null)
            {
                AddCompanyAdminDetails(newEmployee, roleChangeRecords);
                UpdateAuthUserDetails(roleChangeRecords, companyId, true);
            }
        }

        private DataTable AddAuthUserDetails(DataTable commonRecords, int companyId)
        {
            var authUserLogin = AddAuthUserLogin(commonRecords);
            var employeeDashboardModel = new EmployeeDashboardModel() { companyId = companyId };
            var tenantName = _catalogDbContext.EmployeeDashboardRepository.GetTenantNameByCompanyId(employeeDashboardModel);
            var authDetail = _catalogDbContext.AuthUserCsvRepository.GetListAuthUser();
            _catalogDbContext.AuthUserClaimCsvRepository.AddAuthClaims(new AuthUserClaimCsvModel
            {
                tblAuthClaims =
                    GetClaimDetails(
                        GetAuthUserClaims(commonRecords, authUserLogin,
                            ToDataTable(authDetail.ToList())), tenantName)
            });

            return authUserLogin;
        }

        private void updateAuthUserEmlPhnConfirm(List<CsvEmployeesModel> newEmployee)
        {
            foreach (var emp in newEmployee)
            {
                EmployeeModel authEmployee = new EmployeeModel { email = emp.email, empStatus = emp.empStatus, phoneNumber = emp.phonenumber };
                AuthUserDetails authUserDetails = new AuthUserDetails() { ExistingEmail = emp.email };
                authUserDetails = _catalogDbContext.AuthUserDetailsRepository.GetSubIdByEmail(authUserDetails);
                UpdateAuthUser(authEmployee, authUserDetails.Id);
            }
        }

        /// <summary>
        /// convert list to datatable 
        /// </summary>
        /// <param name="items">list of items</param>
        /// <returns> DataTable </returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        /// <summary>
        /// Use to get common record
        /// </summary>
        /// <param name="newRecords"></param>
        /// <param name="companyId"></param>
        /// <returns>DataTable</returns>
        private DataTable GetCommonRecords(DataTable newRecords, int companyId)
        {
            var employeeModel = new EmployeeModel { companyid = companyId };
            var employees = _connectionContext.TriggerContext.EmployeeRepository.GetCsvEmployees(employeeModel);

            var dtbEmpData = ToDataTable(employees);

            return (from a in dtbEmpData.AsEnumerable()
                    join b in newRecords.AsEnumerable()
                    on a["email"].ToString() equals b["email"].ToString()
                    into g
                    where g.Any()
                    select a).CopyToDataTable();

        }

        private DataTable GetMismatchEmployees(DataTable newRecords, DataTable existingEmployees, bool forMail)
        {
            DataTable mismatchData = null;
            if (forMail)
            {
                var returnData = (from a in existingEmployees.AsEnumerable()
                                  join b in newRecords.AsEnumerable()
                                  on a["empid"].ToString() equals b["empid"].ToString()
                                  where (a["email"].ToString() != b["email"].ToString())
                                  && Convert.ToInt32(b["roleid"]) != Enums.DimensionElements.NonManager.GetHashCode()
                                  && Convert.ToInt32(a["roleid"]) != Enums.DimensionElements.NonManager.GetHashCode()
                                  && b["email"].ToString() != "" && a["email"].ToString() != ""
                                  && Convert.ToInt32(b["empstatus"]) == 1
                                  && Convert.ToInt32(a["isMailSent"]) == 1
                                  select b);

                if (returnData.Any())
                {
                    mismatchData = returnData.CopyToDataTable();
                }
            }
            else
            {
                var returnData = (from a in existingEmployees.AsEnumerable()
                                  join b in newRecords.AsEnumerable()
                                  on a["empid"].ToString() equals b["empid"].ToString()
                                  where (a["email"].ToString() != b["email"].ToString())
                                  && Convert.ToInt32(b["roleid"]) != Enums.DimensionElements.NonManager.GetHashCode()
                                  && b["email"].ToString() != "" && a["email"].ToString() != ""
                                  select b);

                if (returnData.Any())
                {
                    mismatchData = returnData.CopyToDataTable();
                }
            }

            return mismatchData;
        }

        private DataTable GetNewUsers(DataTable newRecords, DataTable existingEmployees, bool userRoleChanged)
        {
            DataTable userDetails = null;
            if (userRoleChanged)
            {
                var returnData = (from a in existingEmployees.AsEnumerable()
                                  join b in newRecords.AsEnumerable()
                                  on a["empid"].ToString() equals b["empid"].ToString()
                                  where Convert.ToInt32(a["roleid"]) != Convert.ToInt32(b["roleid"])
                                  && Convert.ToInt32(b["roleid"]) > Enums.DimensionElements.TriggerAdmin.GetHashCode()
                                  && Convert.ToInt32(a["roleid"]) != Enums.DimensionElements.NonManager.GetHashCode()
                                  select b);
                if (returnData.Any())
                {
                    userDetails = returnData.CopyToDataTable();
                }
            }
            else
            {
                var returnData = (from a in existingEmployees.AsEnumerable()
                                  join b in newRecords.AsEnumerable()
                                  on a["empid"].ToString() equals b["empid"].ToString()
                                  where Convert.ToInt32(a["roleid"]) != Convert.ToInt32(b["roleid"])
                                  && Convert.ToInt32(b["roleid"]) > Enums.DimensionElements.TriggerAdmin.GetHashCode()
                                  && Convert.ToInt32(a["roleid"]) == Enums.DimensionElements.NonManager.GetHashCode()
                                  select a);
                if (returnData.Any())
                {
                    userDetails = returnData.CopyToDataTable();
                }
            }

            return userDetails;
        }

        private List<CsvEmployeesModel> GetNewUsersStatusPhnChanged(List<CsvEmployeesModel> newRecords, List<EmployeeModel> existingEmployees)
        {
            List<CsvEmployeesModel> userDetails = null;

            var returnData = (from a in existingEmployees
                              join b in newRecords
                              on a.empId equals b.empId
                              where (a.empStatus != b.empStatus
                              || a.phoneNumber != b.phonenumber)
                              && Convert.ToInt32(b.roleId) != Enums.DimensionElements.NonManager.GetHashCode()
                              select b);
            if (returnData.Any())
            {
                userDetails = returnData.ToList();
            }
            return userDetails;
        }

        /// <summary>
        /// Use to add Auth user login
        /// </summary>
        /// <param name="commonRecords"></param>
        /// <returns></returns>
        private DataTable AddAuthUserLogin(DataTable commonRecords)
        {
            _connectionContext.TriggerContext.UserCsvRepository.AddUserLogin(new UserCsvModel { tblUserLogin = GetUserLoginDetails(commonRecords) });
            var authUserLogin = GetAuthUserLogin(commonRecords);

            _catalogDbContext.AuthUserCsvRepository.AddAuthLogin(new AuthUserCsvModel() { tblAspNetUsers = authUserLogin });

            return authUserLogin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commonRecords"></param>
        /// <returns></returns>
        private static DataTable GetUserLoginDetails(DataTable commonRecords)
        {
            var userLogin = from users in commonRecords.AsEnumerable()
                            select new
                            {
                                userName = users.Field<string>("email"),
                                password = "",
                                empId = users.Field<int>("empId"),
                                bactive = users.Field<bool>("empStatus"),
                                createdBy = users.Field<int>("createdby"),
                                updateby = users.Field<int>("updatedby")

                            };

            return ToDataTable(userLogin.ToList());
        }

        /// <summary>
        /// Use to get authorized User login
        /// </summary>
        /// <param name="commonRecords"></param>
        /// <returns></returns>
        private DataTable GetAuthUserLogin(DataTable commonRecords)
        {
            var authUserLogin = from authUsers in commonRecords.AsEnumerable()
                                select new
                                {

                                    Id = Guid.NewGuid().ToString(),
                                    ConcurrencyStamp = Guid.NewGuid().ToString(Enums.Guid.B.ToString()),
                                    Email = authUsers.Field<string>("email"),
                                    EmailConfirmed = authUsers.Field<Boolean>("empStatus"),
                                    LockoutEnabled = false,
                                    NormalizedEmail = authUsers.Field<string>("email").ToUpper(),
                                    NormalizedUserName = authUsers.Field<string>("email").ToUpper(),
                                    PasswordHash ="",
                                    PhoneNumber = authUsers.Field<string>("phonenumber"),
                                    PhoneNumberConfirmed = false,
                                    SecurityStamp = Guid.NewGuid().ToString(Enums.Guid.D.ToString()),
                                    TwoFactorEnabled = false,
                                    UserName = authUsers.Field<string>("email").ToUpper(),
                                    AccessFailedCount = 0

                                };

            return ToDataTable(authUserLogin.ToList());
        }

        /// <summary>
        /// Use to get claim details
        /// </summary>
        /// <param name="authUserClaim"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private DataTable GetClaimDetails(DataTable authUserClaim, string connectionString)
        {
            List<Trigger.DTO.Claims> claims = new List<Trigger.DTO.Claims>();
            for (int i = 0; i < authUserClaim.Rows.Count; i++)
            {
                claims.Add(GenerateClaimDetails(Enums.ClaimType.CompId.ToString(), Convert.ToString(authUserClaim.Rows[i]["CompanyId"]), Convert.ToString(authUserClaim.Rows[i]["UserId"])));
                claims.Add(GenerateClaimDetails(Enums.ClaimType.RoleId.ToString(), Convert.ToString(authUserClaim.Rows[i]["RoleId"]), Convert.ToString(authUserClaim.Rows[i]["UserId"])));
                claims.Add(GenerateClaimDetails(Enums.ClaimType.EmpId.ToString(), Convert.ToString(authUserClaim.Rows[i]["EmpId"]), Convert.ToString(authUserClaim.Rows[i]["UserId"])));
                claims.Add(GenerateClaimDetails(Enums.ClaimType.Key.ToString(), connectionString, Convert.ToString(authUserClaim.Rows[i]["UserId"])));
            }

            return ToDataTable(claims);
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
            return new Trigger.DTO.Claims
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
        /// <param name="commonRecords"></param>
        /// <param name="authUserLogin"></param>
        /// <param name="authDetails"></param>
        /// <returns></returns>
        private static DataTable GetAuthUserClaims(DataTable commonRecords, DataTable authUserLogin, DataTable authDetails)
        {
            var authUserClaims = from users in commonRecords.AsEnumerable()
                                 join aspnetUsers in authUserLogin.AsEnumerable()
                                     on users["email"].ToString() equals aspnetUsers["email"].ToString()
                                 join authExists in authDetails.AsEnumerable()
                                     on users["email"].ToString() equals authExists["email"].ToString()
                                 select new
                                 {
                                     Id = (authExists.Field<string>("Id").ToString() != string.Empty) ? authExists.Field<string>("Id").ToString() : aspnetUsers.Field<string>("Id").ToString(),
                                     UserId = (authExists.Field<string>("Id").ToString() != string.Empty) ? authExists.Field<string>("Id").ToString() : aspnetUsers.Field<string>("Id").ToString(),
                                     CompanyID = users.Field<int>("CompanyId"),
                                     RoleID = users.Field<int>("RoleId"),
                                     EmpID = users.Field<int>("EmpId"),

                                 };
            return ToDataTable(authUserClaims.ToList());
        }

        /// <summary>
        /// Use to send registration mail
        /// </summary>
        /// <param name="dtbAuthUserLogin"></param>
        //[AutomaticRetry(Attempts = 0)]
        public void SendRegistrationMail(DataTable dtbAuthUserLogin, int compId)
        {
            foreach (DataRow drwEmail in dtbAuthUserLogin.DefaultView.ToTable().Rows)
            {
                try
                {
                    var code = GenerateToken(drwEmail["SecurityStamp"].ToString());
                    var callbackUrl = _authUrl + drwEmail["Id"] + Messages.code + code;
                    EmployeeModel employeeModel = new EmployeeModel { empId = Convert.ToInt32(drwEmail["empid"]), email = drwEmail["Email"].ToString(), companyid = compId };
                    _backGroundJobRequest.SendEmail(employeeModel, callbackUrl, null);

                }
                catch (Exception ex)
                {
                    _iLogger.LogError(ex.Message);
                    throw;
                }
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
        /// <param name="companyAdmin"></param>
        /// <param name="commonRecords"></param>
        private void AddCompanyAdminDetails(DataTable companyAdmin, DataTable commonRecords)
        {
            companyAdmin.DefaultView.RowFilter = Messages.roleid;
            DataTable dtCompanyAdmin = companyAdmin.DefaultView.ToTable();

            if (dtCompanyAdmin.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCompanyAdmin.Select("empid=0"))
                {
                    dr["empid"] = commonRecords.Select("email='" + dr["email"].ToString() + "'")[0]["empid"];
                    dr["employeeid"] = commonRecords.Select("email='" + dr["email"].ToString() + "'")[0]["employeeid"];
                }
                _backGroundJobRequest.AddCompanyAdminFromCsv(dtCompanyAdmin, null);

            }
        }

        
        /// <summary>
        /// Use to execute non query with data table
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQueryWithDataTable(string connectionString, string commandText, SqlParameter commandParameters)
        {
            using (var sqlCommand = new SqlCommand())
            {
                var sqlConnection = new SqlConnection(connectionString);
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = commandText;
                commandParameters.SqlDbType = SqlDbType.Structured;
                sqlCommand.Parameters.AddWithValue(commandParameters.ParameterName, commandParameters.Value);
                var value = sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();
                return value;
            }
        }
    }
}
