using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.TeamConfiguration;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.DTO.TeamConfiguration;
using Trigger.Utility;

namespace Trigger.BLL.TeamConfiguration
{
    /// <summary>
    /// Class Name   :   TeamConfiguration
    /// Author       :   Bhumika Bhavsar
    /// Creation Date:   26 August 2019
    /// Purpose      :   Business Logic for Team Configuration
    /// Revision     : 
    /// </summary>
    public class TeamConfiguration
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<TeamConfigurationModel> _logger;
        private readonly TeamConfigurationContext _teamConfigurationContext;
        private readonly TeamBackgroundJobRequest _teamBackgroundJobRequest;
        private readonly IActionPermission _permission;
        private readonly IClaims _claims;

        public TeamConfiguration(IConnectionContext connectionContext, ILogger<TeamConfigurationModel> logger,
            TeamConfigurationContext teamConfigurationContext, TeamBackgroundJobRequest teamBackgroundJobRequest, IActionPermission permission, IClaims claims)
        {
            _connectionContext = connectionContext;
            _logger = logger;
            _teamConfigurationContext = teamConfigurationContext;
            _teamBackgroundJobRequest = teamBackgroundJobRequest;
            _permission = permission;
            _claims = claims;

        }

        /// <summary>
        /// Method Name  :   GetAllTeams
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   26 August 2019
        /// Purpose      :   Get all teams details for listing purpose
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllTeams()
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.TeamConfiguration, Enums.PermissionType.CanView)) || Convert.ToInt32(_claims["RoleId"].Value) ==  Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    List<TeamConfigurationModel> teams = await Task.FromResult(_connectionContext.TriggerContext.TeamConfigurationRepository.SelectAll());
                    if (teams?.Count > 0)
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(teams, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(teams, Convert.ToInt32(Enums.StatusCodes.status_100), Messages.noDataFound);
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
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }

        }

        /// <summary>
        /// Method Name  :   GetTeamDetailsById
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   26 August 2019
        /// Purpose      :   Get team details by teamid
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetTeamDetailsById(int teamId)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.TeamConfiguration, Enums.PermissionType.CanView)) || Convert.ToInt32(_claims["RoleId"].Value) == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    TeamConfigurationModel teamDetailsTables = await GetTeamConfigurationById(teamId);
                    return JsonSettings.UserCustomDataWithStatusMessage(teamDetailsTables, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(new TeamConfigurationModel(), Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   AddTeamConfiguration
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   27 August 2019
        /// Purpose      :   Add Team Configuration details
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> AddTeamConfiguration(TeamConfigurationModel teamConfigurationModel)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.TeamConfiguration, Enums.PermissionType.CanAdd)) || Convert.ToInt32(_claims["RoleId"].Value) == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    var result = await InsertTeamConfiguration(teamConfigurationModel);
                    if (result > 0)
                    {
                        SendNotificationMailToManagersAsync(teamConfigurationModel);
                    }
                    return GetResponseStatus(result);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   InsertTeamConfiguration
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   27 August 2019
        /// Purpose      :   Add Team Configuration details
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        /// <returns></returns>
        private async Task<int> InsertTeamConfiguration(TeamConfigurationModel teamConfigurationModel)
        {
            try
            {
                return await _teamConfigurationContext.AddTeamConfigurationMain(teamConfigurationModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return -1;
            }
        }

        /// <summary>
        /// Method Name  :   UpdateTeamConfiguration
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   30 August 2019
        /// Purpose      :   Update Team Configuration details
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> UpdateTeamConfiguration(TeamConfigurationModel teamConfigurationModel)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.TeamConfiguration, Enums.PermissionType.CanEdit)) || Convert.ToInt32(_claims["RoleId"].Value) == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {

                    TeamConfigurationModel existingConfiguration = await GetTeamConfigurationById(teamConfigurationModel.TeamId);
                    List<TeamManagerModel> newManagers = teamConfigurationModel.TeamManagers.Where(x => x.Id == 0).ToList();
                    List<int> managerIds = teamConfigurationModel.TeamManagers.Select(x => x.ManagerId).ToList();
                    List<TeamManagerModel> removedManagers = existingConfiguration.TeamManagers.Where(x => !managerIds.Contains(x.ManagerId)).ToList();

                    var result = await EditTeamConfiguration(teamConfigurationModel);
                    if (result > 0)
                    {
                        if (existingConfiguration.Status && !teamConfigurationModel.Status)
                        {
                            SendInActiveNotificationMailToManagersAsync(teamConfigurationModel);
                        }
                        else if (!existingConfiguration.Status && teamConfigurationModel.Status)
                        {
                            SendNotificationMailToManagersAsync(teamConfigurationModel);
                        }
                        else if (teamConfigurationModel.Status
                                && (existingConfiguration.StartDate != teamConfigurationModel.StartDate
                                    || newManagers.Count > 0 
                                    || removedManagers.Count > 0
                                    || existingConfiguration.EndDate != teamConfigurationModel.EndDate
                                    || existingConfiguration.Description != teamConfigurationModel.Description
                                    || existingConfiguration.TriggerActivityDays != teamConfigurationModel.TriggerActivityDays
                                    )
                                )
                        {
                            UpdateTeamEmailNotificationAsync(teamConfigurationModel, newManagers, removedManagers);
                        }
                    }
                    return GetResponseStatus(result, Enums.TransactionType.Update);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   UpdateTeamEmailNotificationAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   30 August 2019
        /// Purpose      :   Send email notification on update team configuration details
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        /// <param name="newManagers"></param>
        /// <param name="removedManagers"></param>
        /// <returns></returns>
        private void UpdateTeamEmailNotificationAsync(TeamConfigurationModel teamConfigurationModel, List<TeamManagerModel> newManagers, List<TeamManagerModel> removedManagers)
        {
            try
            {
                List<EmployeeBasicModel> employeeModels = GetManagersDetails(teamConfigurationModel);
                List<EmployeeBasicModel> newManagerModel = GetManagersDetails(new TeamConfigurationModel { TeamManagers = newManagers });
                List<EmployeeBasicModel> removedManagerModel = GetManagersDetails(new TeamConfigurationModel { TeamManagers = removedManagers });
                _teamBackgroundJobRequest.SendNotificationOnTeamConfigChange(employeeModels, newManagerModel, removedManagerModel, teamConfigurationModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method Name  :   EditTeamConfiguration
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   30 August 2019
        /// Purpose      :   Update Team Configuration details
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        /// <returns></returns>
        private async Task<int> EditTeamConfiguration(TeamConfigurationModel teamConfigurationModel)
        {
            try
            {
                return await _teamConfigurationContext.UpdateTeamConfigurationMain(teamConfigurationModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return -1;
            }
        }

        /// <summary>
        /// Method Name  :   SetTeamStatusInActive
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   03 September 2019
        /// Purpose      :   Inactive Team Configuration
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> SetTeamStatusInActive(TeamConfigurationModel teamConfigurationModel)
        {
            try
            {
                if (_permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.TeamConfiguration, Enums.PermissionType.CanDelete)) || Convert.ToInt32(_claims["RoleId"].Value) == Enums.DimensionElements.TriggerAdmin.GetHashCode())
                {
                    var result = await _teamConfigurationContext.SetTeamStatusInactive(teamConfigurationModel);
                    if (result > 0)
                    {
                        TeamConfigurationModel team = await GetTeamConfigurationById(teamConfigurationModel.TeamId);
                        SendInActiveNotificationMailToManagersAsync(team);
                        return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.TeamIsInactiveMessage);
                    }
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.TeamIsAlreadyInactivated);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   SendNotificationMailToManagers
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   28 August 2019
        /// Purpose      :   Send notification mail to managers when they are included in any team
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        private void SendNotificationMailToManagersAsync(TeamConfigurationModel teamConfigurationModel)
        {
            try
            {
                List<EmployeeBasicModel> employeeModels = GetManagersDetails(teamConfigurationModel);
                _teamBackgroundJobRequest.SendTeamNotification(employeeModels, teamConfigurationModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method Name  :   SendInActiveNotificationMailToManagersAsync
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   04 September 2019
        /// Purpose      :   Send notification mail to managers when team is inactivated.
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        private void SendInActiveNotificationMailToManagersAsync(TeamConfigurationModel teamConfigurationModel)
        {
            try
            {
                List<EmployeeBasicModel> employeeModels = GetManagersDetails(teamConfigurationModel);
                _teamBackgroundJobRequest.SendInactiveTeamNotification(employeeModels, teamConfigurationModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method Name  :   GetManagersDetails
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   03 September 2019
        /// Purpose      :   Get Managers deatils for sending mail of team configuration
        /// </summary>
        /// <param name="teamConfigurationModel"></param>
        /// <returns></returns>
        private List<EmployeeBasicModel> GetManagersDetails(TeamConfigurationModel teamConfigurationModel)
        {
            List<TeamManagerModel> teamManagerModels = teamConfigurationModel.TeamManagers;
            string managerIds = string.Join(",", teamManagerModels.Select(x => x.ManagerId).ToArray());
            return _connectionContext.TriggerContext.EmployeeBasicRepository.GetManagersDetByManagerIds(new EmployeeBasicModel { EmpIdList = managerIds });
        }

        private CustomJsonData GetResponseStatus(int result, Enums.TransactionType transactionType = Enums.TransactionType.Insert)
        {
            switch (result)
            {
                case -1:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.TeamNameAlreadyExists);
                case 1:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), (transactionType == Enums.TransactionType.Insert ? Messages.TeamSuccessMessage : Messages.TeamUpdateSuccessMessage));
                case 0:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
                default:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        private async Task<TeamConfigurationModel> GetTeamConfigurationById(int teamId)
        {
            return await Task.FromResult(_connectionContext.TriggerContext.TeamConfigurationRepository.GetTeamDetailsById(new TeamConfigurationModel { TeamId = teamId }));
        }

    }
}
