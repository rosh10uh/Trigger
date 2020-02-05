using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
using DTOUserLogin = Trigger.DTO.UserLogin;
namespace Trigger.Test.DAL.UserLogin
{
    public class UserLoginRepositoryTest : BaseRepositoryTest
    {
        public UserLoginRepositoryTest() : base("UserLoginRepository")
        {

        }

        [Fact]
        public void To_Verify_GetUserDetails_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerCatalogContext(GetServiceCollection<DTOUserLogin, DTOUserLogin, TriggerCatalogContext>(TestUtility.GetUserLogin()).BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.UserLoginRepository.GetUserDetails(It.IsAny<DTOUserLogin>());

            //Assert      
            Assert.IsType<DTOUserLogin>(result);
            Assert.NotNull(result);
        }
    }
}
