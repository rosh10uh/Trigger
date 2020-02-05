using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Dashboard;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for Team Dashboard
    /// </summary>

    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamDashboardController : ControllerBase
    {
        private readonly TeamDashboard _teamDashboard;

        /// <summary>
        /// Costructor for Team Dashboard
        /// </summary>
        /// <param name="teamDashboard"></param>
        public TeamDashboardController(TeamDashboard teamDashboard)
        {
            _teamDashboard = teamDashboard;
        }

        /// <summary>
        /// Api to get team list as per year selection
        /// </summary>
        /// <param name="yearId"></param>
        /// <returns></returns>
        [HttpGet("{companyId}/{yearId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetYearWiseTeamList(int yearId)
        {
            return await _teamDashboard.GetYearWiseTeamList(yearId);
        }

        /// <summary>
        /// Api to get team dashboard
        /// </summary>
        /// <param name="yearId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("{companyId}/{yearId}/{teamId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetTeamDashboard(int yearId, int teamId)
        {
            return await _teamDashboard.GetTeamDashboard(yearId, teamId);
        }

        /// <summary>
        /// Get Year wise Assessment for Team Dashboard
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet("assessmentYear/{companyId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get(int companyId)
        {
            return await _teamDashboard.GetTeamAssessmentYear();
        }
    }
}