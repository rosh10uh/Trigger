using System;

namespace Trigger.DTO
{
    public class AssessmentScoreModel
    {
        public int AssessmentId { get; set; }
        public int empId { get; set; }
        public int companyId { get; set; }
        public string empName { get; set; }
        public int empScore { get; set; }
        public string empScoreRank { get; set; }
        public DateTime ratingDate { get; set; }
        public string assessmentPeriod { get; set; }
        public int assessmentById { get; set; }
        public string assessmentBy { get; set; }
        public string generalScoreRank { get; set; }
        public string scoreRemarks { get; set; }
        public string managerAction { get; set; }
        public string scoreSummary { get; set; }
        public bool ScoreFeedback { get; set; }
        public int ExpectedScoreId { get; set; }
        public int Result { get; set; }
        public string FeedbackRemark { get; set; }
    }
}
