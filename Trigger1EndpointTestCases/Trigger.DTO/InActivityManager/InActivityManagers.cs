namespace Trigger.DTO.InActivityManager
{
    /// <summary>
    /// Class Name   :   InActivityReminder
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   30 Apr 2019
    /// Purpose      :   DTO class for properties require InActivityReminder
    /// Revision     : 
    /// </summary>
    public class InActivityManagers
    {
        public int companyId { get; set; }
        public int empId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string smsText { get; set; }
        public string emailText { get; set; }
        public string phoneNumber { get; set; }
        public int inActivityDays { get; set; }
        public int createdBy { get; set; }
        public bool isMailSent { get; set; }
        public bool phoneConfirmed { get; set; }
        public bool optForSms { get; set; }

        public int result { get; set; }
    }
}
