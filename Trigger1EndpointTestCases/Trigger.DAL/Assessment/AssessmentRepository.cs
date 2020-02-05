using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;

namespace Trigger.DAL.Assessment
{
    [QueryPath("Trigger.DAL.Query.Assessment.Assessment")]
    public class AssessmentRepository : DaoRepository<EmpAssessmentModel>
    {
        public const string invokeAddAssessment = "AddAssessment";
        public const string invokeAddAssessmentWithAttachment = "AddAssessmentWithAttachment";
        public const string invokeGetEmpLastAssessmentDetail = "GetEmpLastAssessmentDetail";

        public virtual EmpAssessmentModel InsertAssessment(EmpAssessmentModel  empAssessmentModel)
        {
            return ExecuteQuery<EmpAssessmentModel>(empAssessmentModel, invokeAddAssessment);
        }

        public EmpAssessmentModel InsertAssessmentWithAttachment(EmpAssessmentModel empAssessmentModel)
        {
            return ExecuteQuery<EmpAssessmentModel>(empAssessmentModel, invokeAddAssessmentWithAttachment);
        }

        public List<LstEmpAssessment> GetEmpLastAssessmentDetail()
        {
            return ExecuteQuery<List<LstEmpAssessment>>(null, invokeGetEmpLastAssessmentDetail);
        }
    }
}
