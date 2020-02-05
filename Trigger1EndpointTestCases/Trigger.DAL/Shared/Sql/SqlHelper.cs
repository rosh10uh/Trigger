using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Trigger.DAL.Shared.Sql
{
    public class SqlHelper
    {
        public static int ExecuteNonQuery(SqlParameter[] sqlParameters, string connectionString, string spName)
        {
            int result;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                using (var sqlCommand = new SqlCommand())
                {
                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = spName;
                    if (sqlParameters.Any())
                        sqlCommand.Parameters.AddRange(sqlParameters);

                    result = sqlCommand.ExecuteNonQuery();
                    sqlCommand.Parameters.Clear();
                    return result;
                }
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public virtual bool SqlBulkInsert(string ConnString, DataTable dtbSource, string DestinationTableName)
        {
            bool blnResult = false;
            using (var bulkCopy = new SqlBulkCopy(ConnString, SqlBulkCopyOptions.KeepNulls))
            {
                // my DataTable column names match my SQL Column names, so I simply made this loop. However if your column names don't match, just pass in which datatable name matches the SQL column name in Column Mappings
                foreach (DataColumn col in dtbSource.Columns)
                {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }

                bulkCopy.BulkCopyTimeout = 6000;
                bulkCopy.DestinationTableName = DestinationTableName;
                bulkCopy.WriteToServer(dtbSource);
                blnResult = true;
            }
            return blnResult;
        }
    }
}
