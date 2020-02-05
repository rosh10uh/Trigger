using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Widget
{
    public class WidgetTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetUserWiseWidget_WithException(bool isException)
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var triggerContext = new Mock<TriggerContext>(serviceProvider);
            var logger = new Mock<ILogger<Trigger.BLL.Widget.Widget>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            iConnectionContext.SetupGet(x => x.TriggerContext).Returns(triggerContext.Object);
            if (!isException)
            {
                iConnectionContext.Setup(x => x.TriggerContext.WidgetLibraryRepository.GetUserwiseWidget(It.IsAny<WidgetLibrary>())).Returns(TestUtility.GetWidgetLibraries());
            }

            var widget = new Trigger.BLL.Widget.Widget(iConnectionContext.Object, logger.Object);

            // Act
            var result = widget.GetUserwiseWidgetAsync(1, 1);

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_AddWidgetPosition_WithInsertedWidget(int insertWidgetCount)
        {
            // Arrange
            List<WidgetLibrary> widgetLibraries;
            widgetLibraries = insertWidgetCount == 1 ? TestUtility.GetWidgetLibraries() : new List<WidgetLibrary>();

            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var triggerContext = new Mock<TriggerContext>(serviceProvider);
            var logger = new Mock<ILogger<Trigger.BLL.Widget.Widget>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            iConnectionContext.SetupGet(x => x.TriggerContext).Returns(triggerContext.Object);
            iConnectionContext.Setup(x => x.TriggerContext.WidgetPositionRepository.Insert(It.IsAny<List<WidgetLibrary>>())).Returns(widgetLibraries);
            var widget = new Trigger.BLL.Widget.Widget(iConnectionContext.Object, logger.Object);

            // Act
            var result = widget.AddWidgetPositionAsync(TestUtility.GetWidgetLibraries());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddWidgetPosition_WithException(bool isException)
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var triggerContext = new Mock<TriggerContext>(serviceProvider);
            var logger = new Mock<ILogger<Trigger.BLL.Widget.Widget>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            iConnectionContext.SetupGet(x => x.TriggerContext).Returns(triggerContext.Object);
            if (!isException)
            {
                iConnectionContext.Setup(x => x.TriggerContext.WidgetPositionRepository.Insert(It.IsAny<List<WidgetLibrary>>())).Returns(TestUtility.GetWidgetLibraries());
            }

            var widget = new Trigger.BLL.Widget.Widget(iConnectionContext.Object, logger.Object);

            // Act
            var result = widget.AddWidgetPositionAsync(TestUtility.GetWidgetLibraries());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
