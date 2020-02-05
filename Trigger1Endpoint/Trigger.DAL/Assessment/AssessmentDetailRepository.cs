using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;

namespace Trigger.DAL.Assessment
{
    [QueryPath("Trigger.DAL.Query.AssessmentDetail.AssessmentDetail")]
    public class AssessmentDetailRepository : DaoRepository<List<EmpAssessmentDet>>
    {
        public const string invokeAddAssessmentDetails = "AddAssessmentDetails";
        public const string invokeAddAssessmentDetailsWithAttachment = "AddAssessmentDetailsWithAttachment";
        public const string invokeDeleteAssessmentAttachment = "DeleteAssessmentAttachment";
        public const string invokeDeleteAssessmentComment = "DeleteAssessmentComment";
        public const string invokeUpdateAssessmentComments = "UpdateAssessmentComment";

        public List<EmpAssessmentDet> InsertAssessmentDetails(List<EmpAssessmentDet>  empAssessmentDets)
        {
            return ExecuteQuery<List<EmpAssessmentDet>>(empAssessmentDets, invokeAddAssessmentDetails);
        }

        public List<EmpAssessmentDet> InsertAssessmentDetailsWithAttachment(List<EmpAssessmentDet> empAssessmentDets)
        {
            return ExecuteQuery<List<EmpAssessmentDet>>(empAssessmentDets, invokeAddAssessmentDetailsWithAttachment);
        }

        public List<EmpAssessmentDet> DeleteAssessmentAttachment(List<EmpAssessmentDet> empAssessmentDets)
        {
            return ExecuteQuery<List<EmpAssessmentDet>>(empAssessmentDets, invokeDeleteAssessmentAttachment);
        }

        public List<EmpAssessmentDet> UpdateAssessmentComment(List<EmpAssessmentDet> empAssessmentDets)
        {
            return ExecuteQuery<List<EmpAssessmentDet>>(empAssessmentDets, invokeUpdateAssessmentComments);
        }

        public List<EmpAssessmentDet> DeleteAssessmentComment(List<EmpAssessmentDet> empAssessmentDets)
        {
            return ExecuteQuery<List<EmpAssessmentDet>>(empAssessmentDets, invokeDeleteAssessmentComment);
        }

        
    }
}
