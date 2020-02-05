using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.Assessment
{
    [QueryPath("Trigger.DAL.Query.Assessment")]
    public class AssessmentScoreRepository : DaoRepository<AssessmentScoreModel>
    {
        private const string invokeGetAssessmentScore = "GetAssessmentScore";
        private const string invokeUpdateAssessmentScoreFeedback = "UpdateAssessmentScoreFeedback";

        public AssessmentScoreModel GetAssessmentScore(AssessmentScoreModel assessmentScoreModel)
        {
            return ExecuteQuery<AssessmentScoreModel>(assessmentScoreModel, invokeGetAssessmentScore);
        }

        public AssessmentScoreModel UpdateAssessmentScoreFeedback(AssessmentScoreModel assessmentScoreModel)
        {
            return ExecuteQuery<AssessmentScoreModel>(assessmentScoreModel, invokeUpdateAssessmentScoreFeedback);
        }
    }
}
