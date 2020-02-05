using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Widget = Trigger.BLL.Widget.Widget;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
namespace Trigger.Test.Controller
{
    public class WidgetControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<ILogger<BLL_Widget>> _logger;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<BLL_Widget> _widget;

        public WidgetControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _logger = new Mock<ILogger<BLL_Widget>>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext = new Mock<IConnectionContext>();
            _widget = new Mock<BLL_Widget>(_iConnectionContext.Object, _logger.Object);
        }

        [Theory]
        [InlineData(1, 1)]
        public void To_Verify_Get_Method_ReturnWidgetList(int widgetType, int userId)
        {
            // Arrange
            _widget.Setup(x => x.GetUserwiseWidgetAsync(widgetType, userId)).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetWidgetLibraries()));
            var widgetController = new WidgetController(_widget.Object);

            // Act
            var result = widgetController.Get(widgetType, userId);

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Post_Method_ReturnWidgetList()
        {
            // Arrange
            _widget.Setup(x => x.AddWidgetPositionAsync(It.IsAny<List<WidgetLibrary>>())).ReturnsAsync(TestUtility.GetJsonData());
            var widgetController = new WidgetController(_widget.Object);

            // Act
            var result = widgetController.Post(It.IsAny<List<WidgetLibrary>>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
