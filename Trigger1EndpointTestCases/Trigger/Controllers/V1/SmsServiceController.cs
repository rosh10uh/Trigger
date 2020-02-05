using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.SmsService;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Class Name      :   SmsServiceController
    /// Author          :   Vivek Bhavsar
    /// Creation Date   :   22 Apr 2019
    /// Purpose         :   API to generate and send verification code to user
    /// Revision        :   
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class SmsServiceController : ControllerBase
    {
        private readonly SmsVerificationCode _smsVerificationCode;

        /// <summary>
        /// Name            :   SmsVerificationCode
        /// Author          :   Vivek Bhavsar
        /// Creation Date   :   24 Apr 2019
        /// Purpose         :   constructor for SmsServiceController
        /// Revision        :   
        /// </summary>
        /// <param name="smsVerificationCode"></param>
        public SmsServiceController(SmsVerificationCode smsVerificationCode)
        {
            _smsVerificationCode = smsVerificationCode;
        }

        /// <summary>
        /// This API is used to Generate and Send verification code to user on his registered mobile number
        /// Require Parameters : Email,EmpId,PhoneNumber,CreatedBy
        /// This API will perform 3 steps : 
        /// 1.Generate verification code with 6 digits
        /// 2.Store verification code into database for verification use
        /// 3.Send verification code to registered mobile number
        /// </summary>
        /// <param name="smsVerificationCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> SendSMSVerificationCode([FromBody] DTO.SmsService.SmsVerificationCode smsVerificationCode)
        {
            return await _smsVerificationCode.SendSMSVerificationCode(smsVerificationCode);
        }

        /// <summary>
        /// This API is used to verify sms verification code entered by user
        /// Require Parameters : Email,EmpId,VerificationCode,UpdatedBy
        /// verification code will verify in two ways :
        /// 1.verification code matched with code in database -
        /// 2.Validity of verification code will check e.g. valid for 2 minutes
        /// </summary>
        /// <param name="smsVerificationCode"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> VerifySmsVerificationCode([FromBody] DTO.SmsService.SmsVerificationCode smsVerificationCode)
        {
            return await _smsVerificationCode.VerifySmsVerificationCode(smsVerificationCode);
        }
    }
}