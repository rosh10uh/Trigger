using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Trigger.DAL.Shared
{
	/// <summary>
	/// General class to provide collection of config values
	/// </summary>
	public static class Dictionary
    {
        private static Dictionary<string, string> _configDictionary = new Dictionary<string, string>();
        public static Dictionary<string, string> ConfigDictionary { get { return _configDictionary; }}

        //keys to acces appsettings values from dictionary 
        public enum ConfigurationKeys
        {
            ConnectionString,
            TenantConnectionString,
            Passwordchar,
            SmtpServer,
            SmtpPort,
            SenderEmailID,
            ErrorEmailID,
            SenderPassword,
            TriggerSupportEMail,
            PicFolder,
            CSVTemplate,
            TriggerLogo,
            HubConnectionString,
            HubConnection,
            DBServerName,
            DBUserName,
            IndexDB,
            DBPassword,
            BlobContainer,
            ElasticPool,
            StorageAccountName,
            StorageAccountAccessKey,
            StorageAccountURL,
            LandingURL,
            AuthUrl,
            MailScheduleTime,
            CountryCallingCodeURL
        }

        //method to add appsettings values to dictionary with its keys
        public static void AddConfigValues(IConfiguration configuration)
        {
            _configDictionary.Add(ConfigurationKeys.ConnectionString.ToString(), configuration.GetSection("DBConnectionPools").GetSection("DefaultConnection").GetSection("connectionString").Value);
            _configDictionary.Add(ConfigurationKeys.TenantConnectionString.ToString(), configuration.GetSection("DBConnectionPools").GetSection("TenantConnectionString").GetSection("connectionString").Value);

            string environmentName = "AppSettings";
          
            _configDictionary.Add(ConfigurationKeys.Passwordchar.ToString(), configuration.GetSection(environmentName).GetSection("pwdchar").Value);
            _configDictionary.Add(ConfigurationKeys.SmtpServer.ToString(), configuration.GetSection(environmentName).GetSection("SmtpServer").Value);
            _configDictionary.Add(ConfigurationKeys.SmtpPort.ToString(), configuration.GetSection(environmentName).GetSection("SmtpPort").Value);
            _configDictionary.Add(ConfigurationKeys.SenderEmailID.ToString(), configuration.GetSection(environmentName).GetSection("SenderEmailID").Value);
            _configDictionary.Add(ConfigurationKeys.ErrorEmailID.ToString(), configuration.GetSection(environmentName).GetSection("ErrorEmailID").Value);
            _configDictionary.Add(ConfigurationKeys.SenderPassword.ToString(), configuration.GetSection(environmentName).GetSection("SenderPassword").Value);
            _configDictionary.Add(ConfigurationKeys.TriggerSupportEMail.ToString(), configuration.GetSection(environmentName).GetSection("TriggerSupportEMail").Value);
            _configDictionary.Add(ConfigurationKeys.PicFolder.ToString(), configuration.GetSection(environmentName).GetSection("PicFolder").Value);
            _configDictionary.Add(ConfigurationKeys.CSVTemplate.ToString(), configuration.GetSection(environmentName).GetSection("CSVTemplate").Value);
            _configDictionary.Add(ConfigurationKeys.TriggerLogo.ToString(), configuration.GetSection(environmentName).GetSection("TriggerLogo").Value);
            _configDictionary.Add(ConfigurationKeys.HubConnectionString.ToString(), configuration.GetSection(environmentName).GetSection("nHubConnectionString").Value);
            _configDictionary.Add(ConfigurationKeys.HubConnection.ToString(), configuration.GetSection(environmentName).GetSection("nHubConnection").Value);
            _configDictionary.Add(ConfigurationKeys.DBServerName.ToString(), configuration.GetSection(environmentName).GetSection("DBServerName").Value);
            _configDictionary.Add(ConfigurationKeys.DBUserName.ToString(), configuration.GetSection(environmentName).GetSection("DBUserName").Value);
            _configDictionary.Add(ConfigurationKeys.IndexDB.ToString(), configuration.GetSection(environmentName).GetSection("IndexDB").Value);
            _configDictionary.Add(ConfigurationKeys.DBPassword.ToString(), configuration.GetSection(environmentName).GetSection("DBPassword").Value);
            _configDictionary.Add(ConfigurationKeys.BlobContainer.ToString(), configuration.GetSection(environmentName).GetSection("BlobContainer").Value);
            _configDictionary.Add(ConfigurationKeys.ElasticPool.ToString(), configuration.GetSection(environmentName).GetSection("ElasticPool").Value);
            _configDictionary.Add(ConfigurationKeys.StorageAccountName.ToString(), configuration.GetSection(environmentName).GetSection("StorageAccountName").Value);
            _configDictionary.Add(ConfigurationKeys.StorageAccountAccessKey.ToString(), configuration.GetSection(environmentName).GetSection("StorageAccountAccessKey").Value);
            _configDictionary.Add(ConfigurationKeys.StorageAccountURL.ToString(), configuration.GetSection(environmentName).GetSection("StorageAccountURL").Value);
            _configDictionary.Add(ConfigurationKeys.LandingURL.ToString(), configuration.GetSection(environmentName).GetSection("LandingURL").Value);
            _configDictionary.Add(ConfigurationKeys.AuthUrl.ToString(), configuration.GetSection(environmentName).GetSection("AuthUrl").Value);
            _configDictionary.Add(ConfigurationKeys.MailScheduleTime.ToString(), configuration.GetSection(environmentName).GetSection("MailScheduleTime").Value);
            _configDictionary.Add(ConfigurationKeys.CountryCallingCodeURL.ToString(), configuration.GetSection(environmentName).GetSection("CountryCallingCodeURL").Value); 
           
        }
        
    }
}
