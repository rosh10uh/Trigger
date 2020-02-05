using System.Collections.Generic;
using OneRPP.Restful.DataAnnotations;
using OneRPP.Restful.DAO;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
	[QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
	public class ExcelUploadRepository : DaoRepository<MasterTables>
	{
		private const string GetAllMastersForCsv = "GetAllMastersForCSV";
		private const string TempEmployeeDelete = "DeleteTempEmployee";
		private const string GetCsvDataWithCount = "GetCSVDataWithCount";
		private const string AddEmpolyeeFromCsv = "AddEmpolyeeFromCSV";
       
        /// <summary>
        /// Add employee details from Csv
        /// </summary>
        /// <param name="masterTables"></param>
        /// <returns>MasterTables</returns>
        public MasterTables AddEmpolyee(MasterTables masterTables)
		{
			return ExecuteQuery<MasterTables>(masterTables, AddEmpolyeeFromCsv);
		}

		/// <summary>
		/// Get employee details by Id
		/// </summary>
		/// <param name="masterTables"></param>
		/// <returns>MasterTables</returns>
		public MasterTables GetMasterData(MasterTables masterTables)
		{
			return ExecuteQuery<MasterTables>(masterTables, GetAllMastersForCsv);
		}

		/// <summary>
		/// Delete temp employees
		/// </summary>
		/// <returns>int</returns>
		public int DeleteTempEmployee()
		{
			return ExecuteQuery<int>(null,TempEmployeeDelete);
		}

		/// <summary>
		/// Get employee details by Id
		/// </summary>
		/// <param name="masterTables"></param>
		/// <returns>MasterTables</returns>
		public List<CsvData> GetCsvDataCount(MasterTables masterTables)
		{
			return ExecuteQuery<List<CsvData>>(masterTables, GetCsvDataWithCount);
		}
             
    }

    /// <summary>
    /// Set dynamic master table
    /// </summary>
    public class MasterTables
	{
		[TableIndex(1)]
		public List<CountryModel> Country { set; get; }

		[TableIndex(2)]
		public List<RegionModel> Regions { set; get; }

		[TableIndex(3)]
		public List<RaceOrEthnicityModel> RaceOrEthnicity { set; get; }

		[TableIndex(4)]
		public List<DepartmentModel> Department { set; get; }

		[TableIndex(5)]
		public List<RoleModel> RoleMaster { set; get; }

		[TableIndex(6)]
		public List<CSVEmployeeDetailsModel> EmployeeDetails { set; get; }

		public int companyId { get; set; }
	}
}
