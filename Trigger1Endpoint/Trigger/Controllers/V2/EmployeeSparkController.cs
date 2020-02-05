using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Spark;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Middleware;

namespace Trigger.Controllers.V2
{
    /// <summary>
    /// Class Name   :   EmployeeSparkController
    /// Author       :   Mayur Patel
    /// Creation Date:   17 September 2019
    /// Purpose      :   Controller to perform Employee Spark with newly added team wise permission
    /// Revision     :  
    /// </summary>
    [ApiVersion("2.0")]
    [ApiController]
    public class EmployeeSparkController : ControllerBase
    {
        private readonly EmployeeSpark _employeeSpark;

        /// <summary>
        /// constructor to initialized employeespark object
        /// </summary>
        /// <param name="employeeSpark"></param>
        public EmployeeSparkController(EmployeeSpark employeeSpark)
        {
            _employeeSpark = employeeSpark;
        }


        /// <summary>
        /// API to spark an employee with document attachment
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/EmployeeSpark")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<CustomJsonData> PostAsync([FromBody] EmployeeSparkModel employeeSparkModel)
        {
            return await _employeeSpark.AddEmployeeSparkV2(employeeSparkModel);
        }



        /// <summary>
        /// API to update spark of an employee including document attachment
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/EmployeeSpark")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<CustomJsonData> PutAsync([FromBody] EmployeeSparkModel employeeSparkModel)
        {
            return await _employeeSpark.UpdateEmployeeSparkV2(employeeSparkModel);
        }


        /// <summary>
        /// API to delete spark of an employee
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="sparkId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/EmployeeSpark/{empId}/{sparkId}/{userId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> DeleteAsync([FromRoute] int empId, int sparkId, int userId)
        {
            return await _employeeSpark.DeleteEmployeeSparkV2(empId, sparkId, userId);
        }

        /// <summary>
        /// API to delete attachment of spark for an employee
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/EmployeeSpark/Attachment")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> DeleteAttachmentAsync([FromBody] EmployeeSparkModel employeeSparkModel)
        {
            return await _employeeSpark.DeleteSparkAttachmentV2(employeeSparkModel);
        }
    }
}