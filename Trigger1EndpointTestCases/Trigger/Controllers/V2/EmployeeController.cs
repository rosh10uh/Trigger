using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Employee;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V2
{
    /// <summary>
    /// controller to perform action for employee
    /// </summary>
    //[Route("api/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly Employee _employee;

        /// <summary>
        /// employee constructor 
        /// </summary>
        /// <param name="employee"></param>
        public EmployeeController(Employee employee)
        {
            _employee = employee;
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
            return await _employee.GetTriggerEmpListV2(companyId, managerId);
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
            return await _employee.GetDashboardEmpListV2(companyId, managerId);
        }
    }
}
