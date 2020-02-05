using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.EmpSalary
{
    [QueryPath("Trigger.DAL.Query.EmpSalary.EmployeeSalary")]
    public class EmployeeSalaryRepository : DaoRepository<EmployeeSalary>
    {
    }
}
