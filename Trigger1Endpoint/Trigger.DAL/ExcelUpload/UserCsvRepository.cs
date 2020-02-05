using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
	[QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
	public class UserCsvRepository : DaoRepository<UserCsvModel>
	{
		private const string AddUserLoginFromCsv = "AddUserLoginFromCSV";
		
		/// <summary>
		/// Add employee details from Csv
		/// </summary>
		/// <param name="userCsvModel"></param>
		/// <returns>UserCsvModel</returns>
		public UserCsvModel AddUserLogin(UserCsvModel userCsvModel)
		{
			return ExecuteQuery<UserCsvModel>(userCsvModel, AddUserLoginFromCsv);
		}
	}
}
