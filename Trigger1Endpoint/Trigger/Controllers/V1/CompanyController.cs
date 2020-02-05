using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Company;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for new Client Creation
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/client")]
    [ApiController]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly Company _company;

        /// <summary>
        /// Company Constructor
        /// </summary>
        /// <param name="company"></param>
		public CompanyController(Company company)
        {
            _company = company;
        }

        /// <summary>
        /// Get list of All Companies(Clients)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<CustomJsonData>> Get()
        {
            return await _company.SelectAllAsync();
        }

        /// <summary>
        /// Get Company Details by CompanyId
        /// </summary>
        /// <param name="compId"></param>
        /// <returns></returns>
        // GET: api/Company/5
        [HttpGet("{compId}")]
        public async Task<ActionResult<CustomJsonData>> Get(int compId)
        {
            var companyDetailsModel = new CompanyDetailsModel() { compId = compId };
            return await _company.SelectAsync(companyDetailsModel);
        }

        /// <summary>
        /// POST: api/Company/userId
        /// Save Company Details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyDetailsModel"></param>
        /// <returns></returns>
		[HttpPost("{userId}")]
        [ParameterValidationAttribute]
        public async Task<ActionResult<JsonData>> Post(int userId, [FromBody] CompanyDetailsModel companyDetailsModel)
        {
            companyDetailsModel.createdBy = userId;
            return await _company.InsertAsync(companyDetailsModel);
        }

        /// <summary>
        ///  PUT: api/Company/5
        ///  Update Company Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyDetailsModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ParameterValidationAttribute]
        public async Task<ActionResult<JsonData>> Put(int id, [FromBody] CompanyDetailsModel companyDetailsModel)
        {
            companyDetailsModel.updatedBy = id;
            return await _company.UpdateAsync(companyDetailsModel);
        }

        /// <summary>
        /// DELETE: api/Company/5
        /// Delete Company Details(Soft Delete)
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
		[Route("{companyId}/{updatedBy}")]
        [HttpDelete]
        public async Task<ActionResult<JsonData>> Delete([FromRoute]string companyId, int updatedBy)
        {
            return await _company.DeleteAsync(new CompanyDetailsModel { companyId = companyId, updatedBy = updatedBy });
        }

        /// <summary>
        /// API for scheduling Inactivity Reminder Scheduler
        /// </summary>
        /// <returns></returns>
        [HttpPatch("AddInActivityScheduler")]
        public async Task<ActionResult<JsonData>> AddInactivityScheduler()
        {
            return await _company.AddInactivityScheduler();
        }

        /// <summary>
        /// API for scheduling Team notification
        /// </summary>
        /// <returns></returns>
        [HttpPatch("AddTeamNotiifcationScheduler")]
        public async Task<ActionResult<JsonData>> AddTeamNotiifcationScheduler()
        {
            return await _company.AddTeamNotiifcationScheduler();
        }

    }
}
