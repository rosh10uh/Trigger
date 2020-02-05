using Microsoft.Extensions.Logging;
using Moq;
using Trigger.Controllers.V1;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Login = Trigger.BLL.Login.Login;
using BLL_Notification = Trigger.BLL.Notification.Notification;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;


namespace Trigger.Test.Controller
{
    public class LogoutControllerTest
    {
        [Fact]
        public void To_Verify_Delete_Method()
        {
            // Arrange
            var iActionPermission = new Mock<IActionPermission>();
            var logger = new Mock<ILogger<BLL_Login>>();
            var loggerNotification = new Mock<ILogger<BLL_Notification>>();
            var appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            var iConnectionContext = new Mock<IConnectionContext>();
            var notification = new Mock<BLL_Notification>(iConnectionContext.Object, loggerNotification.Object, appSettings);
            var login = new Mock<BLL_Login>(iConnectionContext.Object, logger.Object, iActionPermission.Object, notification.Object,appSettings);
            login.Setup(x => x.InvokeDeleteDeviceInfo(It.IsAny<string>())).Returns(TestUtility.GetJsonData(TestUtility.GetJsonData(TestUtility.GetUserLoginModel())));
            LogoutController logoutController = new LogoutController(login.Object);

            // Act
            var result = logoutController.DeleteAsync(It.IsAny<string>());

            // Assert
            Assert.IsType<JsonData>(result);
        }
    }
}
