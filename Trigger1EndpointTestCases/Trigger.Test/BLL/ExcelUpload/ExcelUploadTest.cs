using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using SharedClaims = Trigger.DAL.Shared.Claims;
using BLL_ExcelUpload = Trigger.BLL.ExcelUpload.ExcelUpload;
using Trigger.DAL.ExcelUpload;
using Microsoft.AspNetCore.Http;
using Trigger.BLL.ExcelUpload;
using Xunit;
using System.Threading.Tasks;
using Trigger.Test.Shared.Utility;
using Trigger.DAL.Shared.Sql;
using System.Data;

namespace Trigger.Test.BLL.ExcelUpload
{
    public class ExcelUploadTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_ExcelUpload>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private readonly Mock<ExcelUploadContext> _excelUploadContext;
        private readonly Mock<IHttpContextAccessor> _iHttpContextAccessor;
        private readonly Mock<ExcelUploadHelper> _excelUploadHelper;
        private readonly Mock<SqlHelper> _sqlHelper;
        private BLL_ExcelUpload _excelUpload;

        public ExcelUploadTest()
        {
            _logger = new Mock<ILogger<BLL_ExcelUpload>>();
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _excelUploadContext = new Mock<ExcelUploadContext>(null, _iConnectionContext.Object, _triggerCatalogContext.Object, null, _appSettings, null);
            _iHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _excelUploadHelper = new Mock<ExcelUploadHelper>(_iConnectionContext.Object, null, null, _appSettings);
            _sqlHelper = new Mock<SqlHelper>();
        }

        private void Init_ExcepUpload()
        {
            _excelUpload = new BLL_ExcelUpload(_iConnectionContext.Object, _excelUploadContext.Object, _triggerCatalogContext.Object, _logger.Object, _iHttpContextAccessor.Object, _appSettings, _excelUploadHelper.Object, _sqlHelper.Object);
        }

        [Fact]
        public void To_Verify_SelectAsync()
        {
            //Arrange
            _iHttpContextAccessor.Setup(_ => _.HttpContext).Returns(new DefaultHttpContext());
            _excelUploadHelper.Setup(x => x.CreateExcelTemplate(It.IsAny<int>())).Returns(true);

            Init_ExcepUpload();
            //Act
            var result = _excelUpload.SelectAsync(1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_SelectAsync_WithException()
        {
            //Arrange
            Init_ExcepUpload();

            //Act
            var result = _excelUpload.SelectAsync(1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_InsertAsync()
        {
            //Arrange
            _triggerCatalogContext.Setup(x => x.CompanyDbConfig.Select<string>(It.IsAny<CompanyDbConfig>())).Returns("devtrigger");
            _sqlHelper.Setup(x => x.SqlBulkInsert(It.IsAny<string>(), It.IsAny<DataTable>(), It.IsAny<string>())).Returns(true);
            _iConnectionContext.Setup(x => x.TriggerContext.ExcelUploadRepository.GetExcelDataCount(It.IsAny<MasterTables>())).Returns(ExcelUploadUtility.GetExcelDatas());
            _triggerCatalogContext.Setup(x => x.AuthUserExcelRepository.GetEmployeeByPhoneNumberCatalog(It.IsAny<AuthUserExcelModel>())).Returns(ExcelUploadUtility.GetExcelDatas());
            _iConnectionContext.Setup(x => x.TriggerContext.AuthUserExcelRepository.GetEmployeeByPhoneNumberTenant(It.IsAny<AuthUserExcelModel>())).Returns(ExcelUploadUtility.GetExcelDatas());

            Init_ExcepUpload();

            //Act
            var result = _excelUpload.InsertAsync(ExcelUploadUtility.GetCountRecordModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

    }
}
