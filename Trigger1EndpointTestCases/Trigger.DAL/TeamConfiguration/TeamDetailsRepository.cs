using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO.TeamConfiguration;

namespace Trigger.DAL.TeamConfiguration
{
    [QueryPath("Trigger.DAL.Query.TeamConfiguration.TeamConfiguration")]
    public class TeamDetailsRepository : DaoRepository<TeamDetailsTables>
    {
        private const string invokeGetTeamDetailsById = "GetTeamDetailsById";

        /// <summary>
		/// Get employee details by Id
		/// </summary>
		/// <param name="masterTables"></param>
		/// <returns>MasterTables</returns>
        public TeamDetailsTables GetTeamDetailsById(TeamDetailsTables teamDetailsTables)
        {
            return ExecuteQuery<TeamDetailsTables>(teamDetailsTables, invokeGetTeamDetailsById);
        }
        
		
    }

    public class TeamDetailsTables
    {
        [TableIndex(1)]
        public List<TeamConfigurationModel> TeamMainData { get; set; }

        [TableIndex(2)]
        public List<TeamManagerModel> TeamManagers { get; set; }

        [TableIndex(3)]
        public List<TeamEmployeesModel> TeamEmployees { get; set; }

        public int TeamId { get; set; }
    }
}
