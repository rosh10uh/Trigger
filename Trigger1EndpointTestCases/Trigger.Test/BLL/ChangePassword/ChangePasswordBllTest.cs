using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using BLL_ChangePassword = Trigger.BLL.ChangePassword.ChangePassword;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.ChangePassword
{
    public class ChangePasswordBllTest
    {
        [Fact]
        public void To_Verify_Change_Password()
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup(true);
            var _claims = setupTest.SetupClaims();
            var triggerContext = new Mock<TriggerContext>(serviceProvider);
            var logger = new Mock<ILogger<BLL_ChangePassword>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            catalogDbContext.Setup(x => x.AuthUserDetailsRepository.GetPasswordHashByUserId(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            iConnectionContext.SetupGet(x => x.TriggerContext).Returns(triggerContext.Object);
            triggerContext.Setup(x=>x.LoginRepository.InvokeLogin(It.IsAny<UserDataModel>())).Returns(TestUtility.GetUserDataModel());
            catalogDbContext.Setup(x => x.ChangePasswordRepository.invokeChangeAuthPassword(It.IsAny<UserChangePassword>())).Returns(new UserChangePassword());
            catalogDbContext.Setup(x => x.UserLoginModelRepository.invokeDeleteDeviceByLoginUserId(It.IsAny<UserLoginModel>())).Returns(new UserLoginModel());

            var changePasswordBll = new BLL_ChangePassword(catalogDbContext.Object, iConnectionContext.Object, logger.Object, _claims.Object);

            // Act
            var result = changePasswordBll.InvokeChangePassword(new UserChangePassword());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result);
        }
    }
}
