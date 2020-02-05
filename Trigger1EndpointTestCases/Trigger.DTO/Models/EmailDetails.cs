namespace Trigger.DTO
{
    public class EmailDetails
    {
        public string FromEmailId { get; set; }
        public string Password { get; set; }
        public string ClientHost { get; set; }
        public  int PortNo { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string CompanyName { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}