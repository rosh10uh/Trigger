using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DAL.Shared;
using Trigger.DTO.EmailTemplate;

namespace Trigger.DAL.EmailTemplate
{
    [QueryPath("Trigger.DAL.Query.EmailTemplate.EmailTemplate")]
    public class EmailTemplateRepository : DaoRepository<EmailTemplateDetails>
    {
        public virtual string GetEmailTemplateByName(EmailTemplateDetails emailTemplateDetails)
        {
            return ExecuteQuery<string>(emailTemplateDetails, SPFileName.InvokeGetTemplateByName);
        }
    }
}
