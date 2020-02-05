using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.SmsService;
using Trigger.DTO;
using Trigger.DTO.SmsService;
using Trigger.Test.Shared;
using Trigger.Utility;
using Xunit;
using BLL_SmsVerificationCode = Trigger.BLL.SmsService.SmsVerificationCode;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller
{
    public class SmsServiceControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<ILogger<BLL_SmsVerificationCode>> _logger;
        private readonly Mock<ILogger<SmsServiceContext>> _loggerSmsServiceContext;
        private readonly Mock<ISmsSender> _iSmsSender;
        private readonly Mock<TriggerCatalogContext> _catalogDbContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly IOptions<SmsSettings> _smsSetting ;
        private readonly Mock<SmsServiceContext> _smsServiceContext ;
        private readonly Mock<BLL_SmsVerificationCode> _smsVerificationCode ;

        public SmsServiceControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _logger = new Mock<ILogger<BLL_SmsVerificationCode>>();
            _loggerSmsServiceContext = new Mock<ILogger<SmsServiceContext>>();
            _iSmsSender = new Mock<ISmsSender>();
            _catalogDbContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iConnectionContext = new Mock<IConnectionContext>();
            _smsSetting = FakeAppSettingsModel.GetSmsSettings();
            _smsServiceContext = new Mock<SmsServiceContext>(_loggerSmsServiceContext.Object, _iConnectionContext.Object, _iSmsSender.Object, _catalogDbContext.Object);
            _smsVerificationCode = new Mock<BLL_SmsVerificationCode>(_smsSetting, _iConnectionContext.Object, _smsServiceContext.Object, _catalogDbContext.Object, _logger.Object);
            
        }

        [Fact]
        public void To_Verify_SendSMSVerificationCode_Method()
        {
            // Arrange
            _smsVerificationCode.Setup(x => x.SendSMSVerificationCode(It.IsAny<SmsVerificationCode>())).ReturnsAsync(TestUtility.GetJsonData());
            var smsServiceController = new SmsServiceController(_smsVerificationCode.Object);

            //Act
            var result = smsServiceController.SendSMSVerificationCode(It.IsAny<SmsVerificationCode>());
            
            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Fact]
        public void To_Verify_VerifySmsVerificationCode_Method()
        {
            // Arrange
            _smsVerificationCode.Setup(x => x.VerifySmsVerificationCode(It.IsAny<SmsVerificationCode>())).ReturnsAsync(TestUtility.GetJsonData(TestUtility.GetSmsVerificationCode()));
            var smsServiceController = new SmsServiceController(_smsVerificationCode.Object);

            //Act
            var result = smsServiceController.VerifySmsVerificationCode(It.IsAny<SmsVerificationCode>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
