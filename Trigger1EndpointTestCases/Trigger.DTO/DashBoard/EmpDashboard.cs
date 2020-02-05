using System;
using System.Collections.Generic;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO
{
    public class EmpDashboard
    {
        public int empId { get; set; }
        public string empName { get; set; }
        public int noOfRatings { get; set; }
        public int lyrNoOfRatings { get; set; }
        public string lastScoreRank { get; set; }
        public int lastScore { get; set; }
        public string currentYrAvgScoreRank { get; set; }
        public int currentYrAvgScore { get; set; }
        public string lyrAvgScoreRank { get; set; }
        public string lastAssessedDate { get; set; }
        public int lyrAvgScore { get; set; }
        public string lastScoreRemarks { get; set; }
        public string lastManagerAction { get; set; }
        public string lastScoreSummary { get; set; }
        public string lastGeneralScoreRank { get; set; }
        public List<GraphCategories> graphCategories { get; set; }
        public List<Remarks> remarks { get; set; }

        public EmpDashboard()
        {
            graphCategories = new List<GraphCategories>();
            remarks = new List<Remarks>();
        }
    }

    public class GraphCategories
    {
        public List<Weekly> lstWeekly { get; set; }
        public List<Monthly> lstMonthly { get; set; }
        public List<Yearly> lstYearly { get; set; }

        public GraphCategories()
        {
            lstWeekly = new List<Weekly>();
            lstMonthly = new List<Monthly>();
            lstYearly = new List<Yearly>();
        }
    }

    public class Weekly
    {
        public int empid { get; set; }
        public string weekNo { get; set; }
        public int weekScore { get; set; }
        public string weekScoreRank { get; set; }
    }
    public class Monthly
    {
        public int empid { get; set; }
        public string monthNo { get; set; }
        public int monthScore { get; set; }
        public string monthScoreRank { get; set; }
    }
    public class Yearly
    {
        public int empid { get; set; }
        public string yearNo { get; set; }
        public int yearScore { get; set; }
        public string yearScoreRank { get; set; }
    }
    public class Remarks
    {
        public int empid { get; set; }
        public string category { get; set; }
        public string remark { get; set; }
        public int status { get; set; }
        public DateTime assessmentDate { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string assessmentByImgPath { get; set; }
        public string DocumentName { get; set; }
        public int AssessmentId { get; set; }
        public int AssessmentById { get; set; }
        public int RemarkId { get; set; }

        private string _cloudFilePath;
        [RequiredCloudFilePath(ValidationMessage.CloudFilePathRequired, nameof(DocumentName))]
        public string CloudFilePath
        {
            get
            {
                return _cloudFilePath == null ? string.Empty : _cloudFilePath;
            }
            set { _cloudFilePath = value; }
        }
    }
}


