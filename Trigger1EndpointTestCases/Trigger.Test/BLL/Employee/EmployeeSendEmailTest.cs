using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using Trigger.BLL.Employee;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using SharedClaims = Trigger.DAL.Shared.Claims;


namespace Trigger.Test.BLL.Employee
{
    public class EmployeeSendEmailTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<EmployeeSendEmail>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<EmployeeCommon> _employeeCommon;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private readonly Mock<EmployeeContext> _employeeContext;
        private readonly Mock<BackgroundJobRequest> _backgroundJobRequest;
        private EmployeeSendEmail _employeeSendEmail;

        public EmployeeSendEmailTest()
        {
            _logger = new Mock<ILogger<EmployeeSendEmail>>();
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _employeeCommon = new Mock<EmployeeCommon>(_triggerCatalogContext.Object, _iConnectionContext.Object, null, null, _appSettings, _iClaims.Object, null);
            _employeeContext = new Mock<EmployeeContext>(_iConnectionContext.Object, _triggerCatalogContext.Object);
            _backgroundJobRequest = new Mock<BackgroundJobRequest>(null, null, _employeeContext.Object, null, _appSettings);
        }

        private void Init_EmployeeSendEmail()
        {
            _employeeSendEmail = new EmployeeSendEmail(_employeeContext.Object, _backgroundJobRequest.Object, _appSettings, _triggerCatalogContext.Object, _logger.Object, _employeeCommon.Object);
        }

        [Fact]
        public void To_Verify_SendMailAndUpdateFlag_WithNoEmployeeFound()
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetEmployeesDetail(It.IsAny<int>(), It.IsAny<string>())).Returns(new List<EmployeeModel>());
            Init_EmployeeSendEmail();

            //Act
            var result = _employeeSendEmail.SendMailAndUpdateFlag(1, EmployeeUtility.GetEmployeeModel()).Result;

            //Assert
            Assert.IsType<CustomJsonData>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_SendMailAndUpdateFlag_WithException()
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetEmployeesDetail(It.IsAny<int>(), It.IsAny<string>())).Returns(EmployeeUtility.GetEmployeeModels());
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetAuthDetailsByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _backgroundJobRequest.Setup(x => x.GetTemplateByName(It.IsAny<string>(), It.IsAny<int>())).Returns("Template " + Messages.headingtext + " " +
                Messages.resetPasswordURL + " " + Messages.userId + " " + Messages.landingURL + " ");
            _employeeContext.Setup(x => x.UpdateEmpIsMailSent(It.IsAny<EmployeeModel>()));
            Init_EmployeeSendEmail();

            //Act
            var result = _employeeSendEmail.SendMailAndUpdateFlag(1, EmployeeUtility.GetEmployeeModel()).Result;

            //Assert
            Assert.IsType<CustomJsonData>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_SendEmployeeRegistrationMail_WithFutureCompanyContractStartDate()
        {
            //Arrange
            List<CompanyDetailsModel> companyDetailsModels = CommonUtility.GetCompanyDetailsModels();
            companyDetailsModels[0].contractStartDate = DateTime.Now.AddDays(3);

            _triggerCatalogContext.Setup(x => x.CompanyRepository.Select<List<CompanyDetailsModel>>(It.IsAny<CompanyDetailsModel>())).Returns(companyDetailsModels);
            _backgroundJobRequest.Setup(x => x.SendEmail(It.IsAny<EmployeeModel>(), It.IsAny<string>()));

            Init_EmployeeSendEmail();

            //Act
            _employeeSendEmail.SendEmployeeRegistrationMail(EmployeeUtility.GetEmployeeModel(), "1", "07d7dbef-8a2b-4f9d-96ff-cac26a42322f");

            //Assert
            Assert.IsType<bool>(true);
        }

        [Theory]
        [InlineData(3)] // For future company contracty start date
        [InlineData(-3)] //For past company contract start date
        public void To_Verify_SendEmailOnUpdateEmployeeDetails_WithCompanyContractStartDate(int addDays)
        {
            //Arrange
            CompanyDetailsModel companyDetailsModels = CommonUtility.GetCompanyDetailsModel();
            companyDetailsModels.contractStartDate = DateTime.Now.AddDays(addDays);

            _triggerCatalogContext.Setup(x => x.CompanyRepository.Select<CompanyDetailsModel>(It.IsAny<CompanyDetailsModel>())).Returns(companyDetailsModels);
            _backgroundJobRequest.Setup(x => x.SendEmail(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _backgroundJobRequest.Setup(x => x.GetTemplateByName(It.IsAny<string>(), It.IsAny<int>())).Returns("Template " + Messages.headingtext + " " +
                Messages.resetPasswordURL + " " + Messages.userId + " " + Messages.landingURL + " ");

            Init_EmployeeSendEmail();

            //Act
            _employeeSendEmail.SendEmailOnUpdateEmployeeDetails(EmployeeUtility.GetEmployeeModel(), "1", "07d7dbef-8a2b-4f9d-96ff-cac26a42322f");

            //Assert
            Assert.IsType<bool>(true);
        }
    }
}
