using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.Dashboard;
using Trigger.BLL.Shared.Interfaces;
using Trigger.Controllers.V2;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller.V2
{
    public class DashboardControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly Mock<IActionPermission> _iActionPermission;
        private readonly Mock<Dashboard> _dashboardBll;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<ILogger<Dashboard>> _logger;
        private readonly IOptions<AppSettings> _AppSettings;
        private readonly Mock<IClaims> _iclaims;


        public DashboardControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _AppSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iclaims = new Mock<IClaims>();
            _iActionPermission = new Mock<IActionPermission>();
            _logger = new Mock<ILogger<Dashboard>>();
            _dashboardBll = new Mock<Dashboard>(_iConnectionContext.Object,_logger.Object,_iclaims.Object,_triggerCatalogContext.Object,_iActionPermission.Object,_AppSettings);
        }

        [Fact]
        public void To_Verify_Get_Employee_DashBoard_Async_V2()
        {
            //Arrange
            _dashboardBll.Setup(x => x.GetEmployeeDashBoardAsyncV2(It.IsAny<int>())).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetEmpDashboards()));
            var dashboardController = new DashboardController(_dashboardBll.Object);

            //Act
            var result = dashboardController.Get(It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
