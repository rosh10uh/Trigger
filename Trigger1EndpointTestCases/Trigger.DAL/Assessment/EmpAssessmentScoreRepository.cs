using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.Assessment
{
    [QueryPath("Trigger.DAL.Query.Assessment.Assessment")]
    public class EmpAssessmentScoreRepository : DaoRepository<EmpAssessmentScore>
    {
        public const string invokeUpdateAssessmentScore = "UpdateAssessmentScore";

        public virtual EmpAssessmentScore UpdateScoreRank(EmpAssessmentScore  empAssessmentScore)
        {
            return ExecuteQuery<EmpAssessmentScore>(empAssessmentScore, invokeUpdateAssessmentScore);
        }
    }
}
