using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Text;
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
    public class LoginControllerTest
    {
       private readonly Mock<IHttpContextAccessor> _mockHttp = null;
       private readonly Mock<ISession> _mockSession = null;

        public LoginControllerTest()
        {
            _mockHttp = new Mock<IHttpContextAccessor>();
            _mockSession = new Mock<ISession>();
            _mockHttp.Setup(x => x.HttpContext.Session).Returns(_mockSession.Object);
        }

        [Fact]
        public void To_Verify_Post_Method()
        {
            // Arrange
            var setupTest = new SetupTest();
            setupTest.Setup();
            var iActionPermission = new Mock<IActionPermission>();
            var loggerNotification = new Mock<ILogger<BLL_Notification>>();
            var logger = new Mock<ILogger<BLL_Login>>();
            var appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            var iConnectionContext = new Mock<IConnectionContext>();
            var context = new DefaultHttpContext();
            var mockSession = new Mock<ISession>();
            byte[] sessionValue = Encoding.UTF8.GetBytes("test");
            mockSession.Setup(x => x.TryGetValue(It.IsAny<string>(), out sessionValue)).Returns(true).Verifiable();
            context.Response.Body = new MemoryStream();
            var notification = new Mock<BLL_Notification>(iConnectionContext.Object, loggerNotification.Object, appSettings);
            var login = new Mock<BLL_Login>(iConnectionContext.Object, logger.Object, iActionPermission.Object, notification.Object, appSettings);
            login.Setup(x => x.InvokeLogin(It.IsAny<UserLoginModel>(), It.IsAny<string>())).Returns(TestUtility.GetJsonData(TestUtility.GetUserDataModel()));
            var loginController = new LoginController(login.Object);

            // Act
            var result = loginController.PostAsync(It.IsAny<UserLoginModel>());

            // Assert
            Assert.IsType<JsonData>(result);
        }
    }
}
