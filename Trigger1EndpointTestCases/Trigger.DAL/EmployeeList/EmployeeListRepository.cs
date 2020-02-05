using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.EmployeeList
{
    /// <summary>
    /// Class to get employee list
    /// </summary>
    [QueryPath("Trigger.DAL.Query.Employee.Employee")]
    public class EmployeeListRepository : DaoRepository<EmployeeListModel>
    {
        /// <summary>
        /// Method to get all employee list
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeListModel> GetAllEmployee(EmployeeListModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeListModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployee);
        }

        /// <summary>
        /// Method to get employee list with hierachy
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeListModel> GetAllEmployeeWithHierachy(EmployeeListModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeListModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployeeWithHierachy);
        }
    }
}
