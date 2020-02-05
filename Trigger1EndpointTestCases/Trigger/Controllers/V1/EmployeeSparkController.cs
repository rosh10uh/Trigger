using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Spark;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Class Name   :   EmployeeSparkController
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   16 Aug 2019
    /// Purpose      :   Controller to perform Employee Spark
    /// Revision     :  
    /// </summary>
    [ApiVersion("1.0")]
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
        /// Method to get list of sparks for an employee
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/EmployeeSpark/{empId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get(int empId)
        {
            return await _employeeSpark.GetAsync(empId);
        }

        /// <summary>
        /// Method to get list of sparks for an employee
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UnApprovedSpark")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetUnApprovedSpark()
        {
            return await _employeeSpark.GetUnApprovedSparkAsync();
        }

        /// <summary>
        /// API to approve or reject spark performed by sms
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/SmsSparkApproval")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<CustomJsonData> UpdateSparkApprovalStatus([FromBody] EmployeeSparkModel employeeSparkModel)
        {
            return await _employeeSpark.UpdateSparkApprovalStatus(employeeSparkModel);
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
            return await _employeeSpark.PostAsync(employeeSparkModel);
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
            return await _employeeSpark.PutAsync(employeeSparkModel);
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
            return await _employeeSpark.DeleteAsync(empId, sparkId, userId);
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
            return await _employeeSpark.DeleteAttachmentAsync(employeeSparkModel);
        }
    }
}