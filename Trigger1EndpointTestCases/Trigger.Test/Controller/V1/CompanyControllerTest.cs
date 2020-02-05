using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OneRPP.Restful.Contracts.Resource;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
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

namespace Trigger.Test.Controller
{
    public class CompanyControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<BLL_Company> _company;

        //Company constructor paerameter
        private readonly Mock<TriggerCatalogContext> _catalogContext;
        private readonly Mock<CompanyContext> _companyContext;
        private readonly Mock<BackgroundJobRequest> _backgroundJobRequest;
        private readonly Mock<IHttpContextAccessor> _iHttpContextAccessor;
        private readonly Mock<ILogger<BLL_Company>> _logger;

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
        private readonly Mock<ILogger<EmployeeContext>> _loggerEmployeeContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly IOptions<AppSettings> _appSettings;

        public CompanyControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup(true);
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            //EmployeeContext
            _catalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _loggerEmployeeContext = new Mock<ILogger<EmployeeContext>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _employeeContext = new Mock<EmployeeContext>(_iConnectionContext.Object, _catalogContext.Object);

            //TriggerResourceReader
            _iResourceReader = new Mock<OneRPP.Restful.Contracts.Utility.IResourceReader>();
            _triggerResourceReader = new Mock<TriggerResourceReader>(_iResourceReader.Object);

            //BackgroundJobRequest
            _loggerbackgroundJobRequest = new Mock<ILogger<BackgroundJobRequest>>();
            _iSmsSender = new Mock<ISmsSender>();
            _iHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _backgroundJobRequest = new Mock<BackgroundJobRequest>(_loggerbackgroundJobRequest.Object, _iHttpContextAccessor.Object, _employeeContext.Object, _iSmsSender.Object, _appSettings);

            //companyContext
            _loggerCompanyContext = new Mock<ILogger<CompanyContext>>();
            _iEnvironmentVariables = new Mock<IEnvironmentVariables>();
            _companyContext = new Mock<CompanyContext>(_catalogContext.Object, _iEnvironmentVariables.Object, _loggerCompanyContext.Object, _triggerResourceReader.Object, _employeeContext.Object, _appSettings);

            //Company
            _logger = new Mock<ILogger<BLL_Company>>();
            _company = new Mock<BLL_Company>(_catalogContext.Object, _companyContext.Object, _backgroundJobRequest.Object, _iHttpContextAccessor.Object, _logger.Object, _appSettings);
        }

        [Fact]
        public void To_Verify_Get_Method_ReturnCompanyList()
        {
            //Arrange
            _company.Setup(x => x.SelectAllAsync()).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetCompanyDetailsModels()));
            var companyController = new CompanyController(_company.Object);

            //Act
            var result = companyController.Get();

            //Assert
            Assert.IsType<Task<ActionResult<CustomJsonData>>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(1)]
        public void To_Verify_Get_Method_ReturnCompanyListByCompanyId(int companyId)
        {
            //Arrange
            _company.Setup(x => x.SelectAsync(It.IsAny<CompanyDetailsModel>())).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetCompanyDetailsModels()));
            var companyController = new CompanyController(_company.Object);

            //Act
            var result = companyController.Get(companyId);

            //Assert
            Assert.IsType<Task<ActionResult<CustomJsonData>>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(1)]
        public void To_Verify_Post_Method_ReturnSucessMessage(int userId)
        {
            //Arrange
            _company.Setup(x => x.InsertAsync(It.IsAny<CompanyDetailsModel>())).ReturnsAsync(TestUtility.GetJsonData());
            var companyController = new CompanyController(_company.Object);

            //Act
            var result = companyController.Post(userId, TestUtility.GetCompanyDetailsModel());

            //Assert
            Assert.IsType<Task<ActionResult<JsonData>>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Put_Method_ReturnSucessMessage()
        {
            //Arrange
            _company.Setup(x => x.UpdateAsync(It.IsAny<CompanyDetailsModel>())).ReturnsAsync(TestUtility.GetJsonData(TestUtility.GetCompLogoModel()));
            var companyController = new CompanyController(_company.Object);

            //Act
            var result = companyController.Put(1, TestUtility.GetCompanyDetailsModel());

            //Assert
            Assert.IsType<Task<ActionResult<JsonData>>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData("Comp1", 1)]
        public void To_Verify_Delete_Method_ReturnSucessMessage(string companyId, int updatedBy)
        {
            //Arrange
            _company.Setup(x => x.DeleteAsync(It.IsAny<CompanyDetailsModel>())).ReturnsAsync(TestUtility.GetJsonData());
            var companyController = new CompanyController(_company.Object);

            //Act
            var result = companyController.Delete(companyId, updatedBy);

            //Assert
            Assert.IsType<Task<ActionResult<JsonData>>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_AddInactivityScheduler_Method_ReturnSucessMessage()
        {
            //Arrange
            _company.Setup(x => x.AddInactivityScheduler()).ReturnsAsync(TestUtility.GetJsonData());
            var companyController = new CompanyController(_company.Object);

            //Act
            var result = companyController.AddInactivityScheduler();

            //Assert
            Assert.IsType<Task<ActionResult<JsonData>>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
