using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO.TeamConfiguration;
using Trigger.Utility;

namespace Trigger.DAL.TeamConfiguration
{
    public class TeamConfigurationContext
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger _logger;

        public TeamConfigurationContext(IConnectionContext connectionContext, ILogger<TeamConfigurationContext> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        public async Task<int> AddTeamConfigurationMain(TeamConfigurationModel teamConfigurationModel)
        {
            int result = 0;
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();
                int teamId = await Task.FromResult(_connectionContext.TriggerContext.TeamConfigurationRepository.InsertTeamMainConfiguration(teamConfigurationModel).TeamId);
                if (teamId > 0)
                {
                    teamConfigurationModel.TeamId = teamId;
                    teamConfigurationModel.TeamManagers.ForEach(x => x.TeamId = teamConfigurationModel.TeamId);
                    teamConfigurationModel.TeamEmployees.ForEach(x => x.TeamId = teamConfigurationModel.TeamId);

                    result = AddTeamManagers(teamConfigurationModel.TeamManagers);
                    if (result > 0)
                    {
                        result = AddTeamEmployees(teamConfigurationModel.TeamEmployees);
                        _connectionContext.TriggerContext.Commit();
                    }
                    else
                    {
                        _connectionContext.TriggerContext.RollBack();
                        return result;
                    }
                }
                else
                {
                    result = teamId;
                }
                return result;
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return result;
            }
        }

        private int AddTeamManagers(List<TeamManagerModel> teamManagerModels)
        {
            var teamConfig =  _connectionContext.TriggerContext.TeamManagersRepository.InsertTeamManagers(teamManagerModels);
            return (Convert.ToInt32(teamConfig.Last().Result));
        }

        private int AddTeamEmployees(List<TeamEmployeesModel> teamEmployeeModel)
        {
            var teamConfig = _connectionContext.TriggerContext.TeamEmployeesRepository.InsertTeamEmployees(teamEmployeeModel);
            return (Convert.ToInt32(teamConfig.Last().Result));
        }

        public async Task<int> UpdateTeamConfigurationMain(TeamConfigurationModel teamConfigurationModel)
        {
            int result = 0;
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();
                var configResult = await Task.FromResult(_connectionContext.TriggerContext.TeamConfigurationRepository.UpdateTeamMainConfiguration(teamConfigurationModel).Result);
                if (configResult  > 0)
                {
                    teamConfigurationModel.TeamManagers.ForEach(x => x.TeamId = teamConfigurationModel.TeamId);
                    teamConfigurationModel.TeamEmployees.ForEach(x => x.TeamId = teamConfigurationModel.TeamId);

                    teamConfigurationModel.TeamManagers.ForEach(x => x.CreatedBy = teamConfigurationModel.UpdatedBy);
                    teamConfigurationModel.TeamEmployees.ForEach(x => x.CreatedBy = teamConfigurationModel.UpdatedBy);

                    TeamConfigurationModel existingConfig =  await Task.FromResult(_connectionContext.TriggerContext.TeamConfigurationRepository.GetTeamDetailsById(new TeamConfigurationModel { TeamId = teamConfigurationModel.TeamId }));
                    
                    if (!CompareObjects.CompareObject(teamConfigurationModel.TeamManagers.Select(x => new { x.ManagerId, x.TeamId }).OrderBy(x => x.ManagerId), existingConfig.TeamManagers.Select(x => new { x.ManagerId, x.TeamId }).OrderBy(x => x.ManagerId)))
                    {
                        List<TeamManagerModel> existingManagers = existingConfig.TeamManagers; //await Task.FromResult(_connectionContext.TriggerContext.TeamConfigurationRepository.GetAllTeamManagersByTeamId(teamConfigurationModel).TeamManagers);
                        List<TeamManagerModel> newManagers = teamConfigurationModel.TeamManagers.Where(x => !existingManagers.Select(y => y.ManagerId).Contains(x.ManagerId)).ToList();
                        List<TeamManagerModel> removedManagers = existingManagers.Where(x => !teamConfigurationModel.TeamManagers.Select(y => y.ManagerId).Contains(x.ManagerId)).ToList();

                        if (newManagers.Count > 0)
                        {
                            result = UpdateTeamManagers(newManagers);
                        }
                        if (removedManagers.Count > 0)
                        {
                            result = DeleteTeamManagersByTeamId(removedManagers);
                        }
                    }
                    else
                    {
                        result = 1;
                    }

                    //if (DeleteTeamManagers(teamConfigurationModel) > 0)
                    //{
                    //    result = UpdateTeamManagers(teamConfigurationModel.TeamManagers);
                    //}
                    if (result > 0 && !CompareObjects.CompareObject(teamConfigurationModel.TeamEmployees.Select(x => new { x.EmpId, x.TeamId }).OrderBy(x => x.EmpId), existingConfig.TeamEmployees.Select(x => new { x.EmpId, x.TeamId }).OrderBy(x => x.EmpId)))
                    {
                        if (DeleteTeamEmployees(teamConfigurationModel) > 0)
                        {
                            result = UpdateTeamEmployees(teamConfigurationModel.TeamEmployees);
                            _connectionContext.TriggerContext.Commit();
                        }
                    }
                    else if(result == 1)
                    {
                        result = 1;
                        _connectionContext.TriggerContext.Commit();
                    }
                    else
                    {
                        _connectionContext.TriggerContext.RollBack();
                        return result;
                    }
                }
                else
                {
                    return configResult;
                }
                return result;
            }
            catch (Exception ex)
            {
                result = 0;
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return result;
            }
        }

        private int DeleteTeamManagersByTeamId(List<TeamManagerModel> removedManagers)
        {
            var teamConfig = _connectionContext.TriggerContext.TeamManagersRepository.DeleteTeamManagersByTeamId(removedManagers);
            return Convert.ToInt32(teamConfig.Last().Result);
        }

        private int DeleteTeamManagers(TeamConfigurationModel teamManagerModels)
        {
            var teamConfig = _connectionContext.TriggerContext.TeamConfigurationRepository.DeleteTeamManagers(teamManagerModels);
            return (Convert.ToInt32(teamConfig.Result));
        }

        private int DeleteTeamEmployees(TeamConfigurationModel teamEmployees)
        {
            var teamConfig = _connectionContext.TriggerContext.TeamConfigurationRepository.DeleteTeamEmployees(teamEmployees);
            return (Convert.ToInt32(teamConfig.Result));
        }

        private int UpdateTeamManagers(List<TeamManagerModel> teamManagerModels)
        {
            var teamConfig = _connectionContext.TriggerContext.TeamManagersRepository.UpdateTeamManagers(teamManagerModels);
            return (Convert.ToInt32(teamConfig.Last().Result));
        }

        private int UpdateTeamEmployees(List<TeamEmployeesModel> teamEmployeeModel)
        {
            var teamConfig = _connectionContext.TriggerContext.TeamEmployeesRepository.UpdateTeamEmployees(teamEmployeeModel);
            return (Convert.ToInt32(teamConfig.Last().Result));
        }

        public async Task<int> SetTeamStatusInactive(TeamConfigurationModel teamConfigurationModel)
        {
            var teamConfig = await Task.FromResult(_connectionContext.TriggerContext.TeamConfigurationRepository.SetTeamStatusInactive(teamConfigurationModel));
            return (Convert.ToInt32(teamConfig.Result));
        }
    }
}
