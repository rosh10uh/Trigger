using Microsoft.AspNetCore.Mvc;
using Trigger.BLL.Spark;
using Trigger.Middleware;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Controller to call api from Twilio account on sms received to Twilio phone number
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class EmployeeSparkSmsController : TwilioController
    {
        private readonly EmployeeSparkSms _employeeSparkSms;

        /// <summary>
        /// Constructor to initialized BLL object for sparksms
        /// </summary>
        /// <param name="employeeSparkSms"></param>
        public EmployeeSparkSmsController(EmployeeSparkSms employeeSparkSms)
        {
            _employeeSparkSms = employeeSparkSms;
        }

        /// <summary>
        /// API to receive sms from twilio on for spark an employee
        /// </summary>
        /// <param name="smsRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ConnectionByPhoneNumber]
        [Consumes("application/x-www-form-urlencoded")]
        public TwiMLResult PostAsync([FromForm]SmsRequest smsRequest)
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message(_employeeSparkSms.SparkBySms(smsRequest.From, smsRequest.Body, smsRequest.SmsSid).Result.ToString());
            return TwiML(messagingResponse );
        }
    }
}