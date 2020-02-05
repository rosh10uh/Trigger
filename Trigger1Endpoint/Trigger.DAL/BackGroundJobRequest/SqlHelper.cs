using System.Data;
using System.Data.SqlClient;
using Trigger.BLL.Shared;
using Trigger.DTO.TeamConfiguration;

namespace Trigger.DAL.BackGroundJobRequest
{
    /// <summary>
    /// Class Name   :   SqlHelper
    /// Author       :   Mayur Patel
    /// Creation Date:   16 september 2019
    /// Purpose      :   manage ado.net methods, used in background job request
    /// Revision     :  
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// Method to get data in datatable form. (without parameter)
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static DataTable GetDataByStoreProcedure(string spName, string connString)
        {
            DataTable companyDetails = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(connString);
            sqlConnection.Open();
            using (var sqlCommand = new SqlCommand(spName, sqlConnection))
            {
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(companyDetails);
                sqlConnection.Close();
                sqlDataAdapter.Dispose();
                return companyDetails;
            }
        }

        /// <summary>
        /// Method to get team employee details by team id
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="connString"></param>
        ///  <param name="spName"></param>
        /// <returns></returns>
        public static DataTable GetTeamEmployeeDetailsByTeamId(int teamId, string connString, string spName)
        {
            DataTable companyDetails = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(connString);
            sqlConnection.Open();
            using (var sqlCommand = new SqlCommand(spName, sqlConnection))
            {
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@TeamId", teamId);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(companyDetails);
                sqlConnection.Close();
                sqlDataAdapter.Dispose();
                return companyDetails;
            }
        }

        /// <summary>
        /// Method to get team employee details by team id wich have no assessment performed as per team trigger activity days
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="connString"></param>
        ///  <param name="triggerActivityDays"></param>
        /// <returns></returns>
        public static DataTable GetTeamInactivityEmployeByTeamId(int teamId, string connString, int triggerActivityDays)
        {
            DataTable companyDetails = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(connString);
            sqlConnection.Open();
            using (var sqlCommand = new SqlCommand(Messages.GetTeamInactivityEmployee, sqlConnection))
            {
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@TeamId", teamId);
                sqlCommand.Parameters.AddWithValue("@triggerActivitydays", triggerActivityDays);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(companyDetails);
                sqlConnection.Close();
                sqlDataAdapter.Dispose();
                return companyDetails;
            }
        }

        /// <summary>
        /// Method to add log of set send employee inactivity email to team manager
        /// </summary>
        /// <param name="connectionString"></param>
        ///  <param name="teamInActivityLogModel"></param>
        /// <returns></returns>
        public static int AddTeamInActivityLog(string connectionString, TeamInActivityLogModel teamInActivityLogModel)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            int inactivityLog = 0;
            sqlConnection.Open();

            using (var sqlCommand = new SqlCommand(Messages.AddTeamInActivityLog, sqlConnection))
            {
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.Add(SetSqlParameter("@teamid ", ParameterDirection.Input, SqlDbType.Int, teamInActivityLogModel.TeamId));
                sqlCommand.Parameters.Add(SetSqlParameter("@emailto ", ParameterDirection.Input, SqlDbType.NVarChar, teamInActivityLogModel.EmailTo));
                sqlCommand.Parameters.Add(SetSqlParameter("@emailtext ", ParameterDirection.Input, SqlDbType.NVarChar, teamInActivityLogModel.EmailText));
                sqlCommand.Parameters.Add(SetSqlParameter("@triggeractivitydays", ParameterDirection.Input, SqlDbType.Int, teamInActivityLogModel.TriggerActivitydays));
                sqlCommand.Parameters.Add(SetSqlParameter("@createdby", ParameterDirection.Input, SqlDbType.Int, teamInActivityLogModel.CreatedBy));
                sqlCommand.Parameters.Add(SetSqlParameter("@Result", ParameterDirection.InputOutput, SqlDbType.Int, inactivityLog));

                inactivityLog = sqlCommand.ExecuteNonQuery();
            }
            return inactivityLog;
        }

        /// <summary>
        /// Method to get email template by name
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="companyId"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static string GetTemplateByName(string templateName, int companyId, string connString)
        {
            string template = string.Empty;
            SqlConnection sqlConnection = new SqlConnection(connString);
            using (var sqlCommand = new SqlCommand())
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = Messages.GetTemplateByName;
                sqlCommand.Parameters.AddWithValue("@companyId", companyId);
                sqlCommand.Parameters.AddWithValue("@templateName", templateName);
                template = (string)sqlCommand.ExecuteScalar();
                sqlCommand.Parameters.Clear();
            }

            return template;
        }

        /// <summary>
        /// Method to get team managers email id
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="companyId"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static string GetTeamManagersEmailIds(int teamId, string connString)
        {
            DataTable companyDetails = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(connString);
            sqlConnection.Open();
            using (var sqlCommand = new SqlCommand(Messages.GetTeamManagersEmailIds, sqlConnection))
            {
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(SetSqlParameter("@teamid", ParameterDirection.Input, SqlDbType.Int, teamId));

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(companyDetails);
                sqlConnection.Close();
                sqlDataAdapter.Dispose();
            }
            return string.Join(",", companyDetails.AsEnumerable().Select(r => r.Field<string>("email")));
        }

        /// <summary>
        /// Method to in active team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static int InActiveTeam(int teamId, string connString)
        {
            int result = 0;
            SqlConnection sqlConnection = new SqlConnection(connString);
            using (var sqlCommand = new SqlCommand())
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = Messages.DeleteTeamConfiguration;
                sqlCommand.Parameters.AddWithValue("@TeamId", teamId);
                sqlCommand.Parameters.AddWithValue("@UpdatedBy", 1);
                sqlCommand.Parameters.AddWithValue("@Result", 1);
                result = (int)sqlCommand.ExecuteScalar();
                sqlCommand.Parameters.Clear();
            }
            return result;
        }

        /// <summary>
        /// Method to manage sql parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="parameterDirection"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static SqlParameter SetSqlParameter(string paramName, ParameterDirection parameterDirection, SqlDbType sqlDbType, object value)
        {
            SqlParameter sqlParameter = new SqlParameter(paramName, value)
            {
                Direction = parameterDirection,
                SqlDbType = sqlDbType
            };
            return sqlParameter;
        }
    }
}
