using System.Collections.Generic;
using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
	[QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
	public class AuthUserCsvRepository : DaoRepository<AuthUserCsvModel>
	{
		private const string AddAuthLoginFromCsv = "AddAuthLoginFromCSV";
		private const string GetExistingAuthUsers = "GetExistingAuthUsers";
        private const string InvokeGetEmpByPhnNumberCatalog = "GetEmployeeByPhoneNumberCatalog";
        private const string InvokeGetEmpByPhnNumberTenant = "GetEmployeeByPhoneNumberTenant";

        /// <summary>
        /// Add authorized user details from Csv
        /// </summary>
        /// <param name="authUserCsvModel"></param>
        /// <returns>AuthUserCsvModel</returns>
        public AuthUserCsvModel AddAuthLogin(AuthUserCsvModel authUserCsvModel)
		{
			return ExecuteQuery<AuthUserCsvModel>(authUserCsvModel, AddAuthLoginFromCsv);
		}

		/// <summary>
		/// get authorized user list
		/// </summary>
		/// <returns>List</returns>
		public List<AuthUserDetails> GetListAuthUser()
		{
			return ExecuteQuery<List<AuthUserDetails>>(null, GetExistingAuthUsers);
		}

        /// <summary>
        /// Get employee auth users by phone number from catalog
        /// </summary>
        /// <param name="authUserCsvModel"></param>
        /// <returns>List</returns>
        public List<CsvData> GetEmployeeByPhoneNumberCatalog(AuthUserCsvModel authUserCsvModel)
        {
            return ExecuteQuery<List<CsvData>>(authUserCsvModel, InvokeGetEmpByPhnNumberCatalog);
        }

        /// <summary>
        /// Get employee auth users by phone number from tenant
        /// </summary>
        /// <param name="authUserCsvModel"></param>
        /// <returns>List</returns>
        public List<CsvData> GetEmployeeByPhoneNumberTenant(AuthUserCsvModel authUserCsvModel)
        {
            return ExecuteQuery<List<CsvData>>(authUserCsvModel, InvokeGetEmpByPhnNumberTenant);
        }
    }
}
