using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.AuthUserDetail
{
    public class AuthUserDetailsRepositoryTest : BaseRepositoryTest
    {
        public AuthUserDetailsRepositoryTest() :base("AuthUserDetailsRepository")
        {

        }

        [Fact]
        public void Verify_To_GetSubIdByEmail_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerCatalogContext(GetServiceCollection<AuthUserDetails, AuthUserDetails, TriggerCatalogContext>(TestUtility.GetAuthUserDetails()).BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>());

            //Assert      
            Assert.IsType<AuthUserDetails>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void Verify_To_InvokeGetPasswordHashByUserId_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerCatalogContext(GetServiceCollection<AuthUserDetails, AuthUserDetails, TriggerCatalogContext>(TestUtility.GetAuthUserDetails()).BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.AuthUserDetailsRepository.GetPasswordHashByUserId(It.IsAny<AuthUserDetails>());

            //Assert      
            Assert.IsType<AuthUserDetails>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void Verify_To_GetAuthDetailsByEmail_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerCatalogContext(GetServiceCollection<AuthUserDetails, AuthUserDetails, TriggerCatalogContext>(TestUtility.GetAuthUserDetails()).BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.AuthUserDetailsRepository.GetAuthDetailsByEmail(It.IsAny<AuthUserDetails>());

            //Assert      
            Assert.IsType<AuthUserDetails>(result);
            Assert.NotNull(result);
        }
    }
}
