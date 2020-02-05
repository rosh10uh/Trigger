using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.Controllers.V2;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Test.Shared;
using Xunit;
using BLL_ActionwisePermission = Trigger.BLL.DimensionMatrix.ActionwisePermission;

namespace Trigger.Test.Controller.V2
{
    public class ActionPermissionControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly Mock<IActionPermission> _iActionPermission;
        private readonly Mock<BLL_ActionwisePermission> _actionwise;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<ILogger<BLL_ActionwisePermission>> _logger;

        public ActionPermissionControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iActionPermission = new Mock<IActionPermission>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _logger = new Mock<ILogger<BLL_ActionwisePermission>>();
            _actionwise = new Mock<BLL_ActionwisePermission>(_iConnectionContext.Object,_triggerCatalogContext.Object,_logger.Object,_iActionPermission.Object);
        }

        [Theory]
        [InlineData(1)]
        public void To_Check_With_Existing_Action_Permission(int empId)
        {
                //Arrange
                _actionwise.Setup(x => x.CheckWithExistingActionPermissionV2(It.IsAny<int>(), It.IsAny<List<ActionList>>()));
                _iActionPermission.Setup(x => x.GetPermissionsV2(empId));

                var actionPermissionController = new ActionPermissionController(_actionwise.Object);

                //Act
                var result = actionPermissionController.CheckWithExistingActionPermission(empId, new List<ActionList>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }
    }
}
