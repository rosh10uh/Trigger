using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OneRPP.Restful.Contracts.Enum;
using OneRPP.Restful.Contracts.Resource;
using OneRPP.Restful.Contracts.Services;
using OneRPP.Restful.DAO;
using OneRPP.Restful.DAO.Interface;
using OneRPP.Restful.DAO.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.Shared;
using Trigger.DTO;
using TestFakeAppSettings = Trigger.Test.Shared.FakeAppSettingsModel;
using SharedClaims = Trigger.DAL.Shared.Claims;
using Trigger.Utility;

namespace Trigger.Test.Shared
{
    public class SetupTest
    {
        private readonly Mock<IExecuteData> _executeData;
        private readonly Mock<IFindDaoRepository> _findDaoRepository;
        private readonly Mock<IUowConnectionManager> _uowConnectionManager;
        private readonly Mock<ILogger<DaoContext>> _logger;
        private readonly Mock<IDaoRepositoryInitializer> _daoRepositoryInitializer;
        private readonly Mock<IConfiguration> _iConfiguration;
        private readonly Mock<IConfigurationSection> _iConfigurationSection;
        public SetupTest()
        {
            var mockRepository = new MockRepository(MockBehavior.Default);

            _executeData = mockRepository.Create<IExecuteData>();
            _findDaoRepository = mockRepository.Create<IFindDaoRepository>();
            _uowConnectionManager = mockRepository.Create<IUowConnectionManager>();
            _logger = mockRepository.Create<ILogger<DaoContext>>();
            _daoRepositoryInitializer = mockRepository.Create<IDaoRepositoryInitializer>();
            _iConfiguration = mockRepository.Create<IConfiguration>();
            _iConfigurationSection = mockRepository.Create<IConfigurationSection>();
            _iConfiguration = SetDictionaryConfiguration(_iConfiguration);
        }

        public ServiceProvider Setup(bool hasDictionary = false)
        {
            if (hasDictionary && Dictionary.ConfigDictionary.Count == 0)
                Dictionary.AddConfigValues(_iConfiguration.Object);

            //Arrange
            _executeData.Setup(x => x.ExecuteQuery<FakeDaoModel, FakeDaoModel>(It.IsAny<FakeDaoModel>(),
                    It.IsAny<string>(), It.IsAny<DbEngine>(), It.IsAny<IDbConnection>()))
                .Returns(new FakeDaoModel
                {
                    Id = 1
                });

            _executeData.Setup(x => x.ExecuteQuery<FakeDaoModel, FakeDaoModel>(It.IsAny<FakeDaoModel>(),
                    It.IsAny<string>(), It.IsAny<DbEngine>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .Returns(new FakeDaoModel
                {
                    Id = 1
                });

            _uowConnectionManager.Setup(x => x.DbConnectionModel).Returns(new DbConnectionModel(It.IsAny<IDbConnection>(), It.IsAny<DbEngine>()));

            _findDaoRepository.Setup(x => x.FindRepositories(It.IsAny<DaoContext>()))
                .Returns(new List<DaoRepositoryModel>
                {
                    new DaoRepositoryModel{Name = "FakeDaoModel", Type = typeof(MockDaoContext).GetProperty("FakeDaoModel") }
                });


            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(x => _executeData.Object);
            serviceCollection.AddTransient(x => _daoRepositoryInitializer.Object);
            serviceCollection.AddTransient(x => _findDaoRepository.Object);
            serviceCollection.AddTransient(x => _uowConnectionManager.Object);
            serviceCollection.AddTransient(x => _logger.Object);
            return serviceCollection.BuildServiceProvider();
        }

        public Mock<SharedClaims> SetupClaims()
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim("email", "abc@abc.com"));
            identity.AddClaim(new Claim("client_id", "123"));
            identity.AddClaim(new Claim("RoleId", "1"));
            identity.AddClaim(new Claim("role", "123m"));
            identity.AddClaim(new Claim("sub", "123"));
            identity.AddClaim(new Claim("compid", "123"));
            identity.AddClaim(new Claim("Key", "ABC123"));
            identity.AddClaim(new Claim("EmpId", "12356")); 

            var iContext = new Mock<IContext>();
            iContext.Setup(x => x.reqobj.HttpContext.User.Claims).Returns(identity.Claims);

            return new Mock<SharedClaims>(iContext.Object);

        }

        public class MockDaoContext : DaoContext
        {
            public MockDaoContext(IServiceProvider serviceProvider) : base(serviceProvider, "") { }
        }

        public Mock<IEnvironmentVariables> SetUpEnvirementVariable()
        {
            Dictionary<string, string> envirementVariable = new Dictionary<string, string>();
            envirementVariable.Add("QueryFolderPath", @"D:\\abc\query");
            var iEnvironmentVariables = new Mock<IEnvironmentVariables>();
            iEnvironmentVariables.Setup(x => x.EnvironmentVariables).Returns(envirementVariable);

            return iEnvironmentVariables;
        }

        public class FakeDaoModel
        {
            public int Id { get; set; }
        }

        private Mock<IConfiguration> SetDictionaryConfiguration(Mock<IConfiguration> iConfiguration)
        {
            string environmentName = "AppSettings";
            AppSettings appSettings = TestFakeAppSettings.GetAppSettingsIOption().Value;
            AuthorizationOptions authorizationOptions = TestFakeAppSettings.GetAuthorization().Value;
            SmsSettings smsSettings = TestFakeAppSettings.GetSmsSettings().Value;

            iConfiguration.Setup(x => x.GetSection("DBConnectionPools").GetSection("DefaultConnection").GetSection("connectionString").Value).Returns(appSettings.DefaultConnection.ConnectionString);
            iConfiguration.Setup(x => x.GetSection("DBConnectionPools").GetSection("TenantConnectionString").GetSection("connectionString").Value).Returns(appSettings.TenantConnectionString.ConnectionString);
            iConfiguration.Setup(x => x.GetSection("Authorization").GetSection("Options").GetSection("ApiName").Value).Returns(authorizationOptions.ApiName);
            iConfiguration.Setup(x => x.GetSection("Authorization").GetSection("Options").GetSection("ApiSecret").Value).Returns(authorizationOptions.ApiSecret);
            iConfiguration.Setup(x => x.GetSection("Authorization").GetSection("Options").GetSection("RequireHttpsMetadata").Value).Returns(authorizationOptions.RequireHttpsMetadata.ToString());
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("pwdchar").Value).Returns(appSettings.PwdChar);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("SmtpServer").Value).Returns(appSettings.SmtpServer);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("SmtpPort").Value).Returns(appSettings.SmtpPort);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("SenderEmailID").Value).Returns(appSettings.SenderEmailID);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("ErrorEmailID").Value).Returns(appSettings.ErrorEmailID);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("SenderPassword").Value).Returns(appSettings.SenderPassword);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("TriggerSupportEMail").Value).Returns(appSettings.TriggerSupportEMail);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("PicFolder").Value).Returns(appSettings.PicFolder);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("CSVTemplate").Value).Returns(appSettings.CSVTemplate);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("TriggerLogo").Value).Returns(appSettings.TriggerLogo);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("nHubConnectionString").Value).Returns(appSettings.nHubConnectionString);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("nHubConnection").Value).Returns(appSettings.nHubConnection);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("DBServerName").Value).Returns(appSettings.DBServerName);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("DBUserName").Value).Returns(appSettings.DBUserName);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("IndexDB").Value).Returns(appSettings.IndexDB);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("DBPassword").Value).Returns(appSettings.DBPassword);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("BlobContainer").Value).Returns(appSettings.BlobContainer);

            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("ElasticPool").Value).Returns(appSettings.ElasticPool);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("StorageAccountName").Value).Returns(appSettings.StorageAccountName);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("StorageAccountAccessKey").Value).Returns(appSettings.StorageAccountAccessKey);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("StorageAccountURL").Value).Returns(appSettings.StorageAccountURL);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("LandingURL").Value).Returns(appSettings.LandingURL);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("AuthUrl").Value).Returns(appSettings.AuthUrl);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("MailScheduleTime").Value).Returns(appSettings.MailScheduleTime);
            iConfiguration.Setup(x => x.GetSection(environmentName).GetSection("CountryCallingCodeURL").Value).Returns(appSettings.CountryCallingCodeURL);

            iConfiguration.Setup(x => x.GetSection("Subscribers").GetSection("URL").Value).Returns(TestFakeAppSettings.Subscribers.URL);
            iConfiguration.Setup(x => x.GetSection("Subscribers").GetSection("UserAgent").Value).Returns(TestFakeAppSettings.Subscribers.UserAgent);
            iConfiguration.Setup(x => x.GetSection("Subscribers").GetSection("Athorization").Value).Returns(TestFakeAppSettings.Subscribers.Athorization);

            iConfiguration.Setup(x => x.GetSection("SMSSettings").GetSection("VerificationCodeMinSize").Value).Returns(smsSettings.VerificationCodeMinSize.ToString());
            iConfiguration.Setup(x => x.GetSection("SMSSettings").GetSection("VerificationCodeMaxSize").Value).Returns(smsSettings.VerificationCodeMaxSize.ToString());
            iConfiguration.Setup(x => x.GetSection("SMSSettings").GetSection("VerificationCodeDigits").Value).Returns(smsSettings.VerificationCodeDigits.ToString());
            iConfiguration.Setup(x => x.GetSection("SMSSettings").GetSection("VerificationCodeTimeOut").Value).Returns(smsSettings.VerificationCodeTimeOut.ToString());

            return iConfiguration;
        }


        public void DeleteFileFromBlobStorage(string fileName)
        {
           
        }
    }
}
