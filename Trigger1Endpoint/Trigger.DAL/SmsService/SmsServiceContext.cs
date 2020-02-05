using Microsoft.Extensions.Logging;
using Trigger.DAL.Shared.Interfaces;
using Trigger.Utility;

namespace Trigger.DAL.SmsService
{
    /// <summary>
    /// Class Name   :   SmsServiceContext
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   22 Apr 2019
    /// Purpose      :   SmsServiceContext class for execution of DAL methods
    /// Revision     : 
    /// </summary>
    public class SmsServiceContext
    {
        private readonly ISmsSender _smsSender;

        public SmsServiceContext(ILogger<SmsServiceContext> iLogger, IConnectionContext connectionContext, ISmsSender smsSender, TriggerCatalogContext catalogDbContext)
        {
            _smsSender = smsSender;
        }

        /// <summary>
        /// Method Name  :   SendSMSVerificationCode
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   22 Apr 2019
        /// Purpose      :   Method to call smsSender class for sending sms verification code
        /// Revision     : 
        /// </summary>
        /// <param name="smsVerificationCode"></param>
        public void SendSMSVerificationCode(DTO.SmsService.SmsVerificationCode smsVerificationCode)
        {
            _smsSender.SendSmsAsync(smsVerificationCode.phoneNumber, smsVerificationCode.smsCodeMessage);
        }

    }
}
