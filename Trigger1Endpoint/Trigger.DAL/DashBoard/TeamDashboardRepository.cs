using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;
using Trigger.DTO.DashBoard;

namespace Trigger.DAL.DashBoard
{
    [QueryPath("Trigger.DAL.Query.DashBoard.DashBoard")]
    public class TeamDashboardRepository : DaoRepository<TeamDashboardModel>
    {
        private const string GetYearWiseTeamDashboard = "GetYearWiseTeamDashboard";
       
        public TeamDashboardModel GetTeamDashboard(TeamDashboardModel teamDashboard)
        {
            return ExecuteQuery<TeamDashboardModel>(teamDashboard, GetYearWiseTeamDashboard);
        }

    }

    public class TeamDashboardModel
    {
        [TableIndex(1)]
        public List<TeamAvgScoreModel> TeamAvgScore { set; get; }

        [TableIndex(2)]
        public List<TeamAvgScoreByDayModel> TeamAvgeScoreByDay { set; get; }
        
        public int YearId { get; set; }

        public int TeamId { get; set; }

        public int CompanyId { get; set; }

        public int ManagerId { get; set; }
    }
}
