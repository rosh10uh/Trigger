using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Department;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for Department
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : Controller
    {
        private readonly Department _department;

        /// <summary>
        /// Constructor for Department
        /// </summary>
        /// <param name="department"></param>
        public DepartmentController(Department department)
        {
            _department = department;
        }

        /// <summary>
        /// Get List of all Departments
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get()
        {
            return await _department.GetAllDepartmentAsync();
        }

        /// <summary>
        /// Get Company and Year wise Departments
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="yearId"></param>
        /// <returns></returns>
        [HttpGet("{CompanyId}/{YearId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetCompanyAndYearwiseDepartments(int companyId, string yearId)
        {
            return await _department.GetCompanyAndYearwiseDepartments(companyId, yearId);
        }

        /// <summary>
        /// Get Company wise Departments
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet("{CompanyId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetCompanywiseDepartments(int companyId)
        {
            return await _department.GetCompanywiseDepartments(companyId);
        }

        /// <summary>
        /// Add New Department
        /// </summary>
        /// <param name="departmentModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> PostAsync([FromBody] DepartmentModel departmentModel)
        {
            return await _department.AddDepartmentAsync(departmentModel);
        }

        /// <summary>
        /// Update Existing Deparment
        /// </summary>
        /// <param name="departmentModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> PutAsync([FromBody] DepartmentModel departmentModel)
        {
            return await _department.UpdateDepartmentAsync(departmentModel);
        }

        /// <summary>
        /// Delete Department
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="departmentId"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        [HttpDelete("{companyId}/{departmentId}/{updatedBy}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> DeleteAsync(int companyId, int departmentId, string updatedBy)
        {
            return await _department.DeleteDepartmentAsync(companyId, departmentId, updatedBy);
        }
    }
}