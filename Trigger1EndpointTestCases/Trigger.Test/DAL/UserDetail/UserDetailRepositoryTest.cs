using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.DAL.UserDetail
{
    public class UserDetailRepositoryTest : BaseRepositoryTest
    {
        public UserDetailRepositoryTest() : base("UserDetailRepository")
        {

        }

        [Fact]
        public void To_Verify_AddUser()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<UserDetails, DTO.UserLogin, TriggerContext>(CommonUtility.GetUserLogin()).BuildServiceProvider());

            //Act
            var result = triggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>());

            //Assert      
            Assert.IsType<DTO.UserLogin>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_UpdateUser()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<UserDetails, DTO.UserLogin, TriggerContext>(CommonUtility.GetUserLogin()).BuildServiceProvider());

            //Act
            var result = triggerContext.UserDetailRepository.UpdateUser(It.IsAny<UserDetails>());

            //Assert      
            Assert.IsType<DTO.UserLogin>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetUserDetails()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<UserDetails, UserDetails, TriggerContext>(CommonUtility.GetUserDetails()).BuildServiceProvider());

            //Act
            var result = triggerContext.UserDetailRepository.GetUserDetails(It.IsAny<UserDetails>());

            //Assert      
            Assert.IsType<DTO.UserDetails>(result);
            Assert.NotNull(result);
        }
    }
}
