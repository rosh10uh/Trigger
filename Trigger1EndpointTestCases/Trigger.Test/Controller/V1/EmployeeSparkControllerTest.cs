using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.Spark;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.Spark;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Test.Shared;
using Trigger.Utility;
using Xunit;
using SharedClaims = Trigger.DAL.Shared.Claims;
namespace Trigger.Test.Controller.V1
{
    public class EmployeeSparkControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private readonly Mock<EmployeeSpark> _employeeSpark;
        private readonly Mock<IActionPermission> _iActionPermission;
        private readonly Mock<EmployeeSparkContext> _employeeSparkContext;
        private readonly Mock<ILogger<EmployeeSparkContext>> _logger;
        private readonly Mock<ILogger<EmployeeSpark>> _loggerEmp;
        private readonly Mock<ISmsSender> _iSmsSender;

        public EmployeeSparkControllerTest()
        {
             _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iActionPermission = new Mock<IActionPermission>();
            _logger = new Mock<ILogger<EmployeeSparkContext>>();
            _loggerEmp = new Mock<ILogger<EmployeeSpark>>();
            _iSmsSender = new Mock<ISmsSender>();
            _employeeSparkContext = new Mock<EmployeeSparkContext>(_iConnectionContext.Object, _appSettings, _logger.Object);
            _employeeSpark = new Mock<EmployeeSpark>(_iConnectionContext.Object, _employeeSparkContext.Object,_appSettings,_iActionPermission.Object,_iSmsSender.Object, _loggerEmp.Object, _iClaims.Object);
        }

        [Fact]
        public void To_Verify_Get()
        {
            // Arrange
            _employeeSpark.Setup(x=>x.GetAsync(It.IsAny<int>()));
            var employeeSparkController = new EmployeeSparkController(_employeeSpark.Object);

            // Act
            var result = employeeSparkController.Get(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Get_UnApproved_Spark( )
        {
            // Arrange
            _employeeSpark.Setup(x=>x.GetUnApprovedSparkAsync());
            var employeeSparkController = new EmployeeSparkController(_employeeSpark.Object);

            // Act
            var result = employeeSparkController.GetUnApprovedSpark();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Update_Spark_Approval_Status()
        {
            // Arrange
            _employeeSpark.Setup(x=>x.UpdateSparkApprovalStatus(It.IsAny<EmployeeSparkModel>()));
            var employeeSparkController = new EmployeeSparkController(_employeeSpark.Object);

            // Act
            var result = employeeSparkController.UpdateSparkApprovalStatus(new DTO.Spark.EmployeeSparkModel());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Post_Async()
        {
            // Arrange
            _employeeSpark.Setup(x=>x.PostAsync(It.IsAny<EmployeeSparkModel>()));
            var employeeSparkController = new EmployeeSparkController(_employeeSpark.Object);

            // Act
            var result = employeeSparkController.PostAsync(It.IsAny<EmployeeSparkModel>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Put_Async()
        {
            // Arrange
            _employeeSpark.Setup(x => x.PutAsync(It.IsAny<EmployeeSparkModel>()));
            var employeeSparkController = new EmployeeSparkController(_employeeSpark.Object);

            // Act
            var result = employeeSparkController.PutAsync(It.IsAny<EmployeeSparkModel>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Delete_Async()
        {
            // Arrange
            _employeeSpark.Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
            var employeeSparkController = new EmployeeSparkController(_employeeSpark.Object);

            // Act
            var result = employeeSparkController.DeleteAsync(It.IsAny<int>(),It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Delete_Attachment_Async()
        {
            // Arrange
            _employeeSpark.Setup(x => x.DeleteAttachmentAsync(It.IsAny<EmployeeSparkModel>()));
            var employeeSparkController = new EmployeeSparkController(_employeeSpark.Object);

            // Act
            var result = employeeSparkController.DeleteAttachmentAsync(It.IsAny<EmployeeSparkModel>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }
    }
}
