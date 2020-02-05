using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.EmpSalary
{
    /// <summary>
    /// Class to manage employee salary
    /// </summary>
    [QueryPath("Trigger.DAL.Query.EmpSalary.EmployeeSalary")]
    public class EmployeeSalaryRepository : DaoRepository<EmployeeSalary>
    {
    }
}
