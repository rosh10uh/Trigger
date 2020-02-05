using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO.TeamConfiguration;

namespace Trigger.DAL.TeamConfiguration
{
    [QueryPath("Trigger.DAL.Query.TeamConfiguration.TeamConfiguration")]
    public class TeamConfigurationRepository : DaoRepository<TeamConfigurationModel>
    {
        private const string invokeGetTeamDetailsById = "GetTeamDetailsById";
        private const string invokeGetTeamManagersByTeamId = "GetTeamManagersByTeamId";
        private const string invokeGetTeamEmployeesByTeamId = "GetTeamEmployeesByTeamId";
        private const string invokeAddTeamMainConfiguration = "AddTeamConfiguration";
        private const string invokeUpdateTeamMainConfiguration = "UpdateTeamConfiguration";
        private const string invokeSetTeamStatusInActive = "SetTeamStatusInActive";
        private const string invokeDeleteTeamManagers = "DeleteTeamManagers";
        private const string invokeDeleteTeamEmployees = "DeleteTeamEmployees";
        private const string invokeGetAllTeamManagersByTeamId = "GetAllTeamManagersByTeamId";

        public TeamConfigurationModel GetTeamDetailsById(TeamConfigurationModel teamConfiguration)
        {
            TeamConfigurationModel team = ExecuteQuery<TeamConfigurationModel>(teamConfiguration, invokeGetTeamDetailsById);
            team.TeamManagers = ExecuteQuery<List<TeamManagerModel>>(teamConfiguration, invokeGetTeamManagersByTeamId);
            team.TeamEmployees = ExecuteQuery<List<TeamEmployeesModel>>(teamConfiguration, invokeGetTeamEmployeesByTeamId);

            return team;
        }

        public TeamConfigurationModel InsertTeamMainConfiguration(TeamConfigurationModel teamConfigurationModel)
        {
            return ExecuteQuery<TeamConfigurationModel>(teamConfigurationModel, invokeAddTeamMainConfiguration);
        }

        public TeamConfigurationModel UpdateTeamMainConfiguration(TeamConfigurationModel teamConfigurationModel)
        {
            return ExecuteQuery<TeamConfigurationModel>(teamConfigurationModel, invokeUpdateTeamMainConfiguration);
        }

        public TeamConfigurationModel SetTeamStatusInactive(TeamConfigurationModel teamConfigurationModel)
        {
            return ExecuteQuery<TeamConfigurationModel>(teamConfigurationModel, invokeSetTeamStatusInActive);
        }

        public TeamConfigurationModel DeleteTeamManagers(TeamConfigurationModel teamManagerModels)
        {
            return ExecuteQuery<TeamConfigurationModel>(teamManagerModels, invokeDeleteTeamManagers);
        }

        public TeamConfigurationModel DeleteTeamEmployees(TeamConfigurationModel teamEmployeeModels)
        {
            return ExecuteQuery<TeamConfigurationModel>(teamEmployeeModels, invokeDeleteTeamEmployees);
        }

        public TeamConfigurationModel GetAllTeamManagersByTeamId(TeamConfigurationModel teamConfiguration)
        {
            TeamConfigurationModel team = new TeamConfigurationModel();
            team.TeamManagers = ExecuteQuery<List<TeamManagerModel>>(teamConfiguration, invokeGetAllTeamManagersByTeamId);
            return team;
        }
    }
}
