using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;

namespace Trigger.DAL.DashBoard
{
    [QueryPath("Trigger.DAL.Query.DashBoard")]
    public class EmployeeDashboardRepository : DaoRepository<EmployeeDashboardModel>
	{
		private const string GetTenantName = "GetTenantName";
        private const string invokeGetEmployeeDashboard = "GetEmployeeDashboard";

        /// <summary>
        /// Get tenant name
        /// </summary>
        /// <param name="employeeDashboardModel"></param>
        /// <returns></returns>
        public virtual string GetTenantNameByCompanyId(EmployeeDashboardModel employeeDashboardModel)
		{
			return ExecuteQuery<string>(employeeDashboardModel, GetTenantName);
		}
        public virtual List<EmployeeDashboardModel> GetEmployeeDashboard(EmployeeDashboardModel employeeDashboardModel)
        {
            return ExecuteQuery<List<EmployeeDashboardModel>>(employeeDashboardModel, invokeGetEmployeeDashboard);
        }
    }
}
