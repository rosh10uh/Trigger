using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
    /// <summary>
    /// Class for add employee user login details
    /// </summary>
	[QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
	public class UserExcelRepository : DaoRepository<UserExcelModel>
	{
        /// <summary>
        /// Add employee details from excel
        /// </summary>
        /// <param name="userExcelModel"></param>
        /// <returns>UserExcelModel</returns>
        public UserExcelModel AddUserLogin(UserExcelModel userExcelModel)
		{
			return ExecuteQuery<UserExcelModel>(userExcelModel, SPFileName.AddUserLoginFromExcel);
		}
	}
}
