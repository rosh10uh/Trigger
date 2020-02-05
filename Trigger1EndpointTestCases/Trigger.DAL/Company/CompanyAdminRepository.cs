using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.Company
{
    /// <summary>
    /// Class to manage company admin details
    /// </summary>
    [QueryPath("Trigger.DAL.Query.Company.Company")]
    public class CompanyAdminRepository : DaoRepository<EmployeeModel>
    {
        /// <summary>
        /// Method to add/update company admin details
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual int AddCompanyAdminDetails(EmployeeModel employeeModel)
        {
            return ExecuteQuery<int>(employeeModel, SPFileName.InvokeAddCompanyAdminDetails);
        }

        /// <summary>
        /// Method to delete company admin details
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual int DeleteCompanyAdminDetails(EmployeeModel employeeModel)
        {
            return ExecuteQuery<int>(employeeModel, SPFileName.InvokeDeleteCompanyAdminDetails);
        }
    }
}
