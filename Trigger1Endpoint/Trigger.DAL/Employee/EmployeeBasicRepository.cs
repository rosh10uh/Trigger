using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;
using Trigger.DTO;

namespace Trigger.DAL.Employee
{
    [QueryPath("Trigger.DAL.Query.Employee.Employee")]
    public class EmployeeBasicRepository : DaoRepository<EmployeeBasicModel>
    {
        private const string GetAllNonManagers = "GetAllNonManagers";
        private const string invokeGetManagersDetByManagerIds = "GetManagersDetByManagerIds";
        private const string invokeGetAspnetUserEmailList = "GetAspnetUserEmailList";
        private const string invokeGetActiveManagerList = "GetActiveManagerList";

        public List<EmployeeBasicModel> GetAllNonManagerList(EmployeeBasicModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeBasicModel>>(employeeModel, GetAllNonManagers);
        }

        public List<EmployeeBasicModel> GetManagersDetByManagerIds(EmployeeBasicModel employeeModels)
        {
            return ExecuteQuery<List<EmployeeBasicModel>>(employeeModels, invokeGetManagersDetByManagerIds);
        }


        /// <summary>
        /// Method Name  :   GetAspnetUserEmailList
        /// Author       :   Mayur Patel
        /// Creation Date:   26 September 2019
        /// Purpose      :   Get aspnet user email ids which have already generated password.
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public List<EmployeeBasicModel> GetAspnetUserEmailList(EmployeeBasicModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeBasicModel>>(employeeModel, invokeGetAspnetUserEmailList);
        }

        /// <summary>
        /// Method Name  :   GetActiveManagerList
        /// Author       :   Mayur Patel
        /// Creation Date:   26 September 2019
        /// Purpose      :   Get active manager list which employee have receive registration email and generate password
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public List<EmployeeBasicModel> GetActiveManagerList()
        {
            return ExecuteQuery<List<EmployeeBasicModel>>(null, invokeGetActiveManagerList);
        }
    }
}
