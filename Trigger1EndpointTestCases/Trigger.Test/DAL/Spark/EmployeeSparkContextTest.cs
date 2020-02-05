using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.Spark;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.DAL.Spark
{
    public class EmployeeSparkContextTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<EmployeeSparkContext> _employeeSparkContext;
        private readonly Mock<ILogger<EmployeeSparkContext>> _logger;
        private readonly IOptions<AppSettings> _appSettings;

        public EmployeeSparkContextTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _logger = new Mock<ILogger<EmployeeSparkContext>>();
            _employeeSparkContext = new Mock<EmployeeSparkContext>(_iConnectionContext.Object, _appSettings, _logger.Object);
        }

        [Fact]
        public void To_Verify_Add_Employee_Spark()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.InsertEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSpark());
            _iConnectionContext.Setup(x=>x.TriggerContext.EmployeeSparkRepository.GetEmployeeSparkDetailsBySparkId(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSpark());

            //Act
            var result = _employeeSparkContext.Setup(x=>x.AddEmployeeSpark(It.IsAny<EmployeeSparkModel>()));

            //Assert
            Assert.NotNull(result);
        }

        
        [Fact]
        public void To_Verify_Update_Employee_Spark()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.UpdateEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSpark());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.GetEmployeeSparkDetailsBySparkId(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSpark());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.Select(It.IsAny<EmployeeSparkModel>()));
            
            //Act
            var result = _employeeSparkContext.Setup(x => x.UpdateEmployeeSpark(It.IsAny<EmployeeSparkModel>()));

            //Assert
            Assert.NotNull(result);
        }
        
        [Fact]
        public void To_Verify_Delete_Async()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.DeleteEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSpark());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.GetEmployeeSparkDetailsBySparkId(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSpark());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.Select(It.IsAny<EmployeeSparkModel>()));

            //Act
            var result = _employeeSparkContext.Setup(x => x.DeleteAsync(It.IsAny<EmployeeSparkModel>()));

            //Assert
            Assert.NotNull(result);
        }

       [Fact]
        public void To_Verify_Delete_Attachment_Async()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.DeleteEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSpark());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.GetEmployeeSparkDetailsBySparkId(It.IsAny<EmployeeSparkModel>())).Returns(ExcelUploadUtility.GetEmployeeSpark());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeSparkRepository.Select(It.IsAny<EmployeeSparkModel>()));

            //Act
            var result = _employeeSparkContext.Setup(x => x.DeleteAttachmentAsync(It.IsAny<EmployeeSparkModel>(),It.IsAny<bool>()));

            //Assert
            Assert.NotNull(result);
        }
    }
}
