using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Dashboard;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V2_1
{
    /// <summary>
    /// API's for Dashboard
    /// </summary>
    [ApiVersion("2.1")]
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
        /// Method to get Employee Dashboard by EmpId version 2
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        [HttpGet("{empId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get(int empId)
        {
            return await _dashboard.GetEmployeeDashBoardAsyncV2_1(empId);
        }

    }
}