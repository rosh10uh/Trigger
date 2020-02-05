using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Assessment;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V2
{
    /// <summary>
    /// Contains API's for Assessment Module
    /// </summary>
    [ApiVersion("2.0")]
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
        /// Method to Save Assessment Details version 2
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
            return await _assessment.AddAssessmentAsyncV2(empAssessmentModel);
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
            return await _assessment.DeleteAssessmentAttachmentV2(empAssessmentDet);
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
            return await _assessment.UpdateAssessmentCommentV2(empAssessmentDet);
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
            return await _assessment.DeleteAssessmentCommentV2(assessmentId, remarkId, userId);
        }
    }
}