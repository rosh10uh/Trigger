using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.Login;
using Trigger.Controllers.V2;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Notification = Trigger.BLL.Notification.Notification;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller.V2
{
    public class LoginControllerTest
    {
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<ILogger<Login>> _logger;
        private readonly Mock<Login> _loginBll;
        private readonly Mock<BLL_Notification> _notification;
        private readonly Mock<IActionPermission> _actionPermission;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<ILogger<BLL_Notification>> _notificationBll;
        private readonly SetupTest _setupTest;
        private readonly Mock<IHttpContextAccessor> _iHttpContextAccessor;

        public LoginControllerTest()
        {
            _setupTest = new SetupTest();
            _iConnectionContext = new Mock<IConnectionContext>();
            _iHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _logger = new Mock<ILogger<Login>>();
            _actionPermission = new Mock<IActionPermission>();
            _notificationBll = new Mock<ILogger<BLL_Notification>>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _notification = new Mock<BLL_Notification>(_iConnectionContext.Object, _notificationBll.Object, _appSettings);
            _loginBll = new Mock<Login>(_iConnectionContext.Object,_logger.Object, _actionPermission.Object, _notification.Object, _appSettings);
        }

        [Fact]
        public void To_Verify_PostAsync()
        {
            //Arrange
            _loginBll.Setup(x => x.InvokeLogin(It.IsAny<UserLoginModel>(), It.IsAny<string>())).Returns(TestUtility.GetJsonData(TestUtility.GetUserDataModel()));
            _iHttpContextAccessor.Setup(_ => _.HttpContext).Returns(new DefaultHttpContext());
            var loginController = new LoginController(_loginBll.Object);

            //Act
            var result = loginController.PostAsync(It.IsAny<UserLoginModel>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result);
        }
    }
}
