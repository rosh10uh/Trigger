using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Test.Shared;
using Xunit;
using BLL_Dashboard = Trigger.BLL.Dashboard.Dashboard;
using SharedClaims = Trigger.DAL.Shared.Claims;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Dashboard
{
    public class DashboardTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_Dashboard>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<SharedClaims> _iclaims;
        private readonly Mock<TriggerCatalogContext> _catalogDbContext;
        private readonly Mock<IActionPermission> _iActionPermission;
        private readonly IOptions<AppSettings> _appSettings;

        public DashboardTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iclaims = _setupTest.SetupClaims();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<BLL_Dashboard>>();
            _iActionPermission = new Mock<IActionPermission>();
            _catalogDbContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Get_EmployeeDashBoardAsyncV1_ReturnListOfDashboardData(int totalResultCount)
        {
            // Arrange 
            var resultData = totalResultCount == 0 ? new List<EmployeeDashboardModel>() : TestUtility.GetEmployeeDashboardModels();
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(It.IsAny<EmployeeDashboardModel>())).Returns(resultData);
            var dashboard = new BLL_Dashboard(_iConnectionContext.Object, _logger.Object, _iclaims.Object, _catalogDbContext.Object, _iActionPermission.Object, _appSettings);

            // Act
            var result = dashboard.GetEmployeeDashBoardAsyncV1(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Get_EmployeeDashBoardAsyncV1_WithException(bool haveException)
        {
            // Arrange 
            if (!haveException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(It.IsAny<EmployeeDashboardModel>())).Returns(TestUtility.GetEmployeeDashboardModels());
            }
            var dashboard = new BLL_Dashboard(_iConnectionContext.Object, _logger.Object, _iclaims.Object, _catalogDbContext.Object, _iActionPermission.Object, _appSettings);

            // Act
            var result = dashboard.GetEmployeeDashBoardAsyncV1(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Get_EmployeeDashBoardAsyncV2_ReturnListOfDashboardData(int totalResultCount)
        {
            // Arrange 
            List<ActionList> actionLists = TestUtility.GetActionLists();
            actionLists[0].ActionId = 3;
            _iActionPermission.Setup(x => x.GetPermissions(It.IsAny<int>())).Returns(actionLists);
            var resultData = totalResultCount == 0 ? new List<EmployeeDashboardModel>() : TestUtility.GetEmployeeDashboardModels();
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(It.IsAny<EmployeeDashboardModel>())).Returns(resultData);
            var dashboard = new BLL_Dashboard(_iConnectionContext.Object, _logger.Object, _iclaims.Object, _catalogDbContext.Object, _iActionPermission.Object, _appSettings);

            // Act
            var result = dashboard.GetEmployeeDashBoardAsyncV2(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Get_EmployeeDashBoardAsyncV2_WithCanViewPermission(bool canView)
        {
            // Arrange 
            List<ActionList> actionLists = TestUtility.GetActionLists();
            actionLists[0].ActionPermissions[0].CanView = canView;
            actionLists[0].ActionId = 3;

            _iActionPermission.Setup(x => x.GetPermissions(It.IsAny<int>())).Returns(actionLists);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(It.IsAny<EmployeeDashboardModel>())).Returns(TestUtility.GetEmployeeDashboardModels());
            var dashboard = new BLL_Dashboard(_iConnectionContext.Object, _logger.Object, _iclaims.Object, _catalogDbContext.Object, _iActionPermission.Object, _appSettings);

            // Act
            var result = dashboard.GetEmployeeDashBoardAsyncV2(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Get_EmployeeDashBoardAsyncV2_WithException(bool haveException)
        {
            // Arrange 
            if (!haveException)
            {
                _iActionPermission.Setup(x => x.GetPermissions(It.IsAny<int>())).Returns(TestUtility.GetActionLists());
                _iConnectionContext.Setup(x => x.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(It.IsAny<EmployeeDashboardModel>())).Returns(TestUtility.GetEmployeeDashboardModels());
            }
            var dashboard = new BLL_Dashboard(_iConnectionContext.Object, _logger.Object, _iclaims.Object, _catalogDbContext.Object, _iActionPermission.Object, _appSettings);

            // Act
            var result = dashboard.GetEmployeeDashBoardAsyncV2(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void GetYearlyDepartmentWiseManagerDashBoardAsync()
        {
            // Arrange 
            _catalogDbContext.Setup(x => x.CompanyDbConfig.Select<string>(It.IsAny<CompanyDbConfig>())).Returns("ABC");
            var dashboard = new BLL_Dashboard(_iConnectionContext.Object, _logger.Object, _iclaims.Object, _catalogDbContext.Object, _iActionPermission.Object, _appSettings);

            //Act
            var result = dashboard.GetYearlyDepartmentWiseManagerDashBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

    }
}
