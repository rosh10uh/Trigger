using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Trigger.DTO.SmsService;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Trigger.Utility
{
    /// <summary>
    /// Class Name   :   SmsSender
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   24 Apr 2019
    /// Purpose      :   Class to send verification code using Twilio
    /// Revision     :  
    /// </summary>
    public class SmsSender : ISmsSender
    {
        private readonly TwilioSettings _twilioSettings;
        private readonly ILogger<SmsSender> _iLogger;

        public SmsSender(IOptions<TwilioSettings> twilioSettings, ILogger<SmsSender> iLogger)
        {
            _twilioSettings = twilioSettings.Value;
            _iLogger = iLogger;
        }

        /// <summary>
        /// Method Name  :   SendSmsAsync
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   24 Apr 2019
        /// Purpose      :   Method to send verification code via sms  to user using Twilio
        /// Revision     :  
        /// </summary>
        /// <param name="number"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<int> SendSmsAsync(string number, string message)
        {
            try
            {
                TwilioClient.Init(_twilioSettings.Sid, _twilioSettings.Token);
                await MessageResource.CreateAsync(new PhoneNumber(number),
                        from: new PhoneNumber(_twilioSettings.From),
                     body: message);

                return 1;
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message.ToString());
                return 0;
            }
        }
    }
}
