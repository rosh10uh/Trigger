using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trigger.BLL.TeamConfiguration;
using Trigger.DTO;
using Trigger.DTO.TeamConfiguration;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Class Name   :   TeamConfiguration
    /// Author       :   Bhumika Bhavsar
    /// Creation Date:   26 August 2019
    /// Purpose      :   Controller for Team wise action permission Configurations
    /// Revision     :  
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamConfigurationController : ControllerBase
    {
        private readonly TeamConfiguration _teamConfiguration;

        /// <summary>
        /// Constructor for team configuration
        /// </summary>
        /// <param name="teamConfiguration"></param>
        public TeamConfigurationController(TeamConfiguration teamConfiguration)
        {
            _teamConfiguration = teamConfiguration;
        }

        /// <summary>
        /// API To get all Teams details
        /// </summary>
        /// <returns></returns>
        [HttpGet("{companyId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get()
        {
            return await _teamConfiguration.GetAllTeams();
        }

        /// <summary>
        /// Api to get details of team by teamId
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("{companyId}/{teamId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetTeamDetailsById(int teamId)
        {
            return await _teamConfiguration.GetTeamDetailsById(teamId);
        }

        /// <summary>
        /// Api to add Team configuration
        /// </summary>
        /// <param name="teamConfiguration"></param>
        /// <returns></returns>
        [HttpPost("{companyId}")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidation]
        public async Task<CustomJsonData> TeamConfigurationAsync(TeamConfigurationModel teamConfiguration )
        {
            return await _teamConfiguration.AddTeamConfiguration(teamConfiguration);
        }

        /// <summary>
        /// Api to update Team configuration
        /// </summary>
        /// <param name="teamConfiguration"></param>
        /// <returns></returns>
        [HttpPut("{companyId}")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidation]
        public async Task<CustomJsonData> UpdateTeamConfigurationAsync(TeamConfigurationModel teamConfiguration)
        {
            return await _teamConfiguration.UpdateTeamConfiguration(teamConfiguration);
        }


        /// <summary>
        /// Api to Inactive Team configuration
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="teamId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("SetInactive/{companyId}/{teamId}/{userId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> SetTeamAsInActiveAsync([FromRoute] int companyId, int teamId, int userId)
        {
            return await _teamConfiguration.SetTeamStatusInActive(new TeamConfigurationModel { TeamId = teamId, UpdatedBy = userId });
        }
    }
}