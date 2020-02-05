using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DTO;

namespace Trigger.DAL.AssessmentYear
{
    [QueryPath("Trigger.DAL.Query.AssessmentYear")]
    public class AssessmentYearRepository : DaoRepository<AssessmentYearModel>
    {
        private const string GetAssessmentYearByCompanyManagerId = "GetAssessmentYear";

        public virtual List<AssessmentYearModel> GetAssessmentYear(AssessmentYearModel assessmentYearModel)
        {
            return ExecuteQuery<List<AssessmentYearModel>>(assessmentYearModel, GetAssessmentYearByCompanyManagerId);
        }
    }
}
