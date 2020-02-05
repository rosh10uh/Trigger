namespace Trigger.DTO.DashBoard
{
    public class TeamListForDashboard
    {
        public int YearId { get; set; }
        public int ManagerId { get; set; }
        public int TeamId { get; set; }
        public string Team { get; set; }
        public int TeamType { get; set; }
    }

    public class TeamAvgScoreModel
    {
        public int TeamAvgscore { get; set; }
        public string TeamAvgscoreRank { get; set; }
    }

    public class TeamAvgScoreByDayModel
    {
        public int AvgScoreByDay { get; set; }
        public string AvgScoreByDayRank { get; set; }
    }
}
