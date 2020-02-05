using System;
using System.Collections.Generic;
using System.Text;
using OneRPP.Restful.DataAnnotations;
using OneRPP.Restful.DAO;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
	[QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
	public class EmployeeCsvRepository : DaoRepository<EmployeeCsvModel>
	{
		private const string AddEmpolyeeFromCsv = "AddEmpolyeeFromCSV";

		/// <summary>
		/// Add employee details from Csv
		/// </summary>
		/// <param name="employeeCsvModel"></param>
		/// <returns>EmployeeCsvModel</returns>
		public EmployeeCsvModel AddEmpolyee(EmployeeCsvModel employeeCsvModel)
		{
			return ExecuteQuery<EmployeeCsvModel>(employeeCsvModel, AddEmpolyeeFromCsv);
		}
	}
}
