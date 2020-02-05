using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;

using Trigger.DTO.EmailTemplate;

namespace Trigger.DAL.EmailTemplate
{
    [QueryPath("Trigger.DAL.Query.EmailTemplate.EmailTemplate")]
    public class EmailTemplateRepository : DaoRepository<EmailTemplateDetails>
    {
        private string invokeGetTemplateByName = "GetTemplateByName";

        public string GetEmailTemplateByName(EmailTemplateDetails emailTemplateDetails)
        {
            return ExecuteQuery<string>(emailTemplateDetails, invokeGetTemplateByName);
        }
    }
}
