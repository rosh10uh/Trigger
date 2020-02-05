using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Trigger.Utility
{
    public static class ConvertToDataTable
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties by using reflection   
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names  
                    dataTable.Columns.Add(prop.Name, prop.PropertyType);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }

            return dataTable;
        }

        public static bool SqlBulkInsert(string ConnString, DataTable dtbSource, string DestinationTableName)
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

        public static string EvalNullString(string field, string optional)
        {
            if (field == null)
            {
                field = optional;
            }
            return field;
        }
    }
}
