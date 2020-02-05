using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.DimensionMatrix;
using Trigger.Controllers.V1;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Xunit;

namespace Trigger.Test.Controller.V1
{
    public class DimensionElementsControllerTest
    {
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<ILogger<DimensionElements>> _logger;
        private readonly Mock<DimensionElements> _dimensionElements;

        public DimensionElementsControllerTest()
        {
            _iConnectionContext = new Mock<IConnectionContext>();
            _logger = new Mock<ILogger<DimensionElements>>();
            _dimensionElements = new Mock<DimensionElements>(_iConnectionContext.Object,_logger.Object);
        }

        [Fact]
        public void To_Verify_Get()
        {
            // Arrange
            _dimensionElements.Setup(x => x.GetDimensionElements());
            var dimensionElementsController = new DimensionElementsController(_dimensionElements.Object);

            // Act
            var result = dimensionElementsController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Post_Async()
        {
            // Arrange
            _dimensionElements.Setup(x => x.AddDimensionElements(It.IsAny<DimensionElementsModel>()));
            var dimensionElementsController = new DimensionElementsController(_dimensionElements.Object);

            // Act
            var result = dimensionElementsController.PostAsync(It.IsAny<DimensionElementsModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Put_Async()
        {
            // Arrange
            _dimensionElements.Setup(x => x.UpdateDimensionElements(It.IsAny<DimensionElementsModel>()));
            var dimensionElementsController = new DimensionElementsController(_dimensionElements.Object);

            // Act
            var result = dimensionElementsController.PutAsync(It.IsAny<DimensionElementsModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Delete_Async()
        {
            // Arrange
            _dimensionElements.Setup(x => x.DeleteDimensionElements(It.IsAny<int>(), It.IsAny<int>(),It.IsAny<int>()));
            var dimensionElementsController = new DimensionElementsController(_dimensionElements.Object);

            // Act
            var result = dimensionElementsController.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result);
        }
    }
}
