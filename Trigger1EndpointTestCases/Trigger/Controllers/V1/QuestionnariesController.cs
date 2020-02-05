using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API for Questionnaries
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnariesController : Controller
    {
        private readonly BLL.Questionnaries.Questionnaries _questionnaires;

        /// <summary>
        /// Constructor for Questionaries
        /// </summary>
        /// <param name="questionnaires"></param>
        public QuestionnariesController(BLL.Questionnaries.Questionnaries questionnaires)
        {
            _questionnaires = questionnaires;
        }

        /// <summary>
        /// Get List of all Questions and Answers for Assessment Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DynamicConnection]
        [Authorize]
        public async Task<CustomJsonData> Get()
        {
            return await _questionnaires.GetAllQuestionnaries();
        }

        /// <summary>
        /// Get List of all Categories from master
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet("category/{companyId}")]
        [DynamicConnection]
        [Authorize]
        public async Task<CustomJsonData> GetAllCategories([FromRoute] int companyId)
        {
            return await _questionnaires.GetAllCategories();
        }

        /// <summary>
        /// To get score ranks from score remarks master
        /// </summary>
        /// <returns></returns>
        [HttpGet("ScoreRank")]
        [DynamicConnection]
        [Authorize]
        public async Task<CustomJsonData> GetScoreRank()
        {
            return await _questionnaires.GetScoreRanks();
        }
    }
}