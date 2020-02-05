using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO.DashBoard;

namespace Trigger.DAL.DashBoard
{
    [QueryPath("Trigger.DAL.Query.DashBoard.DashBoard")]
    public class TeamListRepository : DaoRepository<TeamListForDashboard>
    {
        private const string GetYearWiseTeamList = "GetYearWiseTeamList";
        private const string GetTeamAssessmentYear = "GetTeamAssessmentYear";
        private const string GetTeamListCount = "GetTeamListCount";

        public List<TeamListForDashboard> GetTeamListForDashboard(TeamListForDashboard teamList)
        {
            return ExecuteQuery<List<TeamListForDashboard>>(teamList, GetYearWiseTeamList);
        }

        public List<TeamListForDashboard> GetAssessmentYear(TeamListForDashboard teamList)
        {
            return ExecuteQuery<List<TeamListForDashboard>>(teamList, GetTeamAssessmentYear);
        }

        public List<TeamListForDashboard> GetTeamListCountForDashboard(TeamListForDashboard teamList)
        {
            return ExecuteQuery<List<TeamListForDashboard>>(teamList, GetTeamListCount);
        }
    }

    
}
