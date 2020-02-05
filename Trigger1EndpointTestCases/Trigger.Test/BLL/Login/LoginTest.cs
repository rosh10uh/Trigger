using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using BLL_Login = Trigger.BLL.Login.Login;
using BLL_Notification = Trigger.BLL.Notification.Notification;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Login
{
    public class LoginTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<ILogger<BLL_Login>> _logger;
        private readonly Mock<ILogger<BLL_Notification>> _loggerNotification;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<IActionPermission> _iActionPermission;
        private readonly Mock<BLL_Notification> _notification;
        private readonly IOptions<AppSettings> _appSettings;

        public LoginTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup(true);
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _logger = new Mock<ILogger<BLL_Login>>();
            _loggerNotification = new Mock<ILogger<BLL_Notification>>();
            _iActionPermission = new Mock<IActionPermission>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _notification = new Mock<BLL_Notification>(_iConnectionContext.Object, _loggerNotification.Object, _appSettings);
            _notification.Setup(x => x.InvokeNotificationAsync(It.IsAny<EmployeeModel>()));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_InvokeLogin_Method_WithLoginError(bool haveLoginError)
        {
            // Arrange
            UserDataModel userDataModel = TestUtility.GetUserDataModel();
            userDataModel.Error = haveLoginError ? "Getting Error." : null;

            _iConnectionContext.Setup(x => x.TriggerContext.LoginRepository.InvokeLogin(It.IsAny<UserDataModel>())).Returns(userDataModel);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _iConnectionContext.Setup(x => x.TriggerContext.UserLoginModelRepository.RegisterUserDeviceInfo(It.IsAny<UserLoginModel>())).Returns(TestUtility.GetUserLoginModel());
            BLL_Login login = new BLL_Login(_iConnectionContext.Object, _logger.Object, _iActionPermission.Object, _notification.Object, _appSettings);

            // Act
            var result = login.InvokeLogin(TestUtility.GetUserLoginModel(), "1.0");

            // Assert
            Assert.IsType<JsonData>(result);
            Assert.NotNull(result);

        }

        [Theory]
        [InlineData("Trigger Admin", 1)]
        [InlineData("Company Admin", 2)]
        public void To_Verify_InvokeLogin_Method_Role(string role, int roleId)
        {
            // Arrange
            UserDataModel userDataModel = TestUtility.GetUserDataModel();
            userDataModel.roleId = roleId;
            userDataModel.role = role;

            _iConnectionContext.Setup(x => x.TriggerContext.LoginRepository.InvokeLogin(It.IsAny<UserDataModel>())).Returns(userDataModel);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _iConnectionContext.Setup(x => x.TriggerContext.UserLoginModelRepository.RegisterUserDeviceInfo(It.IsAny<UserLoginModel>())).Returns(TestUtility.GetUserLoginModel());
            BLL_Login login = new BLL_Login(_iConnectionContext.Object, _logger.Object, _iActionPermission.Object, _notification.Object, _appSettings);

            // Act
            var result = login.InvokeLogin(TestUtility.GetUserLoginModel(), "1.0");

            // Assert
            Assert.IsType<JsonData>(result);

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_InvokeLogin_Method_WithDevices(bool isWithDevice)
        {
            // Arrange
            UserDataModel userDataModel = TestUtility.GetUserDataModel();
            userDataModel.roleId = isWithDevice ? 2 : 1;

            _iConnectionContext.Setup(x => x.TriggerContext.LoginRepository.InvokeLogin(It.IsAny<UserDataModel>())).Returns(userDataModel);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _iConnectionContext.Setup(x => x.TriggerContext.UserLoginModelRepository.RegisterUserDeviceInfo(It.IsAny<UserLoginModel>())).Returns(TestUtility.GetUserLoginModel());
            BLL_Login login = new BLL_Login(_iConnectionContext.Object, _logger.Object, _iActionPermission.Object, _notification.Object, _appSettings);

            // Act
            UserLoginModel userLoginModel = TestUtility.GetUserLoginModel();
            userLoginModel.deviceID = isWithDevice ? "abc123" : null;
            var result = login.InvokeLogin(userLoginModel, "1.0");

            // Assert
            Assert.IsType<JsonData>(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_InvokeLogin_WithException(bool isException)
        {
            // Arrange
            UserDataModel userDataModel = TestUtility.GetUserDataModel();
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.LoginRepository.InvokeLogin(It.IsAny<UserDataModel>())).Returns(userDataModel);
                _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
                _iConnectionContext.Setup(x => x.TriggerContext.UserLoginModelRepository.RegisterUserDeviceInfo(It.IsAny<UserLoginModel>())).Returns(TestUtility.GetUserLoginModel());
            }
            BLL_Login login = new BLL_Login(_iConnectionContext.Object, _logger.Object, _iActionPermission.Object, _notification.Object, _appSettings);

            // Act
            var result = login.InvokeLogin(TestUtility.GetUserLoginModel(), "1.0");

            // Assert
            Assert.IsType<JsonData>(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_InvokeDeleteDeviceInfo_Method_IsDeviceDeleted(bool isDeviceDeleted)
        {
            // Arrange
            UserLoginModel userLoginModel = TestUtility.GetUserLoginModel();
            userLoginModel.result = isDeviceDeleted ? 1 : 0;

            _iConnectionContext.Setup(x => x.TriggerContext.UserLoginModelRepository.invokeDeleteDeviceInfo(It.IsAny<UserLoginModel>())).Returns(userLoginModel);
            BLL_Login login = new BLL_Login(_iConnectionContext.Object, _logger.Object, _iActionPermission.Object, _notification.Object, _appSettings);

            // Act
            var result = login.InvokeDeleteDeviceInfo(It.IsAny<string>());

            // Assert
            Assert.IsType<JsonData>(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_InvokeDeleteDeviceInfo_Method_WithException(bool isException)
        {
            // Arrange
            UserLoginModel userLoginModel = TestUtility.GetUserLoginModel();
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.UserLoginModelRepository.invokeDeleteDeviceInfo(It.IsAny<UserLoginModel>())).Returns(userLoginModel);
            }
            BLL_Login login = new BLL_Login(_iConnectionContext.Object, _logger.Object, _iActionPermission.Object, _notification.Object, _appSettings);

            // Act
            var result = login.InvokeDeleteDeviceInfo(It.IsAny<string>());

            // Assert
            Assert.IsType<JsonData>(result);
        }
    }
}
