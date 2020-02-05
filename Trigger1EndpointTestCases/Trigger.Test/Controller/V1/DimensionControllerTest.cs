using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
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
    public class DimensionControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<ILogger<Dimension>> _logger;
        private readonly Mock<Dimension> _dimension;

        public DimensionControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _logger = new Mock<ILogger<Dimension>>();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _dimension= new Mock<Dimension>(_triggerCatalogContext.Object, _logger.Object);
        }

        [Fact]
        public void To_Verify_Get()
        {
            // Arrange
            _dimension.Setup(x => x.GetAllDimension());
            var dimensionController = new DimensionController(_dimension.Object);

            // Act
            var result = dimensionController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        
        [Fact]
        public void To_Verify_Post_Async()
        {
            // Arrange
            _dimension.Setup(x => x.AddDimension(It.IsAny<DimensionModel>()));
            var dimensionController = new DimensionController(_dimension.Object);

            // Act
            var result = dimensionController.PostAsync(It.IsAny<DimensionModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Put_Async()
        {
            // Arrange
            _dimension.Setup(x => x.UpdateDimension(It.IsAny<DimensionModel>()));
            var dimensionController = new DimensionController(_dimension.Object);

            // Act
            var result = dimensionController.PutAsync(It.IsAny<DimensionModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result);
        }
    }
}
