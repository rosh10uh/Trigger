using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;


namespace Trigger.Test.DAL.SmsService
{
    public class SmsServiceRepositoryTest : BaseRepositoryTest
    {
        public SmsServiceRepositoryTest() : base("SmsServiceRepository")
        {

        }

        [Fact]
        public void To_Verify_GetVerificationCodeForUser_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<SmsSettings, int, TriggerContext>(123).BuildServiceProvider());

            //Act
            var result = triggerContext.SmsServiceRepository.GetVerificationCodeForUser(FakeAppSettingsModel.GetSmsSettings().Value);

            //Assert      
            Assert.IsType<int>(result);
        }
    }
}
