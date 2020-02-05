using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.AssessmentYear;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Contains API for Yearwise Assessment for Dashboard
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentYearController : ControllerBase
    {
        private readonly AssessmentYear _assessmentYear;

        /// <summary>
        /// Yearwise Assessment Module
        /// </summary>
        /// <param name="assessmentYear"></param>
        public AssessmentYearController(AssessmentYear assessmentYear)
        {
            _assessmentYear = assessmentYear;
        }

        /// <summary>
        /// Get Yearwise Assessment for Dashboard
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        [HttpGet("{companyId}/{managerId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get([FromRoute]int companyId, int managerId)
        {
            return await _assessmentYear.GetAssessmentYearAsync(companyId, managerId);
        }
    }
}