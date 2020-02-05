using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
    /// <summary>
    /// Class for add employee details and company admin details
    /// </summary>
    [QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
    public class EmployeeExcelRepository : DaoRepository<EmployeeExcelModel>
    {
        /// <summary>
        /// Add employee details from excel
        /// </summary>
        /// <param name="employeeExcelModel"></param>
        /// <returns>EmployeeExcelModel</returns>
        public virtual EmployeeExcelModel AddEmpolyee(EmployeeExcelModel employeeExcelModel)
        {
            return ExecuteQuery<EmployeeExcelModel>(employeeExcelModel, SPFileName.AddEmpolyeeFromExcel);
        }

        /// <summary>
        /// Add company admin details from excel
        /// </summary>
        /// <param name="employeeExcelModel"></param>
        /// <returns>EmployeeExcelModel</returns>
        public virtual EmployeeExcelModel AddCompanyAdminDetails(EmployeeExcelModel employeeExcelModel)
        {
            return ExecuteQuery<EmployeeExcelModel>(employeeExcelModel, SPFileName.AddCompanyAdminFromExcel);
        }
    }
}
