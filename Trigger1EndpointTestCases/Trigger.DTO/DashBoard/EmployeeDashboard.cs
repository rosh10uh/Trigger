using System;

namespace Trigger.DTO
{
    public class EmployeeDashboardModel
    {
        public int empId { get; set; }
        public int companyId { get; set; }
		public decimal lastScore { get; set; }
        public string lastRank { get; set; }
        public int noOfCount { get; set; }
        public int lyrNoOfCount { get; set; }
        public int currAvgScore { get; set; }
        public string currAvgScoreRank { get; set; }

        public decimal lyrAvgScore { get; set; }
        public string lyrAvgScoreRank { get; set; }

        public string lastAssessedDate { get; set; }
        public string lastScoreRemarks { get; set; }
        public string lastManagerAction { get; set; }
        public string lastScoreSummary { get; set; }
        public string lastGeneralScoreRank { get; set; }

        public string empName { get; set; }
        public string category { get; set; }
        public string remark { get; set; }
        public string generalRemark { get; set; }
        public int status { get; set; }
        public int generalStatus { get; set; }
        public DateTime assessmentDate { get; set; }
        public DateTime GeneralUpddtStamp { get; set; }
        public DateTime KpiUpddtStamp { get; set; }
        public string generalDocPath { get; set; }
        public string documentPath { get; set; }
        public int assessmentId { get; set; }
        public int remarkId { get; set; }

        public int assessmentById { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string assessmentByImgPath { get; set; }
        public string monYrId { get; set; }
        public string monYr { get; set; }
        public decimal dayScore { get; set; }
        public string dayScoreRank { get; set; }
        public decimal monthAvgScore { get; set; }
        public string monthAvgScoreRank { get; set; }

        public string weekNo { get; set; }
        public int wkNo { get; set; }
        public decimal weekAvgScore { get; set; }
        public string weekAvgScoreRank { get; set; }
        public string year { get; set; }
        public int yearId { get; set; }
        public decimal yearAvgScore { get; set; }
        public string yeatAvgScoreRank { get; set; }

        public string GeneralCloudFilePath { get; set; }
        public string CloudFilePath { get; set; }
    }
}
