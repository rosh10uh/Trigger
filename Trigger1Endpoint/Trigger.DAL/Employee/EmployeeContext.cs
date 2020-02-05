using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;


namespace Trigger.DAL.Employee
{
    /// <summary>
    /// Contains method to manage multiple operation.
    /// </summary>

    public class EmployeeContext
    {
        private readonly TriggerCatalogContext _catalogDbContext;

        private readonly ILogger _iLogger;
        private readonly IConnectionContext _connectionContext;
        public EmployeeContext(ILogger<EmployeeContext> iLogger, IConnectionContext connectionContext, TriggerCatalogContext catalogDbContext)
        {
            _iLogger = iLogger;
            _catalogDbContext = catalogDbContext;
            _connectionContext = connectionContext;
        }

        public int InsertEmployee(EmployeeModel employee)
        {
            var emp = _connectionContext.TriggerContext.EmployeeRepository.Insert(employee);
            return emp.resultId;
        }
        [AutomaticRetry(Attempts = 0)]
        public int InsertCompanyAdmin(EmployeeModel employeeAdmin, string connString)
        {
            SqlConnection sqlConnection = new SqlConnection(connString);

            try
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(Messages.uspAddCompanyAdminDetails, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(SetSqlParameter("@EmpId", ParameterDirection.Input, SqlDbType.Int, employeeAdmin.empId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@EmployeeId", ParameterDirection.Input, SqlDbType.VarChar, employeeAdmin.employeeId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@CompanyId", ParameterDirection.Input, SqlDbType.Int, employeeAdmin.companyid));
                    sqlCommand.Parameters.Add(SetSqlParameter("@FirstName", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.firstName));
                    sqlCommand.Parameters.Add(SetSqlParameter("@MiddleName", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.middleName));
                    sqlCommand.Parameters.Add(SetSqlParameter("@LastName", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.lastName));
                    sqlCommand.Parameters.Add(SetSqlParameter("@Suffix", ParameterDirection.Input, SqlDbType.VarChar, employeeAdmin.suffix));
                    sqlCommand.Parameters.Add(SetSqlParameter("@Email", ParameterDirection.Input, SqlDbType.VarChar, employeeAdmin.email));
                    sqlCommand.Parameters.Add(SetSqlParameter("@JobTitle", ParameterDirection.Input, SqlDbType.VarChar, employeeAdmin.jobTitle));
                    sqlCommand.Parameters.Add(SetSqlParameter("@JoiningDate", ParameterDirection.Input, SqlDbType.Date, employeeAdmin.joiningDate));
                    sqlCommand.Parameters.Add(SetSqlParameter("@WorkCity", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.workCity));
                    sqlCommand.Parameters.Add(SetSqlParameter("@WorkState", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.workState));
                    sqlCommand.Parameters.Add(SetSqlParameter("@WorkZipcode", ParameterDirection.Input, SqlDbType.VarChar, employeeAdmin.workZipcode));
                    sqlCommand.Parameters.Add(SetSqlParameter("@DepartmentId", ParameterDirection.Input, SqlDbType.SmallInt, employeeAdmin.departmentId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@ManagerId", ParameterDirection.Input, SqlDbType.Int, employeeAdmin.managerId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@ManagerFName", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.managerFName));
                    sqlCommand.Parameters.Add(SetSqlParameter("@ManagerLName", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.managerLName));
                    sqlCommand.Parameters.Add(SetSqlParameter("@EmpStatus", ParameterDirection.Input, SqlDbType.Bit, employeeAdmin.empStatus));
                    sqlCommand.Parameters.Add(SetSqlParameter("@RoleId", ParameterDirection.Input, SqlDbType.TinyInt, employeeAdmin.roleId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@DateOfBirth", ParameterDirection.Input, SqlDbType.Date, employeeAdmin.dateOfBirth));
                    sqlCommand.Parameters.Add(SetSqlParameter("@RaceOrEthanicityId", ParameterDirection.Input, SqlDbType.SmallInt, employeeAdmin.raceorethanicityId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@JobGroupId", ParameterDirection.Input, SqlDbType.SmallInt, employeeAdmin.jobGroupId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@JobGroup", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.jobGroup));
                    sqlCommand.Parameters.Add(SetSqlParameter("@Gender", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.gender));
                    sqlCommand.Parameters.Add(SetSqlParameter("@JobCategoryId", ParameterDirection.Input, SqlDbType.SmallInt, employeeAdmin.jobCategoryId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@JobCategory", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.jobCategory));
                    sqlCommand.Parameters.Add(SetSqlParameter("@EmpLocation", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.empLocation));
                    sqlCommand.Parameters.Add(SetSqlParameter("@JobCodeId", ParameterDirection.Input, SqlDbType.SmallInt, employeeAdmin.jobCodeId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@JobCode", ParameterDirection.Input, SqlDbType.NVarChar, employeeAdmin.jobCode));
                    sqlCommand.Parameters.Add(SetSqlParameter("@LastPromoDate", ParameterDirection.Input, SqlDbType.Date, employeeAdmin.lastPromodate));
                    sqlCommand.Parameters.Add(SetSqlParameter("@CurrentSalary", ParameterDirection.Input, SqlDbType.Decimal, employeeAdmin.currentSalary));
                    sqlCommand.Parameters.Add(SetSqlParameter("@LastIncDate", ParameterDirection.Input, SqlDbType.Date, employeeAdmin.lastIncDate));
                    sqlCommand.Parameters.Add(SetSqlParameter("@CountryId", ParameterDirection.Input, SqlDbType.Int, employeeAdmin.countryId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@RegionId", ParameterDirection.Input, SqlDbType.Int, employeeAdmin.regionId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@EmpImgPath", ParameterDirection.Input, SqlDbType.VarChar, employeeAdmin.empImgPath));
                    sqlCommand.Parameters.Add(SetSqlParameter("@CreatedBy", ParameterDirection.Input, SqlDbType.Int, employeeAdmin.createdBy));
                    sqlCommand.Parameters.Add(SetSqlParameter("@PhoneNumber", ParameterDirection.Input, SqlDbType.VarChar, employeeAdmin.phoneNumber));
                    sqlCommand.Parameters.Add(SetSqlParameter("@ResultId", ParameterDirection.Output, SqlDbType.Int, employeeAdmin.resultId));

                    employeeAdmin.resultId = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return employeeAdmin.resultId;
        }

        private SqlParameter SetSqlParameter(string paramName, ParameterDirection parameterDirection, SqlDbType sqlDbType, object value)
        {
            SqlParameter sqlParameter = new SqlParameter(paramName, value)
            {
                Direction = parameterDirection,
                SqlDbType = sqlDbType
            };
            return sqlParameter;
        }
        [AutomaticRetry(Attempts = 0)]
        public int DeleteCompanyAdmin(Trigger.DTO.EmployeeModel employeeAdmin, string connString)
        {
            SqlConnection sqlConnection = new SqlConnection(connString);

            try
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(Messages.usp_DeleteCompanyAdminDetails, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0;
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(SetSqlParameter("@EmpId", ParameterDirection.Input, SqlDbType.Int, employeeAdmin.empId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@CompanyId", ParameterDirection.Input, SqlDbType.Int, employeeAdmin.companyid));
                    sqlCommand.Parameters.Add(SetSqlParameter("@Result", ParameterDirection.Output, SqlDbType.Int, 0));
                    employeeAdmin.resultId = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return employeeAdmin.resultId;
        }

        public int InsertUser(Trigger.DTO.UserDetails userDetails)
        {
            var emp = _connectionContext.TriggerContext.UserDetailRepository.AddUser(userDetails);
            return emp.result;
        }

        public Trigger.DTO.AuthUserDetails Insert1AuthUser(Trigger.DTO.AuthUserDetails authUserDetails)
        {
            return _catalogDbContext.AuthUserDetailsRepository.Insert(authUserDetails);
        }

        public List<Trigger.DTO.Claims> Insert1AuthClaims(List<Trigger.DTO.Claims> claims)
        {
            return _catalogDbContext.ClaimsRepository.Insert(claims);
        }

        public int UpdateEmployee(EmployeeModel employee)
        {
            var emp = _connectionContext.TriggerContext.EmployeeRepository.Update(employee);
            return emp.result;
        }

        public int UpdateEmployeeProfilePic(Trigger.DTO.EmpProfile empProfile)
        {
            var emp = _connectionContext.TriggerContext.EmpProfileRepository.Update(empProfile);
            return emp.result;
        }

        public int UpdateEmployeeProfile(Trigger.DTO.EmployeeProfile employeeProfile)
        {
            var emp = _connectionContext.TriggerContext.EmployeeProfileRepository.Update(employeeProfile);
            return emp.result;
        }

        public int UpdateEmployeeSalary(Trigger.DTO.EmployeeSalary employeeSalary)
        {
            var emp = _connectionContext.TriggerContext.EmpSalaryRepository.Update(employeeSalary);
            return emp.result;
        }

        public int UpdateUser(Trigger.DTO.UserDetails userDetails)
        {
            var emp = _connectionContext.TriggerContext.UserDetailRepository.UpdateUser(userDetails);
            return emp.result;
        }

        public void Update1AuthUser(Trigger.DTO.AuthUserDetails authUserDetails)
        {
            _catalogDbContext.AuthUserDetailsRepository.Update(authUserDetails);
        }

        public void UpdateAuthUserClaims(Trigger.DTO.Claims claims)
        {
            _catalogDbContext.ClaimsCommonRepository.Update(claims);
        }

        public int UpdateEmpIsMailSent(EmployeeModel employee)
        {
            var emp = _connectionContext.TriggerContext.EmployeeRepository.UpdateEmpForIsMailSent(employee);
            return emp.result;
        }

        public Trigger.DTO.AuthUserDetails DeleteAuthUser(Trigger.DTO.AuthUserDetails authUserDetails)
        {
            return _catalogDbContext.AuthUserDetailsRepository.Delete(authUserDetails);
        }       
    }
}
