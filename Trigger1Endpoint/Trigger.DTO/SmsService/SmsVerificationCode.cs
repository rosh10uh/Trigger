namespace Trigger.DTO.SmsService
{
    /// <summary>
    /// Class Name   :   SmsVerificationCode
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   22 Apr 2019
    /// Purpose      :   DTO class for properties require for SMS Verification Code
    /// Revision     : 
    /// </summary>
    public class SmsVerificationCode
    {
        public string email { get; set; }
        public string empId { get; set; }
        public int verificationCode { get; set; }
        public int verificationCodeTimeOut { get; set; }
        public string phoneNumber { get; set; }
        public string smsCodeMessage { get; set; }
        public int createdBy { get; set; }
        public int updatedBy { get; set; }
        public int result { get; set; }
    }

    public class SmsCodeVerification
    {
        public bool smsVerified { get; set; }
    }
}
