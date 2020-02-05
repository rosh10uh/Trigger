using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_ChangePassword = Trigger.BLL.ChangePassword.ChangePassword;
using TestUtility = Trigger.Test.Shared.Utility;

namespace Trigger.Test.BLL.ChangePassword
{
    public class ChangePasswordTest
    {
        [Theory]
        // [InlineData(true)]
        // [InlineData(false)]
        public void Invoke_ChangePassword(bool isException)
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup(true);
            var _claims = setupTest.SetupClaims();
            var triggerContext = new Mock<TriggerContext>(serviceProvider);
            var logger = new Mock<ILogger<BLL_ChangePassword>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            var iConnectionContext = new Mock<IConnectionContext>();
            var passwordHasherOptions = new Mock<IOptions<PasswordHasherOptions>>();


            var passwordHasher = new Mock<PasswordHasher<Trigger.DTO.UserChangePassword>>(passwordHasherOptions.Object);
            iConnectionContext.SetupGet(x => x.TriggerContext).Returns(triggerContext.Object);

            if (!isException)
            {
                catalogDbContext.Setup(x => x.AuthUserDetailsRepository.invokeGetPasswordHashByUserId(It.IsAny<AuthUserDetails>())).Returns(TestUtility.GetAuthUserDetails());
                iConnectionContext.Setup(x => x.TriggerContext.LoginRepository.InvokeLogin(TestUtility.GetUserDataModel())).Returns(TestUtility.GetUserDataModel());
            }

            passwordHasher.Setup(x => x.VerifyHashedPassword(It.IsAny<UserChangePassword>(), It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Success);
            passwordHasher.Setup(x => x.HashPassword(It.IsAny<UserChangePassword>(), It.IsAny<string>())).Returns("New Password");


            catalogDbContext.Setup(x => x.ChangePasswordRepository.invokeChangeAuthPassword(TestUtility.GetUserChangePassword())).Returns(TestUtility.GetUserChangePassword());
            iConnectionContext.Setup(x => x.TriggerContext.UserLoginModelRepository.invokeDeleteDeviceByLoginUserId(TestUtility.GetUserLoginModel()));
            var changePassword = new BLL_ChangePassword(passwordHasher.Object, catalogDbContext.Object, iConnectionContext.Object, logger.Object, _claims.Object);

            // Act
            var result = changePassword.InvokeChangePassword(TestUtility.GetUserChangePassword());

            //  Assert
            Assert.IsType<JsonData>(result);
        }
    }
}
