using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Trigger.BLL.DimensionMatrix;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.Test.Shared;

namespace Trigger.Test.BLL.ActionwisePermissions
{
    public class ActionwisePermissionTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<ILogger<ActionwisePermission>> _logger;
        private readonly Mock<ActionwisePermission> _actionwisePermission;
        private readonly Mock<IActionPermission> _iActionPermission;

        public ActionwisePermissionTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _logger = new Mock<ILogger<ActionwisePermission>>();
            _iActionPermission = new Mock<IActionPermission>();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _actionwisePermission = new Mock<ActionwisePermission>(_iConnectionContext.Object, _triggerCatalogContext.Object, _logger.Object, _iActionPermission.Object);
        }

    }
}
