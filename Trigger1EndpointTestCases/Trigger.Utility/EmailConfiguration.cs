using System.Net;
using System.Net.Mail;
using Trigger.DTO;

namespace Trigger.Utility
{
    public static class EmailConfiguration 
    {
        public static void SendMail(string fromMailId, string password, string mailTo, string subject, string message, string companyName, string clientHost, int portNo)
        {
            var mailClient = new SmtpClient(clientHost, portNo)
            {
                Credentials = new NetworkCredential(fromMailId, password),
                EnableSsl = true
            };
            mailClient.Send(fromMailId, mailTo, subject, message);
        }

        public static void SendMail(string fromMailId, string password, string clientHost, int portNo, string mailTo, string subject, string message, string companyName, bool IsBodyHtml)
        {
            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.IsBodyHtml = IsBodyHtml;
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            mailMessage.From = new MailAddress(fromMailId);
            mailMessage.To.Add(mailTo);

            var mailClient = new SmtpClient(clientHost, portNo)
            {
                Credentials = new NetworkCredential(fromMailId, password),
                EnableSsl = true
            };
            mailClient.Send(mailMessage);
        }

        public static void SendMail(EmailDetails emailDetails)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = emailDetails.IsBodyHtml;
            mailMessage.Body = emailDetails.Message;
            mailMessage.Subject = emailDetails.Subject;
            mailMessage.From = new MailAddress(emailDetails.FromEmailId);
            mailMessage.To.Add(emailDetails.MailTo);

            var mailClient = new SmtpClient(emailDetails.ClientHost, emailDetails.PortNo)
            {
                Credentials = new NetworkCredential(emailDetails.FromEmailId, emailDetails.Password),
                EnableSsl = true
            };
            mailClient.Send(mailMessage);
        }



    }
}
