using System;
using System.Collections.Generic;
using System.Text;
using OneRPP.Restful.DataAnnotations;
using OneRPP.Restful.DAO;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
	[QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
	public class AuthUserClaimCsvRepository : DaoRepository<AuthUserClaimCsvModel>
	{
		private const string AddAuthClaimsFromCsv = "AddAuthClaimsFromCSV";

		/// <summary>
		/// Add employee details from Csv
		/// </summary>
		/// <param name="employeeCsvModel"></param>
		/// <returns>EmployeeCsvModel</returns>
		public AuthUserClaimCsvModel AddAuthClaims(AuthUserClaimCsvModel employeeCsvModel)
		{
			return ExecuteQuery<AuthUserClaimCsvModel>(employeeCsvModel, AddAuthClaimsFromCsv);
		}
	}
}
