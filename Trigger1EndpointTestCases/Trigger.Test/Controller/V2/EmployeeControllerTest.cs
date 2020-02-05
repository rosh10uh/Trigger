using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.Employee;
using Trigger.BLL.Notification;
using Trigger.Controllers.V2;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using SharedClaims = Trigger.DAL.Shared.Claims;


namespace Trigger.Test.Controller.V2
{
    public class EmployeeControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly IOptions<AppSettings> _AppSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private readonly Mock<EmployeeCommon> _employeeCommon;
        private readonly Mock<EmployeeSendEmail> _employeeSendEmail;
        private readonly Mock<Employee> _employee;
        private readonly Mock<Notification> _Notification;
        private readonly Mock<IActionPermission> _iActionPermission;

        public EmployeeControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _AppSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iActionPermission = new Mock<IActionPermission>();
            _employeeCommon = new Mock<EmployeeCommon>(_triggerCatalogContext.Object, _iConnectionContext.Object, null, null, _AppSettings, _iClaims.Object, null);
            _employeeSendEmail = new Mock<EmployeeSendEmail>(null, null, _AppSettings, _triggerCatalogContext.Object, null, _employeeCommon.Object);
            _Notification = new Mock<Notification>(_iConnectionContext.Object, null, _AppSettings);
            _employee = new Mock<Employee>(_iConnectionContext.Object, null, _triggerCatalogContext.Object,
                _employeeSendEmail.Object, _employeeCommon.Object, _Notification.Object, _iActionPermission.Object);
        }


        [Fact]
        public void To_Verify_Get_TriggerEmpList()
        {
            // Arrange
            _employee.Setup(x => x.GetTriggerEmpListV2(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeListModels()));

            var employeeController = new EmployeeController(_employee.Object);

            // Act
            var result = employeeController.GetTriggerEmpList(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_DashboardEmpList()
        {
            // Arrange
            _employee.Setup(x => x.GetDashboardEmpListV2(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeListModels()));

            var employeeController = new EmployeeController(_employee.Object);

            // Act
            var result = employeeController.GetDashboardEmpList(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
