using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Assessment;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Contains API's for Assessment Module
    /// </summary>
    //[Route("api/[controller]")]
    [ApiVersion("1.0")]
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
        /// Method to get Assessment Score
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="empId"></param>
        /// <returns></returns>
        //[HttpGet("{companyId}/{empId}")]
        [Route("api/Assessment/{companyId}/{empId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> Get([FromRoute]int companyId, int empId)
        {
            return await _assessment.GetAssessmentScore(companyId, empId);
        }

        /// <summary>
        /// POST api/values 
        /// Method to Save Assessment Details
        /// </summary>
        /// <param name="empAssessmentModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Assessment")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> PostAsync([FromBody] EmpAssessmentModel empAssessmentModel)
        {
            return await _assessment.AddAssessmentAsyncV1(empAssessmentModel);
        }

        /// <summary>
        /// Delete assessment attachment for comment and remove document name from assessment details
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Assessment/Attachment")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> DeleteDocumentAsync([FromBody]EmpAssessmentDet empAssessmentDet)
        {
            return await _assessment.DeleteAssessmentAttachment(empAssessmentDet);
        }

        /// <summary>
        /// Update assessment general and categorywise comments
        /// </summary>
        /// <param name="empAssessmentDet"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Assessment/comment")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> UpdateAssessmentCommentAsync([FromBody] EmpAssessmentDet empAssessmentDet)
        {
            return await _assessment.UpdateAssessmentComment(empAssessmentDet);
        }

        /// <summary>
        /// API to delete assessment comment including attachment with that comment for remark category
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="remarkId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Assessment/comment/{assessmentId}/{remarkId}/{userId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> DeleteAssessmentCommentAsync([FromRoute]int assessmentId, int remarkId, int userId)
        {
            return await _assessment.DeleteAssessmentComment(assessmentId, remarkId, userId);
        }

        // <summary>
        /// Update assessment score feedback
        /// </summary>
        /// <param name="assessmentScoreModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Assessment/ScoreFeedback")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> UpdateAssessmentScoreFeedback([FromBody] AssessmentScoreModel assessmentScoreModel)
        {
            return await _assessment.UpdateAssessmentScoreFeddback(assessmentScoreModel);
        }
    }
}