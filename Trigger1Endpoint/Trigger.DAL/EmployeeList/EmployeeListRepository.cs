using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;

namespace Trigger.DAL.EmployeeList
{
    [QueryPath("Trigger.DAL.Query.Employee.Employee")]
    public class EmployeeListRepository : DaoRepository<EmployeeListModel>
    {
        private const string invokeempGetAllEmployee = "GetAllEmployee";
        private const string invokeGetAllEmployeesForActionByManager = "GetAllEmployeesForActionByManager";
        private const string invokeEmpGetAllEmployeeWithHierachy = "GetAllEmployeeWithHierachy";

        public List<EmployeeListModel> GetAllEmployee(EmployeeListModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeListModel>>(employeeModel, invokeempGetAllEmployee);
        }

        public List<EmployeeListModel> GetAllEmployeeForActionBymanager(EmployeeListModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeListModel>>(employeeModel, invokeGetAllEmployeesForActionByManager);
        }

        public List<EmployeeListModel> GetAllEmployeeWithHierachy(EmployeeListModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeListModel>>(employeeModel, invokeEmpGetAllEmployeeWithHierachy);
        }
        
    }
}
