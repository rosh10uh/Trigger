using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_ChangePassword = Trigger.BLL.ChangePassword.ChangePassword;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller
{
    public class ChangePasswordControllerTest
    {
        [Fact]
        public void To_Verify_Post_Method()
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup(true);
            var _claims = setupTest.SetupClaims();
            var logger = new Mock<ILogger<BLL_ChangePassword>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            var changePassword = new Mock<BLL_ChangePassword>(catalogDbContext.Object, iConnectionContext.Object, logger.Object, _claims.Object);
            changePassword.Setup(x => x.InvokeChangePassword(It.IsAny<UserChangePassword>())).Returns(TestUtility.GetJsonData());
            var changePasswordController = new ChangePasswordController(changePassword.Object);

            // Act
            var result = changePasswordController.PostAsync(It.IsAny<UserChangePassword>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
