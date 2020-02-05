using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.Shared;

namespace Trigger.Middleware
{
    /// <summary>
    /// Landing Middleware Manager to validate and set response for login
    /// </summary>
    public class LandingMiddlewareManager : ILandingMiddlewareManager
    {
        private readonly string storageAccountUrl;
        private readonly string _blobContainerEmployee = Messages.profilePic;
        private readonly string _blobContainerCompany = Messages.companyLogo;
        private readonly ILogger<LandingMiddlewareManager> _iLogger;

        /// <summary>
        /// Constructor for Landing Middleware
        /// </summary>
        public LandingMiddlewareManager(ILogger<LandingMiddlewareManager> iLogger)
        {
            storageAccountUrl = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.StorageAccountURL.ToString()];
            _iLogger = iLogger;
        }

        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DTO.UserDataModel CheckUserLogin(string connectionString, string userName)
        {
            var loggedUser = new DTO.UserDataModel();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] {
                                           new SqlParameter("@username", userName),
                                           new SqlParameter("@result", string.Empty)
                                                     };

                var loggedUserDetails = GetDataTableADO(connectionString, Messages.uspUserLogin, sqlParameters);

                if (loggedUserDetails.Rows.Count == 1)
                {
                    if (loggedUserDetails.Columns.Contains("Error"))
                    {
                        loggedUser.Message = Convert.ToString(loggedUserDetails.Rows[0]["Error"]);
                        return loggedUser;
                    }
                    else
                    {
                        return SetLoggedUserdData(loggedUserDetails.Rows[0]);
                    }
                }
                else
                {
                    loggedUser = null;
                }
            }
            catch (Exception)
            {
                //exception
            }
            return loggedUser;

        }

        /// <summary>
        /// Set response data from datarow object
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private Trigger.DTO.UserDataModel SetLoggedUserdData(DataRow dataRow)
        {
            return new Trigger.DTO.UserDataModel()
            {
                empId = Convert.ToInt32(dataRow["empid"]),
                empEmailId = Convert.ToString(dataRow["email"]),
                userId = Convert.ToInt32(dataRow["userid"]),
                userName = Convert.ToString(dataRow["username"]),
                companyid = Convert.ToInt32(dataRow["companyid"]),
                roleId = Convert.ToInt32(dataRow["roleid"]),
                role = Convert.ToString(dataRow["role"]),
                Message = "",
                employee = SetEmloyeeDetails(dataRow)
            };
        }

        /// <summary>
        /// Set employeedetails based on login response
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private Trigger.DTO.EmployeeModel SetEmloyeeDetails(DataRow dataRow)
        {
            return new Trigger.DTO.EmployeeModel()
            {
                EmpId = Convert.ToInt32(dataRow["empid"]),
                CompanyId = Convert.ToInt32(dataRow["companyid"]),
                FirstName = Convert.ToString(dataRow["firstname"]),
                MiddleName = Convert.ToString(dataRow["middlename"]),
                LastName = Convert.ToString(dataRow["lastname"]),
                Email = Convert.ToString(dataRow["email"]),
                JobTitle = Convert.ToString(dataRow["jobtitle"]),
                Suffix = Convert.ToString(dataRow["suffix"]),
                DepartmentId = Convert.ToInt32(dataRow["departmentid"]),
                Department = Convert.ToString(dataRow["department"]),
                PhoneNumber= Convert.ToString(dataRow["phonenumber"]),
                EmpFolderPath = "",
                EmpImgPath = (dataRow["empImgPath"].ToString() == null || dataRow["empImgPath"].ToString() == string.Empty) ? string.Empty : (storageAccountUrl + Messages.slash + _blobContainerEmployee + Messages.slash + dataRow["empImgPath"].ToString()),
                CompanyLogoPath = (dataRow["compLogoPath"].ToString() == null || dataRow["compLogoPath"].ToString() == string.Empty) ? string.Empty : (storageAccountUrl + Messages.slash + _blobContainerCompany + Messages.slash + dataRow["compLogoPath"].ToString())
            };
        }

        /// <summary>
        /// Get response as datatable  for sql operation
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="storedProcedureName"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public DataTable GetDataTableADO(string connectionString, string storedProcedureName, SqlParameter[] sqlParameters)
        {
            DataTable dtbManagerDashboard = ExecuteDataTable(connectionString, storedProcedureName, sqlParameters, null);
            return dtbManagerDashboard;
        }

        /// <summary>
        /// Get response as datatable  for sql operation
        /// </summary>
        public DataTable ExecuteDataTable(string connectionString, string storedProcedureName, SqlParameter[] sqlParameters, object[] parameterValues)
        {
            var dataTable = new DataTable();
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                using (var cmd = BeginStoredProcedure(sqlConnection, storedProcedureName, sqlParameters, parameterValues))
                {
                    using (var sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        sqlDataAdapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        private SqlCommand BeginStoredProcedure(SqlConnection sqlConnection, string storedProcedure, SqlParameter[] parameters, object[] parameterValues)
        {
            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();  // connect to the database
                }

                return CreateCommand(storedProcedure, sqlConnection, CommandType.StoredProcedure, parameters, parameterValues, UpdateRowSource.None);
            }
            catch (SqlException ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
        }

        private SqlCommand CreateCommand(
            string storedProcedure,
            SqlConnection sqlConnection,
            CommandType commandType,
            SqlParameter[] sqlParameters,
            object[] parameterValues,
            UpdateRowSource updateRowSource)
        {
            var cmd = new SqlCommand(storedProcedure, sqlConnection) { CommandType = commandType, CommandTimeout = 300 };

            if (sqlParameters != null)
            {
                for (var idx = 0; idx < sqlParameters.Length; ++idx)
                {
                    cmd.Parameters.Add(sqlParameters[idx]);

                    if (parameterValues != null && parameterValues.Length > idx && parameterValues[idx] != null)
                    {
                        cmd.Parameters[sqlParameters[idx].ParameterName].Value = parameterValues[idx];
                    }
                }
            }

            cmd.UpdatedRowSource = updateRowSource;
            return cmd;
        }
    }
}
