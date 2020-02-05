using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.Notification
{
    public class NotificationRepositoryTest : BaseRepositoryTest
    {
        public NotificationRepositoryTest() : base("NotificationRepository")
        {

        }

        [Fact]
        public void To_Verify_UpdateNotificationFlagIsSent()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<NotificationModel, NotificationModel, TriggerContext>(TestUtility.GetNotificationModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.NotificationRepository.UpdateNotificationFlagIsSent(It.IsAny<NotificationModel>());

            //Assert      
            Assert.IsType<NotificationModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_InvokeUpdateNotificationStatus()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<NotificationModel, NotificationModel, TriggerContext>(TestUtility.GetNotificationModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.NotificationRepository.InvokeUpdateNotificationStatus(It.IsAny<NotificationModel>());

            //Assert      
            Assert.IsType<NotificationModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllNotification_ReturnListOfNotification()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<NotificationModel, List<NotificationModel>, TriggerContext>(TestUtility.GetNotificationModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.NotificationRepository.GetAllNotification(It.IsAny<NotificationModel>());

            //Assert      
            Assert.IsType<List<NotificationModel>>(result);
            Assert.NotNull(result);
        }
    }
}
