using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
namespace Trigger.Test.DAL.Login
{
    public class LoginRepositoryTest : BaseRepositoryTest
    {
        public LoginRepositoryTest() : base("LoginRepository")
        {
        }

        [Fact]
        public void To_Verify_InvokeLogin_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<UserDataModel, UserDataModel, TriggerContext>(TestUtility.GetUserDataModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.LoginRepository.InvokeLogin(It.IsAny<UserDataModel>());

            //Assert      
            Assert.IsType<UserDataModel>(result);
            Assert.NotNull(result);
        }
    }
}
