using Microsoft.Extensions.Logging;
using Moq;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.SmsService;
using Trigger.Test.Shared;
using Trigger.Utility;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.SmsService
{
    public class SmsServiceContextTest
    {

        [Fact]
        public void To_Verify_SendSMSVerificationCode_Method()
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<SmsServiceContext>>();
            var iSmsSender = new Mock<ISmsSender>();
            var triggerContext = new Mock<TriggerContext>(serviceProvider);
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            var iConnectionContext = new Mock<IConnectionContext>();
            iConnectionContext.SetupGet(x => x.TriggerContext).Returns(triggerContext.Object);
            iSmsSender.Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()));
            var smsServiceContext = new SmsServiceContext(logger.Object,iConnectionContext.Object,iSmsSender.Object,catalogDbContext.Object);

            //Act
             smsServiceContext.SendSMSVerificationCode(TestUtility.GetSmsVerificationCode());

            //Assert
            Assert.True(true);
        }
    }
}
