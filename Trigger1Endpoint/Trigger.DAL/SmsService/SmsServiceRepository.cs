using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO.SmsService;

namespace Trigger.DAL.SmsService
{
    /// <summary>
    /// Class Name   :   SmsServiceRepository
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   22 Apr 2019
    /// Purpose      :   Repository for verification code generate/get & save
    /// Revision     : 
    /// </summary>
    [QueryPath("Trigger.DAL.Query.SmsService.SmsService")]
    public class SmsServiceRepository: DaoRepository<SmsVerificationCode>
    {
        public int GetVerificationCodeForUser(DTO.SmsSettings smsSettings)
        {
            return Utility.VerificationCodeGenerator.GenerateVerificationCode(smsSettings);
        }
    }
}
