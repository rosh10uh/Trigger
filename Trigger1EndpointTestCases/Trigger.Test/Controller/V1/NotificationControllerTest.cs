using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Notification = Trigger.BLL.Notification.Notification;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller
{
    public class NotificationControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_Notification>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<BLL_Notification> _notification;
        private readonly IOptions<AppSettings> _appSettings;

        public NotificationControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<BLL_Notification>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _notification = new Mock<BLL_Notification>(_iConnectionContext.Object, _logger.Object, _appSettings);
        }

        [Fact]
        public void To_Verify_Get_Method_Return_ListOfNotification()
        {
            // Arrange
            _notification.Setup(x => x.GetAllNotification(It.IsAny<int>())).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetNotificationModels()));
            NotificationController notificationController = new NotificationController(_notification.Object);

            // Act
            var result = notificationController.Get(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Put_Method_Return_SuccessMessage()
        {
            // Arrange
            _notification.Setup(x => x.UpdateNotificationMarkAsReadAsync(It.IsAny<string>())).ReturnsAsync(TestUtility.GetJsonData());
            NotificationController notificationController = new NotificationController(_notification.Object);

            // Act
            var result = notificationController.PutAsync(It.IsAny<string>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
