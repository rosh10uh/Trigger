using System;
namespace Trigger.Utility
{
    /// <summary>
    /// Class Name   :   VerificationCodeGenerator
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   22 Apr 2019
    /// Purpose      :   to generate verification random number
    /// Revision     : 
    /// </summary>
    public static class VerificationCodeGenerator
    {
        /// <summary>
        /// Method Name  :   GenerateVerificationCode
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   22 Apr 2019
        /// Purpose      :   to generate verification code
        /// Revision     : 
        /// </summary>
        /// <returns>returns 6 digit random int number</returns>
        public static int GenerateVerificationCode(DTO.SmsSettings smsSettings)
        {
            int code;
            Random randomNumber = new Random();
            code = Convert.ToInt32(randomNumber.Next(smsSettings.VerificationCodeMinSize, smsSettings.VerificationCodeMaxSize).ToString("D"+ smsSettings.VerificationCodeDigits.ToString()));

            return code;
        }

    }
}
