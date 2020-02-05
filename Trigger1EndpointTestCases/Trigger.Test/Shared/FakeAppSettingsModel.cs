using Microsoft.Extensions.Options;
using Trigger.DTO;
namespace Trigger.Test.Shared
{
    public class FakeAppSettingsModel
    {
        //Appsetting
        public static IOptions<AppSettings> GetAppSettingsIOption()
        {
            return Options.Create(new AppSettings
            {
                DefaultConnection = new DefaultConnection { ConnectionString = "Server=tcp:devtrigger.database.windows.net,1433;Initial Catalog=Catalog;Persist Security Info=False;User ID=devadmin;Password=Trigger789@@@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0;" },
                TenantConnectionString = new TenantConnectionString { ConnectionString = "Server=tcp:devtrigger.database.windows.net,1433;Initial Catalog={0};Persist Security Info=False;User ID=devadmin;Password=Trigger789@@@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0;" },
                PwdChar = "admin@123",
                SmtpServer = "smtp.abc.com",
                SmtpPort = "587",
                SenderEmailID = "noreply@yopmail.com",
                ErrorEmailID = "abc@yopmail.com",
                SenderPassword = "admin@123",
                TriggerSupportEMail = "abc@yopmail.com",
                PicFolder = "ProfilePic",
                CSVTemplate = "ExcelTemplate\\ExcelTemplate.xlsx",
                TriggerLogo = "ExcelTemplate\\TriggerLogo.png",
                nHubConnection = "triggeruatnotification",
                DBServerName = "tcp:devtrigger.database.windows.net,1433",
                DBUserName = "devadmin",
                IndexDB = "Catalog",
                DBPassword = "admin@123",
                BlobContainer = "triggersdata",
                ElasticPool = "Triggerqapool",
                StorageAccountName = "tqa",
                StorageAccountAccessKey = "Mi0dlU6HPBxH0dzB+Xiuf22nhNwU1i1IGCppYwcU2zzXmiOPuGw0UtBrrR5dOiadifMag1VnjRigWvo3Zx2hKg==",
                StorageAccountURL = "https://tqa.blob.core.windows.net",
                LandingURL = "https://dev.trigger123.com/",
                AuthUrl = "https://devauth.trigger123.com",
                MailScheduleTime = "07:00",
                CountryCallingCodeURL = "https://restcountries.eu/rest/v2/all"
            });
        }

        //AuthorizationOptions
        public static IOptions<AuthorizationOptions> GetAuthorization()
        {
            return Options.Create(new AuthorizationOptions
            {
                Authority = "https://devauth.trigger123.com/",
                RequireHttpsMetadata = true,
                ApiName = "TriggerApi",
                ApiSecret = "Triggersecretapi"
            });
        }

        //SmsSettings
        public static IOptions<SmsSettings> GetSmsSettings()
        {
            return Options.Create(new SmsSettings
            {
                VerificationCodeMinSize = 100000,
                VerificationCodeMaxSize = 1000000,
                VerificationCodeDigits = 6,
                VerificationCodeTimeOut = 2,
                VerificationCodeMessage = "Trigger Transformation - Your phone number verification code for SMS notification is: {0}, expires in {1} minutes."
            });
        }


        //Subscribers
        public static class Subscribers
        {
            public static string URL = "https://api.getdrip.com/v2/9555213/campaigns/295152656/subscribers";
            public static string UserAgent = "www.trigger123.com";
            public static string Athorization = "Basic ZDI2NDg4YWQ1ZjA5NzA5MzYzNWI0YzJjMWY3ZGJhMTA=";
        }

    }
}
