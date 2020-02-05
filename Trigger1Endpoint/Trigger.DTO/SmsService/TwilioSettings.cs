namespace Trigger.DTO.SmsService
{
    /// <summary>
    /// Class Name   :   TwilioSettings
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   24 Apr 2019
    /// Purpose      :   DTO class to set settings of Twilio Account to send SMS
    /// Revision     : 
    /// </summary>
    public class TwilioSettings
    {
        public string Sid { get; set; }

        public string Token { get; set; }

        public string From { get; set; }
    }
}
