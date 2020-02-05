using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Assessment;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V2_2
{
    /// <summary>
    /// Version 2.2 : Contains API's for Assessment Module
    /// </summary>
    [ApiVersion("2.2")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        private readonly Assessment _assessment;

        /// <summary>
        /// Assessment Constructor
        /// </summary>
        /// <param name="assessment"></param>
        public AssessmentController(Assessment assessment)
        {
            _assessment = assessment;
        }

        /// <summary>
        /// POST api/values 
        /// Method to Save Assessment Details version 2.2
        /// </summary>
        /// <param name="empAssessmentModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Assessment")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidation]
        public async Task<JsonData> PostAsync([FromBody] EmpAssessmentModel empAssessmentModel)
        {
            return await _assessment.AddAssessmentAsyncV2_2(empAssessmentModel);
        }

    }
}