using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.ChangePassword
{
    public class ChangePasswordRepositoryTest : BaseRepositoryTest
    {
        public ChangePasswordRepositoryTest() : base("ChangePasswordRepository")
        {

        }

        [Fact]
        public void To_Verify_InvokeChangeAuthPassword_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerCatalogContext(GetServiceCollection<UserChangePassword, UserChangePassword, TriggerCatalogContext>(TestUtility.GetUserChangePassword()).BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.ChangePasswordRepository.invokeChangeAuthPassword(TestUtility.GetUserChangePassword());

            //Assert      
            Assert.IsType<UserChangePassword>(result);
            Assert.NotNull(result);
        }
    }
}
