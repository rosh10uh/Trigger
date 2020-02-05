using Newtonsoft.Json;
using System.Collections.Generic;

namespace Trigger.DTO
{
    public class ManagerDashBoardModel
    {
        public int companyId { get; set; }
        public int managerId { get; set; }
        public string deparmentList { get; set; }
        public int yearId { get; set; }

        public int directRptCnt { get; set; }
        public int directRptAvgScore { get; set; }
        public string directRptAvgScoreRank { get; set; }
        public int orgRptCnt { get; set; }
        public int orgRptAvgScore { get; set; }
        public string orgRptAvgScoreRank { get; set; }
        public List<GraphDirectRptPct> lstGraphDirectRptPct { get; set; }
        public List<GraphDirectRptRank> lstGraphDirectRptRank { get; set; }
        public List<GraphOrgRptPct> lstGraphOrgRptPct { get; set; }
        public List<GraphOrgRptRank> lstGraphOrgRptRank { get; set; }
        public List<GraphTodayDirectRpt> lstGraphTodayDirectRpt { get; set; }
        public List<GraphTodayOrgRpt> lstGraphTodayOrgRpt { get; set; }
        public int UnApprovedSparkCount { get; set; }
        public int TeamListCount { get; set; }
        public ManagerDashBoardModel()
        {
            lstGraphDirectRptPct = new List<GraphDirectRptPct>();
            lstGraphDirectRptRank = new List<GraphDirectRptRank>();
            lstGraphOrgRptPct = new List<GraphOrgRptPct>();
            lstGraphOrgRptRank = new List<GraphOrgRptRank>();
            lstGraphTodayDirectRpt = new List<GraphTodayDirectRpt>();
            lstGraphTodayOrgRpt = new List<GraphTodayOrgRpt>();
        }

    }

    public class GraphDirectRptPct
    {
        public int directMonYrId { get; set; }
        public string directMonYr { get; set; }
        public int directRptEmpCnt { get; set; }
        public string directScoreRank { get; set; }
        public int directRptEmpPct { get; set; }
    }
    public class GraphDirectRptRank
    {
        public int directAvgMonYrId { get; set; }
        public string directAvgMonYr { get; set; }
        public int directRptAvgScore { get; set; }
        public string directAvgScoreRank { get; set; }

    }
    public class GraphOrgRptPct
    {
        public int orgMonYrId { get; set; }
        public string orgMonYr { get; set; }
        public int orgRptEmpCnt { get; set; }
        public string orgScoreRank { get; set; }
        public int orgRptEmpPct { get; set; }
    }
    public class GraphOrgRptRank
    {
        public int orgAvgMonYrId { get; set; }
        public string orgAvgMonYr { get; set; }
        public int orgRptAvgScore { get; set; }
        public string orgAvgScoreRank { get; set; }
    }
    public class GraphTodayDirectRpt
    {
        [JsonProperty("TodayDirectRptCnt")]
        public int TodayDirectRptCnt { get; set; }
        [JsonProperty("TodayRptEmpList")]
        public string TodayRptEmpList { get; set; }
        [JsonProperty("TodayDirectRptRank")]
        public string TodayDirectRptRank { get; set; }
    }
    public class GraphTodayOrgRpt
    {
        [JsonProperty("TodayOrgRptCnt")]
        public int TodayOrgRptCnt { get; set; }
        [JsonProperty("TodayOrgEmpList")]
        public string TodayOrgEmpList { get; set; }
        [JsonProperty("TodayOrgRptRank")]
        public string TodayOrgRptRank { get; set; }
    }

    public class MngrDashboard
    {
        public int CntDirectEmps { get; set; }
        public decimal DirectRptAvgScore { get; set; }
        public string DirectRptAvgScoreRank { get; set; }
        public int CntOrgEmps { get; set; }
        public decimal OrgRptAvgScore { get; set; }
        public string OrgRptAvgScoreRank { get; set; }

        public int grphDrctRpt { get; set; }
        public string DrctScoreRank { get; set; }
        public string DrctmonYrId { get; set; }
        public string DrctmonYr { get; set; }
        public int TotDirctEmps { get; set; }
        public decimal DrctPct { get; set; }

        public int grphOrgRpt { get; set; }
        public string OrgRank { get; set; }
        public string OrgMonYrId { get; set; }
        public string OrgMonYr { get; set; }
        public int TotOrgEmps { get; set; }
        public decimal OrgPct { get; set; }
        public decimal DrctAvgScore { get; set; }
        public string AvgDrctRank { get; set; }

        public string DrctAvgMonYrId { get; set; }
        public string DrctAvgMonYr { get; set; }
        public decimal OrgAvgScore { get; set; }
        public string AvgOrgRank { get; set; }

        public string OrgAvgMonYrId { get; set; }
        public string OrgAvgMonYr { get; set; }
        public int TodayRptEmpCnt { get; set; }
        public string TodayRptEmpRank { get; set; }
        public int TodayOrgEmpCnt { get; set; }
        public string TodayOrgEmpRank { get; set; }
        public string TodayRptEmpList { get; set; }
        public string TodayOrgEmpList { get; set; }
    }
    public class AssessmentYearModel
    {
        [JsonProperty("AssessedYear")]
        public string AssessedYear { get; set; }
        public int CompanyId { get; set; }
        public int ManagerId { get; set; }
    }
}
