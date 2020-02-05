using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.ExcelUpload;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.Controller.V1
{
    public class ExcelUploadControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<ExcelUpload> _excelUpload;
        private ExcelUploadController _excelUploadController;

        public ExcelUploadControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _excelUpload = new Mock<ExcelUpload>(_iConnectionContext.Object, null, _triggerCatalogContext.Object, null, null, _appSettings, null, null);

            // _excelUpload = new Mock<ExcelUpload>(_iConnectionContext.Object, null, _triggerCatalogContext.Object, null, null, _AppSettings, null);
        }


        [Fact]
        public void To_Verify_Get()
        {
            // Arrange
            _excelUpload.Setup(x => x.SelectAsync(It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(@"D:\abc"));

            _excelUploadController = new ExcelUploadController(_excelUpload.Object);
            // Act
            var result = _excelUploadController.Get(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<ActionResult<CustomJsonData>>>(result);
            Assert.NotNull(result.Result);
        }


        [Fact]
        public void To_Verify_Post()
        {
            // Arrange
            _excelUpload.Setup(x => x.InsertAsync(It.IsAny<CountRecordModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(ExcelUploadUtility.GetCountRecordModels()));

            _excelUploadController = new ExcelUploadController(_excelUpload.Object);
            // Act
            var result = _excelUploadController.Post("abc_1", ExcelUploadUtility.GetExcelEmployeesModels());

            // Assert
            Assert.IsType<Task<ActionResult<CustomJsonData>>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UploadAsync()
        {
            // Arrange
            _excelUpload.Setup(x => x.UploadAsync(It.IsAny<CountRecordModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData());

            _excelUploadController = new ExcelUploadController(_excelUpload.Object);
            // Act
            var result = _excelUploadController.PostExcel("abc_1", ExcelUploadUtility.GetExcelEmployeesModels());

            // Assert
            Assert.IsType<Task<ActionResult<CustomJsonData>>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
