using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.DimensionMatrix;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.Controller.V1
{
    public class ActionPermissionControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<ILogger<ActionwisePermission>> _logger;
        private readonly Mock<ActionwisePermission> _actionwisePermission;
        private readonly Mock<IActionPermission> _iActionPermission;

        public ActionPermissionControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _logger = new Mock<ILogger<ActionwisePermission>>();
            _iActionPermission = new Mock<IActionPermission>();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _actionwisePermission = new Mock<ActionwisePermission>(_iConnectionContext.Object,_triggerCatalogContext.Object, _logger.Object,_iActionPermission.Object);
        }

        [Fact]
        public void To_Verify_Get()
        {
            // Arrange
            _actionwisePermission.Setup(x => x.GetActionwisePermission());
            var actionwisePermissionController = new ActionPermissionController(_actionwisePermission.Object);

            // Act
            var result = actionwisePermissionController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Post_Async()
        {
            // Arrange
            _actionwisePermission.Setup(x => x.AddActionwisePermission(It.IsAny<List<ActionwisePermissionModel>>()));
            var actionwisePermissionController = new ActionPermissionController(_actionwisePermission.Object);

            // Act
            var result = actionwisePermissionController.PostAsync(It.IsAny<List<ActionwisePermissionModel>>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Get_All_Actions_Async()
        {
            // Arrange
            _actionwisePermission.Setup(x => x.GetAllActionsAsync());
            var actionwisePermissionController = new ActionPermissionController(_actionwisePermission.Object);

            // Act
            var result = actionwisePermissionController.GetAllActionsAsync();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Check_With_Existing_Action_Permission()
        {
            // Arrange
            _actionwisePermission.Setup(x => x.CheckWithExistingActionPermission(It.IsAny<int>(), It.IsAny<List<ActionList>>()));
            var actionwisePermissionController = new ActionPermissionController(_actionwisePermission.Object);

            // Act
            var result = actionwisePermissionController.CheckWithExistingActionPermission(It.IsAny<int>(),It.IsAny<List<ActionList>>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }
    }
}
