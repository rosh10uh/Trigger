using System.Collections.Generic;
using OneRPP.Restful.DataAnnotations;
using OneRPP.Restful.DAO;
using Trigger.DTO;
using Trigger.DAL.Shared;

namespace Trigger.DAL.ExcelUpload
{
	[QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
	public class ExcelUploadRepository : DaoRepository<MasterTables>
	{

		/// <summary>
		/// Get employee details by Id
		/// </summary>
		/// <param name="masterTables"></param>
		/// <returns>MasterTables</returns>
		public MasterTables GetMasterData(MasterTables masterTables)
		{
			return ExecuteQuery<MasterTables>(masterTables,SPFileName.GetAllMastersForExcel);
		}

		/// <summary>
		/// Delete temp employees
		/// </summary>
		/// <returns>int</returns>
		public int DeleteTempEmployee()
		{
			return ExecuteQuery<int>(null, SPFileName.TempEmployeeDelete);
		}

		/// <summary>
		/// Get employee details by Id
		/// </summary>
		/// <param name="masterTables"></param>
		/// <returns>MasterTables</returns>
		public virtual List<ExcelData> GetExcelDataCount(MasterTables masterTables)
		{
			return ExecuteQuery<List<ExcelData>>(masterTables, SPFileName.GetExcelDataWithCount);
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
		public List<ExcelEmployeeDetailsModel> EmployeeDetails { set; get; }

		public int CompanyId { get; set; }
	}
}
