using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.DTO.EmailTemplate;
using Trigger.Utility;
namespace Trigger.BLL.ContactUs
{
    public class ContactUs
    {
        private readonly ILogger<ContactUs> _logger;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly IClaims _claims;
        private readonly AppSettings _appSettings;

        public ContactUs(TriggerCatalogContext catalogDbContext, IClaims claims, ILogger<ContactUs> logger, IOptions<AppSettings> appSettings)
        {
            _catalogDbContext = catalogDbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _claims = claims;
        }



        /// <summary>
        /// Insert Contact us details and send support mail to support team
        /// </summary>
        /// <param name="contactDetails">contact Details</param>
        /// <returns>custome JSON object</returns>
        public async Task<CustomJsonData> InvokeContactUsSupport(ContactDetails contactDetails)
        {
            try
            {
                int copmanyId = Convert.ToInt32(_claims["CompId"].Value);
                SendEmailToUser(contactDetails, copmanyId);
                SendEmailToSupportTeam(contactDetails, copmanyId);
                return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.contactRequestSent));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError));
            }
        }

        /// <summary>
        /// Send email to support team
        /// </summary>
        /// <param name="contactUs">ContactUs model</param>
        private void SendEmailToSupportTeam(ContactDetails contactUs, int companyId)
        {
            try
            {
                string emailSubject = Messages.enquiry;
                EmailTemplateDetails emailTemplateDetails = new EmailTemplateDetails { templateName = Messages.contactUsEmailTemplate, companyId = companyId };
                string emailBody = _catalogDbContext.EmailTemplateRepository.GetEmailTemplateByName(emailTemplateDetails);

                emailBody = emailBody.Replace(Messages.contactUsFullName, contactUs.fullName);
                emailBody = emailBody.Replace(Messages.contactUsEmail, contactUs.email);
                emailBody = emailBody.Replace(Messages.contactUsSubject, contactUs.subject);
                emailBody = emailBody.Replace(Messages.contactUsComments, contactUs.comments.Replace("\n", "<br/>"));

                EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), _appSettings.TriggerSupportEMail, emailSubject, emailBody, string.Empty, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Send thank you email to Requester 
        /// </summary>
        /// <param name="contactUs">ContactUs model</param>
        private void SendEmailToUser(ContactDetails contactUs, int companyId)
        {
            try
            {
                string emailSubject = Messages.enquiry;
                EmailTemplateDetails emailTemplateDetails = new EmailTemplateDetails { templateName = Messages.ContactUsFeedBackEmailTemplate, companyId = companyId };
                string thankYouEmailBody = _catalogDbContext.EmailTemplateRepository.GetEmailTemplateByName(emailTemplateDetails);
                thankYouEmailBody = thankYouEmailBody.Replace(Messages.contactUsFullName, contactUs.fullName);
                EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), contactUs.email, emailSubject, thankYouEmailBody, string.Empty, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }
    }
}
