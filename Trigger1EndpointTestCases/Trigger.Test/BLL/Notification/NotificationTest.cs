using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Notification = Trigger.BLL.Notification.Notification;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Notification
{
    public class NotificationTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_Notification>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<BLL_Notification> _notification;
        private readonly IOptions<AppSettings> _appSettings;


        public NotificationTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<BLL_Notification>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _notification = new Mock<BLL_Notification>(_iConnectionContext.Object, _logger.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_GetAllNotification_WithNotificationCount(int notificationCount)
        {
            // Arrange
            List<NotificationModel> notificationModels = notificationCount == 0 ? new List<NotificationModel>() : TestUtility.GetNotificationModels();
            _iConnectionContext.Setup(x => x.TriggerContext.NotificationRepository.GetAllNotification(It.IsAny<NotificationModel>())).Returns(notificationModels);
            var notification = new BLL_Notification(_iConnectionContext.Object, _logger.Object, _appSettings);

            // Act
            var result = notification.GetAllNotification(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_Get_AllNotification_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.NotificationRepository.GetAllNotification(It.IsAny<NotificationModel>())).Returns(TestUtility.GetNotificationModels());
            }
            var notification = new BLL_Notification(_iConnectionContext.Object, _logger.Object, _appSettings);

            // Act
            var result = notification.GetAllNotification(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_UpdateNotificationMarkAsReadAsync_WithNotificationResult(int notificationResult)
        {
            // Arrange
            NotificationModel notificationModel = TestUtility.GetNotificationModel();
            notificationModel.result = notificationResult;
            _iConnectionContext.Setup(x => x.TriggerContext.NotificationRepository.InvokeUpdateNotificationStatus(It.IsAny<NotificationModel>())).Returns(notificationModel);
            var notification = new BLL_Notification(_iConnectionContext.Object, _logger.Object, _appSettings);

            // Act
            var result = notification.UpdateNotificationMarkAsReadAsync(It.IsAny<string>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_Get_UpdateNotificationMarkAsReadAsync_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.NotificationRepository.InvokeUpdateNotificationStatus(It.IsAny<NotificationModel>())).Returns(TestUtility.GetNotificationModel());
            }
            var notification = new BLL_Notification(_iConnectionContext.Object, _logger.Object, _appSettings);

            // Act
            var result = notification.UpdateNotificationMarkAsReadAsync(It.IsAny<string>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
