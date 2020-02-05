using System.Collections.Generic;
using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
    /// <summary>
    /// Class for add/get employee auth  details
    /// </summary>
	[QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
	public class AuthUserExcelRepository : DaoRepository<AuthUserExcelModel>
	{
        /// <summary>
        /// Add authorized user details from excel
        /// </summary>
        /// <param name="authUserExcelModel"></param>
        /// <returns>AuthUserExcelModel</returns>
        public virtual AuthUserExcelModel AddAuthLogin(AuthUserExcelModel authUserExcelModel)
		{
			return ExecuteQuery<AuthUserExcelModel>(authUserExcelModel, SPFileName.AddAuthLoginFromExcel);
		}

        /// <summary>
        /// get authorized user list
        /// </summary>
        /// <returns>List of AuthUserDetails</returns>
        public virtual List<AuthUserDetails> GetListAuthUser()
		{
			return ExecuteQuery<List<AuthUserDetails>>(null, SPFileName.GetExistingAuthUsers);
		}

        /// <summary>
        /// Get employee auth users by phone number from catalog
        /// </summary>
        /// <param name="authUserExcelModel"></param>
        /// <returns>List</returns>
        public virtual List<ExcelData> GetEmployeeByPhoneNumberCatalog(AuthUserExcelModel authUserExcelModel)
        {
            return ExecuteQuery<List<ExcelData>>(authUserExcelModel, SPFileName.InvokeGetEmpByPhnNumberCatalog);
        }

        /// <summary>
        /// Get employee auth users by phone number from tenant
        /// </summary>
        /// <param name="authUserExcelModel"></param>
        /// <returns>List</returns>
        public virtual List<ExcelData> GetEmployeeByPhoneNumberTenant(AuthUserExcelModel authUserExcelModel)
        {
            return ExecuteQuery<List<ExcelData>>(authUserExcelModel, SPFileName.InvokeGetEmpByPhnNumberTenant);
        }
    }
}
