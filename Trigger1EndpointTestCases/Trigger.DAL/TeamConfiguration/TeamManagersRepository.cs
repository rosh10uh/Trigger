using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO.TeamConfiguration;

namespace Trigger.DAL.TeamConfiguration
{
    [QueryPath("Trigger.DAL.Query.TeamConfiguration.TeamConfiguration")]
    public class TeamManagersRepository : DaoRepository<List<TeamManagerModel>>
    {
        private const string invokeAddTeamManagersConfiguration = "AddTeamManagers";
        private const string invokeUpdateTeamManagersConfiguration = "UpdateTeamManagers";
        private const string invokeGetAllTeamManagersByTeamId = "GetAllTeamManagersByTeamId";
        private const string invokeDeleteTeamManagersByTeamId = "DeleteTeamManagersByTeamId";


        public List<TeamManagerModel> InsertTeamManagers(List<TeamManagerModel> teamManagerModels)
        {
            return ExecuteQuery<List<TeamManagerModel>>(teamManagerModels, invokeAddTeamManagersConfiguration);
        }

        public List<TeamManagerModel> UpdateTeamManagers(List<TeamManagerModel> teamManagerModels)
        {
            //return ExecuteQuery<List<TeamManagerModel>>(teamManagerModels, invokeAddTeamManagersConfiguration);
            return ExecuteQuery<List<TeamManagerModel>>(teamManagerModels, invokeUpdateTeamManagersConfiguration);
        }

        public List<TeamManagerModel> GetAllTeamManagersByTeamId(List<TeamManagerModel> teamManagerModels)
        {
            return ExecuteQuery<List<TeamManagerModel>>(teamManagerModels, invokeGetAllTeamManagersByTeamId);
        }

        public List<TeamManagerModel> DeleteTeamManagersByTeamId(List<TeamManagerModel> teamManagerModels)
        {
            return ExecuteQuery<List<TeamManagerModel>>(teamManagerModels, invokeDeleteTeamManagersByTeamId);
        }
    }
}
