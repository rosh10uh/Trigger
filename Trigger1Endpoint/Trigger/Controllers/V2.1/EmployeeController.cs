using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Employee;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V2_1
{
    /// <summary>
    /// controller to perform action for employee
    /// </summary>
    //[Route("api/[controller]")]
    [ApiVersion("2.1")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DashboardEmployeeList _dashboardEmployee;
        private readonly TriggerEmployeeList _triggerEmployee;

        /// <summary>
        /// employee constructor 
        /// </summary>
        /// <param name="dashboardEmployee"></param>
        /// <param name="triggerEmployee"></param>
        public EmployeeController(DashboardEmployeeList dashboardEmployee, TriggerEmployeeList triggerEmployee)
        {
            _dashboardEmployee = dashboardEmployee;
            _triggerEmployee = triggerEmployee;
        }

        
        /// <summary>
        /// Api version 2.0 to get employee list for Trigger employee 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/triggeremplist/{companyId}/{managerId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetTriggerEmpList([FromRoute]int companyId, int managerId)
        {
            return await _triggerEmployee.GetTriggerEmpListV2_1(companyId, managerId);
        }


        /// <summary>
        /// Api version 2.0 to get employee list for Employee Dashboard
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/dashboardemplist/{companyId}/{managerId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetDashboardEmpList([FromRoute]int companyId, int managerId)
        {
            return await _dashboardEmployee.GetDashboardEmpListV2_1(companyId, managerId);
        }
    }
}
