using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Dashboard;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for Dashboard
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : Controller
    {
        private readonly Dashboard _dashboard;

        /// <summary>
        /// Costructor for Dashboard
        /// </summary>
        /// <param name="dashboard"></param>
        public DashboardController(Dashboard dashboard)
        {
            _dashboard = dashboard;
        }

        /// <summary>
        /// Method to get Employee Dashboard by EmpId
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        [HttpGet("{empId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get(int empId)
        {
            return await _dashboard.GetEmployeeDashBoardAsyncV1(empId);
        }

        /// <summary>
        /// Get Yearwise Manager Dashboard by Comany and Managerid
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="yearId"></param>
        /// <param name="departmentList"></param>
        /// <returns></returns>
        [HttpGet("{companyId}/{managerId}/{yearId}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get(int companyId, int managerId, int yearId, string departmentList)
        {
            return await _dashboard.GetYearlyDepartmentWiseManagerDashBoardAsync(companyId, managerId, yearId, departmentList);
        }
        /// <summary>
        /// UnUsed API to respond with app version upgrade message for mobile apps
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        [HttpGet("{companyId}/{managerId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> GetDashboard([FromRoute]int companyId, int managerId)
        {
            return await _dashboard.GetDashboard();
        }

        /// <summary>
        /// UnUsed API to resond with app version upgrade message for mobile apps
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="departmentList"></param>
        /// <returns></returns>
        [HttpGet("{companyId}/{managerId}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> GetDashboard([FromRoute]int companyId, int managerId, string departmentList)
        {
            return await _dashboard.GetDashboard();
        }

    }
}