using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OneRPP.Restful.Contracts.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Company;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Utility;
using Xunit;
using BLL_Company = Trigger.BLL.Company.Company;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Company
{
    public class CompanyTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;

        //Company constructor paerameter
        private readonly Mock<TriggerCatalogContext> _catalogContext;
        private readonly Mock<CompanyContext> _companyContext;
        private readonly Mock<BackgroundJobRequest> _backgroundJobRequest;
        private readonly Mock<IHttpContextAccessor> _iHttpContextAccessor;
        private readonly Mock<ILogger<BLL_Company>> _logger;
        private readonly IOptions<AppSettings> _appSettings;

        // company context constructo parameter
        private readonly Mock<IEnvironmentVariables> _iEnvironmentVariables;
        private readonly Mock<ILogger<CompanyContext>> _loggerCompanyContext;
        private readonly Mock<TriggerResourceReader> _triggerResourceReader;
        private readonly Mock<EmployeeContext> _employeeContext;

        //BackgroundJobRequest
        private readonly Mock<ILogger<BackgroundJobRequest>> _loggerbackgroundJobRequest;
        private readonly Mock<ISmsSender> _iSmsSender;

        //TriggerResourceReader construcor parameter
        private readonly Mock<OneRPP.Restful.Contracts.Utility.IResourceReader> _iResourceReader;

        //EmployeeContext constructor parametere
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerContext> _triggerContext;

        public CompanyTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup(true);
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();

            //EmployeeContext
            _catalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _employeeContext = new Mock<EmployeeContext>(_iConnectionContext.Object, _catalogContext.Object);

            //TriggerResourceReader
            _iResourceReader = new Mock<OneRPP.Restful.Contracts.Utility.IResourceReader>();
            _triggerResourceReader = new Mock<TriggerResourceReader>(_iResourceReader.Object);

            //BackgroundJobRequest
            _loggerbackgroundJobRequest = new Mock<ILogger<BackgroundJobRequest>>();
            _iSmsSender = new Mock<ISmsSender>();
            _iHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _backgroundJobRequest = new Mock<BackgroundJobRequest>(_loggerbackgroundJobRequest.Object, _iHttpContextAccessor.Object, _employeeContext.Object, _iSmsSender.Object,_appSettings);
                
            //companyContext
            _loggerCompanyContext = new Mock<ILogger<CompanyContext>>();
            _iEnvironmentVariables = new Mock<IEnvironmentVariables>();
            _companyContext = new Mock<CompanyContext>(_catalogContext.Object, _iEnvironmentVariables.Object, _loggerCompanyContext.Object, _triggerResourceReader.Object, _employeeContext.Object,_appSettings);

            //Company
            _logger = new Mock<ILogger<BLL_Company>>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_SelectAllAsync_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _catalogContext.Setup(x => x.CompanyRepository.SelectAll()).Returns(TestUtility.GetCompanyDetailsModels());
            }
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object,_appSettings);

            //Act
            var result = company.SelectAllAsync();

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_SelectAsync_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _catalogContext.Setup(x => x.CompanyRepository.Select<List<CompanyDetailsModel>>(It.IsAny<CompanyDetailsModel>())).Returns(TestUtility.GetCompanyDetailsModels());
            }
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);

            //Act
            var result = company.SelectAsync(It.IsAny<CompanyDetailsModel>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_InsertAsync_WithExpectedResult(int expectedResult)
        {
            //Arrange
            var companyDetailsModel = TestUtility.GetCompanyDetailsModel();
            companyDetailsModel.result = expectedResult;
            _companyContext.Setup(x => x.InsertCompany(It.IsAny<CompanyDetailsModel>())).Returns(companyDetailsModel);
            _backgroundJobRequest.Setup(x => x.ScheduleInActivityReminder());
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);

            //Act
            var result = company.InsertAsync(TestUtility.GetCompanyDetailsModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_InsertAsync_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _companyContext.Setup(x => x.InsertCompany(It.IsAny<CompanyDetailsModel>())).Returns(TestUtility.GetCompanyDetailsModel());
                _backgroundJobRequest.Setup(x => x.ScheduleInActivityReminder());
            }
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);

            //Act
            var result = company.InsertAsync(TestUtility.GetCompanyDetailsModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void To_Verify_UpdateAsync_WithExpectedResult(int expectedResult)
        {
            //Arrange
            var companyDetailModel = TestUtility.GetCompanyDetailsModel();
            companyDetailModel.result = expectedResult;

            _catalogContext.Setup(x => x.CompanyRepository.Update(It.IsAny<CompanyDetailsModel>())).Returns(companyDetailModel);
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);

            //Act
            var result = company.UpdateAsync(TestUtility.GetCompanyDetailsModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_UpdateAsync_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _catalogContext.Setup(x => x.CompanyRepository.Update(It.IsAny<CompanyDetailsModel>())).Returns(TestUtility.GetCompanyDetailsModel());
            }
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);

            //Act
            var result = company.UpdateAsync(TestUtility.GetCompanyDetailsModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_DeleteAsync_WithExpectedResult(int expectedResult)
        {
            //Arrange
            var companyDetailModel = TestUtility.GetCompanyDetailsModel();
            companyDetailModel.result = expectedResult;
            _catalogContext.Setup(x => x.CompanyRepository.Delete(It.IsAny<CompanyDetailsModel>())).Returns(companyDetailModel);
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);

            //Act
            var result = company.DeleteAsync(TestUtility.GetCompanyDetailsModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_DeleteAsync_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _catalogContext.Setup(x => x.CompanyRepository.Delete(It.IsAny<CompanyDetailsModel>())).Returns(TestUtility.GetCompanyDetailsModel());
            }
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);

            //Act
            var result = company.DeleteAsync(TestUtility.GetCompanyDetailsModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddInactivityScheduler_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _backgroundJobRequest.Setup(x => x.ScheduleInActivityReminder());
            }
            var company = new BLL_Company(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);

            //Act
            var result = company.AddInactivityScheduler();

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
