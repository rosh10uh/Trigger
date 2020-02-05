using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.Login
{
    public class UserLoginModelRepositoryTest : BaseRepositoryTest
    {
        public UserLoginModelRepositoryTest() : base("UserLoginModelRepository")
        {

        }

        [Fact]
        public void To_Verify_RegisterUserDeviceInfo_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<UserLoginModel, UserLoginModel, TriggerContext>(TestUtility.GetUserLoginModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.UserLoginModelRepository.RegisterUserDeviceInfo(It.IsAny<UserLoginModel>());

            //Assert      
            Assert.IsType<UserLoginModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_InvokeDeleteDeviceInfo_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<UserLoginModel, UserLoginModel, TriggerContext>(TestUtility.GetUserLoginModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.UserLoginModelRepository.invokeDeleteDeviceInfo(It.IsAny<UserLoginModel>());

            //Assert      
            Assert.IsType<UserLoginModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_InvokeDeleteDeviceByLoginUserId_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<UserLoginModel, UserLoginModel, TriggerContext>(TestUtility.GetUserLoginModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.UserLoginModelRepository.invokeDeleteDeviceByLoginUserId(It.IsAny<UserLoginModel>());

            //Assert      
            Assert.IsType<UserLoginModel>(result);
            Assert.NotNull(result);
        }
    }
}
