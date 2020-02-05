using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Trigger.BLL.Spark;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.Spark;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Trigger.Utility;
using Xunit;
using static Trigger.BLL.Shared.Enums;
using SharedClaims = Trigger.DAL.Shared.Claims;

namespace Trigger.Test.BLL.Spark
{
    public class EmployeeSparkTest
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
        private readonly Mock<TriggerContext> _triggerContext;

        public EmployeeSparkTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iActionPermission = new Mock<IActionPermission>();
            _logger = new Mock<ILogger<EmployeeSparkContext>>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _loggerEmp = new Mock<ILogger<EmployeeSpark>>();
            _iSmsSender = new Mock<ISmsSender>();
            _employeeSparkContext = new Mock<EmployeeSparkContext>(_iConnectionContext.Object, _appSettings, _logger.Object);
            _employeeSpark = new Mock<EmployeeSpark>(_iConnectionContext.Object, _employeeSparkContext.Object, _appSettings, _iActionPermission.Object, _iSmsSender.Object, _loggerEmp.Object, _iClaims.Object);
        }

        [Fact]
        public void To_Verify_Get_Async()
        {
            // Arrange
            _iConnectionContext.Setup(x=>x.TriggerContext.EmployeeSparkRepository.GetEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSparks());
            // Act
            var result = _employeeSpark.Setup(x=>x.GetAsync(It.IsAny<int>()));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Get_UnApproved_SparkAsync()
        {
            // Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.GetUnApprovedSparkDetails(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSparks());
            // Act
            var result = _employeeSpark.Setup(x => x.GetUnApprovedSparkAsync());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Post_Async()
        {
            // Arrange
            _iActionPermission.Setup(x=>x.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanAdd)).Returns(new DTO.DimensionMatrix.CheckPermission());
            _employeeSparkContext.Setup(x=>x.AddEmployeeSpark(It.IsAny<EmployeeSparkModel>()));
            // Act
            var result = _employeeSpark.Setup(x => x.PostAsync(It.IsAny<EmployeeSparkModel>()));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Update_Spark_Approval_Status()
        {
            // Arrange
            _iConnectionContext.Setup(x=>x.TriggerContext.EmployeeRepository.Select(It.IsAny<EmployeeModel>())).Returns(new EmployeeModel());
            _iConnectionContext.Setup(x=>x.TriggerContext.EmployeeSparkRepository.UpdateSparkApprovalStatus(It.IsAny<EmployeeSparkModel>())).Returns(new EmployeeSparkModel());
            // Act
            var result = _employeeSpark.Setup(x => x.UpdateSparkApprovalStatus(It.IsAny<EmployeeSparkModel>()));

            // Assert
            Assert.NotNull(result);
        } 

        [Fact]
        public void To_Verify_Put_Async()
        {
            // Arrange
            _iActionPermission.Setup(x=>x.GetPermissionParameters(Actions.SparkEmployee,PermissionType.CanAdd)).Returns(new DTO.DimensionMatrix.CheckPermission());
            _employeeSparkContext.Setup(x=>x.UpdateEmployeeSpark(It.IsAny<EmployeeSparkModel>()));
             // Act
             var result = _employeeSpark.Setup(x => x.PutAsync(It.IsAny<EmployeeSparkModel>()));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Delete_Async()
        {
            // Arrange
            _iActionPermission.Setup(x => x.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanDelete)).Returns(new DTO.DimensionMatrix.CheckPermission());
            _employeeSparkContext.Setup(x => x.DeleteAsync(It.IsAny<EmployeeSparkModel>()));
            _iConnectionContext.Setup(x=>x.TriggerContext.EmployeeSparkRepository.DeleteEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>())).Returns(new EmployeeSparkModel());
            // Act
            var result = _employeeSpark.Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Delete_Attachment_Async()
        {
            // Arrange
            _iActionPermission.Setup(x => x.GetPermissionParameters(Actions.SparkEmployee, PermissionType.CanDelete)).Returns(new DTO.DimensionMatrix.CheckPermission());
            _employeeSparkContext.Setup(x => x.DeleteAttachmentAsync(It.IsAny<EmployeeSparkModel>(),It.IsAny<bool>()));
          
            // Act
            var result = _employeeSpark.Setup(x => x.DeleteAttachmentAsync(It.IsAny<EmployeeSparkModel>()));

            // Assert
            Assert.NotNull(result);
        }
    }
}
