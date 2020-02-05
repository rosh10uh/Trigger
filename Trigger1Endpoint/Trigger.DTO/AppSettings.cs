namespace Trigger.DTO
{
    /// <summary>
    /// Class Name   :   AppSettings
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   23 May 2019
    /// Purpose      :   DTO class for binding values from appsettings config
    /// Revision     : 
    /// </summary>
    public class AppSettings
    {
        public DefaultConnection DefaultConnection { get; set; }
        public TenantConnectionString TenantConnectionString { get; set; }
        public string PwdChar { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }
        public string SenderEmailID { get; set; }
        public string ErrorEmailID { get; set; }
        public string SenderPassword { get; set; }
        public string TriggerSupportEMail { get; set; }
        public string PicFolder { get; set; }
        public string CSVTemplate { get; set; }
        public string TriggerLogo { get; set; }
        public string nHubConnectionString { get; set; }
        public string nHubConnection { get; set; }
        public string DBServerName { get; set; }
        public string DBUserName { get; set; }
        public string IndexDB { get; set; }
        public string DBPassword { get; set; }
        public string BlobContainer { get; set; }
        public string ElasticPool { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccountAccessKey { get; set; }
        public string StorageAccountURL { get; set; }
        public string LandingURL { get; set; }
        public string AuthUrl { get; set; }
        public string MailScheduleTime { get; set; }
        public string CountryCallingCodeURL { get; set; }
    }

    public class DefaultConnection
    {
        public string ConnectionString { get; set; }
    }

    public class TenantConnectionString
    {
        public string ConnectionString { get; set; }
    }

    public class AuthorizationOptions
    {
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
    }

    public class SmsSettings
    {
        public int VerificationCodeMinSize { get; set; }
        public int VerificationCodeMaxSize { get; set; }
        public int VerificationCodeDigits { get; set; }
        public int VerificationCodeTimeOut { get; set; }
        public string VerificationCodeMessage { get; set; }
    }
}
