using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
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

namespace Trigger.Test.BLL.SmsService
{
    public class SmsVerificationCodeTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<ILogger<BLL_SmsVerificationCode>> _logger;
        private readonly Mock<ILogger<SmsServiceContext>> _loggerSmsServiceContext;
        private readonly Mock<ISmsSender> _iSmsSender;
        private readonly Mock<TriggerCatalogContext> _catalogDbContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly IOptions<SmsSettings> _smsSetting;
        private readonly Mock<SmsServiceContext> _smsServiceContext;
        private readonly Mock<BLL_SmsVerificationCode> _smsVerificationCode;
        private readonly Mock<TriggerContext> _triggerContext;

        public SmsVerificationCodeTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _logger = new Mock<ILogger<BLL_SmsVerificationCode>>();
            _loggerSmsServiceContext = new Mock<ILogger<SmsServiceContext>>();
            _iSmsSender = new Mock<ISmsSender>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _catalogDbContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iConnectionContext = new Mock<IConnectionContext>();
            _smsSetting = FakeAppSettingsModel.GetSmsSettings();
            _smsServiceContext = new Mock<SmsServiceContext>(_loggerSmsServiceContext.Object, _iConnectionContext.Object, _iSmsSender.Object, _catalogDbContext.Object);
            _smsVerificationCode = new Mock<BLL_SmsVerificationCode>(_smsSetting, _iConnectionContext.Object, _smsServiceContext.Object, _catalogDbContext.Object, _logger.Object);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);

        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-3)]
        [InlineData(1)]
        public void To_Verify_SendSMSVerificationCode_WithResonseResult(int responseResult)
        {
            // Arrange
            SmsVerificationCode smsVerificationCodeModel = TestUtility.GetSmsVerificationCode();
            smsVerificationCodeModel.result = responseResult;
            _iConnectionContext.Setup(x => x.TriggerContext.SmsServiceRepository.GetVerificationCodeForUser(It.IsAny<SmsSettings>())).Returns(123);
            _iConnectionContext.Setup(x => x.TriggerContext.SmsServiceRepository.Insert(It.IsAny<SmsVerificationCode>())).Returns(smsVerificationCodeModel);
            _smsServiceContext.Setup(x => x.SendSMSVerificationCode(It.IsAny<SmsVerificationCode>()));
            var smsVerificationCode = new BLL_SmsVerificationCode(_smsSetting, _iConnectionContext.Object, _smsServiceContext.Object, _catalogDbContext.Object, _logger.Object);

            //Act
            var result = smsVerificationCode.SendSMSVerificationCode(TestUtility.GetSmsVerificationCode());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_SendSMSVerificationCode_WithException(bool isException)
        {
            // Arrange
            SmsVerificationCode smsVerificationCodeModel = TestUtility.GetSmsVerificationCode();
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.SmsServiceRepository.Insert(It.IsAny<SmsVerificationCode>())).Returns(smsVerificationCodeModel);
            }
            _iConnectionContext.Setup(x => x.TriggerContext.SmsServiceRepository.GetVerificationCodeForUser(It.IsAny<SmsSettings>())).Returns(123);
            _smsServiceContext.Setup(x => x.SendSMSVerificationCode(It.IsAny<SmsVerificationCode>()));
            var smsVerificationCode = new BLL_SmsVerificationCode(_smsSetting, _iConnectionContext.Object, _smsServiceContext.Object, _catalogDbContext.Object, _logger.Object);

            //Act
            var result = smsVerificationCode.SendSMSVerificationCode(TestUtility.GetSmsVerificationCode());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-2)]
        [InlineData(-3)]
        public void To_Verify_VerifySmsVerificationCode_WithResonseResult(int responseResult)
        {
            // Arrange
            SmsVerificationCode smsVerificationCodeModel = TestUtility.GetSmsVerificationCode();
            smsVerificationCodeModel.result = responseResult;
            _iConnectionContext.Setup(x => x.TriggerContext.SmsServiceRepository.Select(It.IsAny<SmsVerificationCode>())).Returns(smsVerificationCodeModel);
            var smsVerificationCode = new BLL_SmsVerificationCode(_smsSetting, _iConnectionContext.Object, _smsServiceContext.Object, _catalogDbContext.Object, _logger.Object);

            //Act
            var result = smsVerificationCode.VerifySmsVerificationCode(TestUtility.GetSmsVerificationCode());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_VerifySmsVerificationCode_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.SmsServiceRepository.Select(It.IsAny<SmsVerificationCode>())).Returns(TestUtility.GetSmsVerificationCode());
            }

            var smsVerificationCode = new BLL_SmsVerificationCode(_smsSetting, _iConnectionContext.Object, _smsServiceContext.Object, _catalogDbContext.Object, _logger.Object);

            //Act
            var result = smsVerificationCode.VerifySmsVerificationCode(TestUtility.GetSmsVerificationCode());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
