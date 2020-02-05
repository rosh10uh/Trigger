using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Dashboard = Trigger.BLL.Dashboard.Dashboard;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
using SharedClaims = Trigger.DAL.Shared.Claims;
using Microsoft.Extensions.Options;

namespace Trigger.Test.Controller
{
    public class DashboardControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_Dashboard>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<BLL_Dashboard> _dashboard;
        private readonly Mock<SharedClaims> _iclaims;
        private readonly Mock<TriggerCatalogContext> _catalogDbContext;
        private readonly Mock<IActionPermission> _iActionPermission;
        private readonly IOptions<AppSettings> _appSettings;

        public DashboardControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup(true);
            _iclaims = _setupTest.SetupClaims();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iActionPermission = new Mock<IActionPermission>();
            _logger = new Mock<ILogger<BLL_Dashboard>>();
            _catalogDbContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _dashboard = new Mock<BLL_Dashboard>(_iConnectionContext.Object, _logger.Object, _iclaims.Object, _catalogDbContext.Object, _iActionPermission.Object, _appSettings);
        }

        [Fact]
        public void To_Verify_Get_Mehod_ReturnDashboardData()
        {
            //Arrange
            _dashboard.Setup(x => x.GetEmployeeDashBoardAsyncV1(It.IsAny<int>())).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetEmpDashboards()));
            var dashboardController = new DashboardController(_dashboard.Object);

            //Act
            var result = dashboardController.Get(It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_Mehod_ReturnYearlyDepartmentWiseManagerDashBoardData()
        {
            //Arrange
            _dashboard.Setup(x => x.GetYearlyDepartmentWiseManagerDashBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetManagerDashBoardModels()));
            var dashboardController = new DashboardController(_dashboard.Object);

            //Act
            var result = dashboardController.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
