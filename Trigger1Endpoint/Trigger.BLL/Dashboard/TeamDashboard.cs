using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.DashBoard;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DashBoard;
using Trigger.Utility;

namespace Trigger.BLL.Dashboard
{
    public class TeamDashboard
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<TeamDashboard> _logger;
        private readonly IActionPermission _permission;
        private readonly IClaims _claims;

        /// <summary>
        /// Constructor to get Team Dashboard
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        public TeamDashboard(IConnectionContext connectionContext, ILogger<TeamDashboard> logger, IActionPermission permission, IClaims claims)
        {
            _connectionContext = connectionContext;
            _logger = logger;
            _permission = permission;
            _claims = claims;
        }

        /// <summary>
        /// Method Name  :   GetTeamDashboard
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   11 September 2019
        /// Purpose      :   Version 1 : Get team dashboard data as per action permission given for loggedin user 
        /// </summary>
        /// <param name="yearId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetTeamDashboard(int yearId, int teamId)
        {
            try
            {
                bool canView = _permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.TeamDashboard, Enums.PermissionType.CanView));
                if (canView)
                {
                    TeamDashboardModel teamDashboard = await GetTeamDashboard(new TeamDashboardModel { YearId = yearId, TeamId = teamId, ManagerId = Convert.ToInt32(_claims["EmpId"].Value) });
                    if (teamDashboard != null && teamDashboard.TeamAvgScore.Select(x=>x.TeamAvgscore).FirstOrDefault() != 0)
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(teamDashboard, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_404), "");
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(new TeamDashboardModel(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }

        }

        /// <summary>
        /// Method Name  :   GetTeamDashboard
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   11 September 2019
        /// Purpose      :   Version 1 : Get team dashboard multiple tables from database 
        /// </summary>
        /// <param name="teamDashboard"></param>
        /// <returns></returns>
        private async Task<TeamDashboardModel> GetTeamDashboard(TeamDashboardModel teamDashboard)
        {
            try
            {
                teamDashboard = await Task.FromResult(_connectionContext.TriggerContext.TeamDashboardRepository.GetTeamDashboard(new TeamDashboardModel() { TeamId = teamDashboard.TeamId, YearId = teamDashboard.YearId, ManagerId = teamDashboard.ManagerId }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            return teamDashboard;
        }

        /// <summary>
        /// Method Name  :   GetYearWiseTeamList
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   18 September 2019
        /// Purpose      :   Version 1 : Get year and permission wise Team List
        /// </summary>
        /// <param name="yearId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetYearWiseTeamList(int yearId)
        {
            try
            {
                bool canView = false;
                List<int> teamTypes = new List<int>();
                if (Convert.ToInt32(_claims["RoleId"].Value) != Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    var permissions = _permission.GetPermissionsV2_1(Convert.ToInt32(_claims["EmpId"].Value)).Where(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ToList();
                    if (permissions.Count > 0)
                    {
                        canView = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ActionPermissions.Any(x => x.DimensionId != Enums.DimensionType.Role.GetHashCode() && x.CanView);
                        teamTypes = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ActionPermissions.Where(x => x.DimensionId == Enums.DimensionType.Team.GetHashCode()).Select(x => x.DimensionValueid).ToList();
                    }
                }
                if (Convert.ToInt32(_claims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || canView)
                {
                    List<TeamListForDashboard> teamLists = await Task.FromResult(_connectionContext.TriggerContext.TeamListRepository.GetTeamListForDashboard(new TeamListForDashboard() { YearId = yearId, ManagerId = Convert.ToInt32(_claims["EmpId"].Value) }));
                    if (teamTypes?.Count > 0)
                    {
                        teamLists = teamLists.Where(x => teamTypes.Contains(x.TeamType)).ToList();
                    }
                    if (teamLists?.Count > 0)
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(teamLists, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(teamLists, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noDataFound);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(new List<TeamListForDashboard>(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }


        public int GetTeamListByPermission(int yearId)
        {
            bool canView = false;
            List<int> teamTypes = new List<int>();
            List<TeamListForDashboard> teamLists = new List<TeamListForDashboard>();
            if (Convert.ToInt32(_claims["RoleId"].Value) != Enums.DimensionElements.CompanyAdmin.GetHashCode())
            {
                var permissions = _permission.GetPermissionsV2_1(Convert.ToInt32(_claims["EmpId"].Value)).Where(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ToList();
                if (permissions.Count > 0)
                {
                    canView = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ActionPermissions.Any(x => x.DimensionId != Enums.DimensionType.Role.GetHashCode() && x.CanView);
                    teamTypes = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ActionPermissions.Where(x => x.DimensionId == Enums.DimensionType.Team.GetHashCode()).Select(x => x.DimensionValueid).ToList();
                }
            }
            if (Convert.ToInt32(_claims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || canView)
            {
                teamLists = _connectionContext.TriggerContext.TeamListRepository.GetTeamListForDashboard(new TeamListForDashboard() { YearId = yearId, ManagerId = Convert.ToInt32(_claims["EmpId"].Value) });
                if (teamTypes?.Count > 0)
                {
                    teamLists = teamLists.Where(x => teamTypes.Contains(x.TeamType)).ToList();
                }
            }
            return teamLists.Count;
        }

        /// <summary>
        /// Method Name  :   GetTeamListCountByPermission
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   11 November 2019
        /// Purpose      :   Version 1 : Get permission wise Team List
        /// </summary>
        /// <returns></returns>
        public int GetTeamListCountByPermission()
        {
            bool canView = false;
            List<int> teamTypes = new List<int>();
            List<TeamListForDashboard> teamLists = new List<TeamListForDashboard>();
            if (Convert.ToInt32(_claims["RoleId"].Value) != Enums.DimensionElements.CompanyAdmin.GetHashCode())
            {
                var permissions = _permission.GetPermissionsV2_1(Convert.ToInt32(_claims["EmpId"].Value)).Where(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ToList();
                if (permissions.Count > 0)
                {
                    canView = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ActionPermissions.Any(x => x.DimensionId != Enums.DimensionType.Role.GetHashCode() && x.CanView);
                    teamTypes = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ActionPermissions.Where(x => x.DimensionId == Enums.DimensionType.Team.GetHashCode()).Select(x => x.DimensionValueid).ToList();
                }
            }
            if (Convert.ToInt32(_claims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || canView)
            {
                teamLists = _connectionContext.TriggerContext.TeamListRepository.GetTeamListCountForDashboard(new TeamListForDashboard() { ManagerId = Convert.ToInt32(_claims["EmpId"].Value) });
                if (teamTypes?.Count > 0)
                {
                    teamLists = teamLists.Where(x => teamTypes.Contains(x.TeamType)).ToList();
                }
            }
            return teamLists.Count;
        }


        /// <summary>
        /// Method Name  :   GetTeamAssessmentYear
        /// Author       :   Roshan Patel
        /// Creation Date:   15 October 2019
        /// Purpose      :   Version 1 : Get year wise Assessment for Team Dashboard
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns>Task<CustomJsonData></returns>
        public async Task<CustomJsonData> GetTeamAssessmentYear()
        {
            try
            {
                bool canView = false;
                List<int> teamTypes = new List<int>();
                List<TeamListForDashboard> assessmentYearsWithTeamType;
                List<AssessmentYearModel> assessmentYears = new List<AssessmentYearModel>();

                if (Convert.ToInt32(_claims["RoleId"].Value) != Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    var permissions = _permission.GetPermissionsV2_1(Convert.ToInt32(_claims["EmpId"].Value)).Where(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ToList();
                    if (permissions.Count > 0)
                    {
                        canView = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ActionPermissions.Any(x => x.DimensionId != Enums.DimensionType.Role.GetHashCode() && x.CanView);
                        teamTypes = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.TeamDashboard.GetHashCode()).ActionPermissions.Where(x => x.DimensionId == Enums.DimensionType.Team.GetHashCode()).Select(x => x.DimensionValueid).ToList();
                    }
                }

                if (Convert.ToInt32(_claims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || canView)
                {
                    var teamListForDashboard = new TeamListForDashboard { ManagerId = Convert.ToInt32(_claims["EmpId"].Value) };
                    assessmentYearsWithTeamType = await Task.FromResult(_connectionContext.TriggerContext.TeamListRepository.GetAssessmentYear(teamListForDashboard));
                    if (teamTypes?.Count > 0)
                    {
                        assessmentYearsWithTeamType = assessmentYearsWithTeamType.Where(x => teamTypes.Contains(x.TeamType)).ToList();
                    }
                    int teamMinYear = assessmentYearsWithTeamType.Select(x => x.YearId).Distinct().Min();
                    int currentYear = DateTime.Now.Year;

                    while (teamMinYear <= currentYear)
                    {
                        assessmentYears.Add(new AssessmentYearModel { AssessedYear = teamMinYear.ToString() });
                        teamMinYear++;
                    }

                    if (assessmentYears.Count > 0)
                    {
                        assessmentYears = assessmentYears.OrderByDescending(x => x.AssessedYear).ToList();
                        return JsonSettings.UserCustomDataWithStatusMessage(assessmentYears, Enums.StatusCodes.status_200.GetHashCode(), Messages.ok);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_404.GetHashCode(), Messages.dataNotFound);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }
    }
}
