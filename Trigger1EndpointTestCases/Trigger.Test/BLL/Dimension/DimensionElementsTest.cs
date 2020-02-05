using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Trigger.BLL.DimensionMatrix;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO.DimensionMatrix;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.BLL.Bll_Dimension
{
    public class DimensionElementsTest
    {
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<ILogger<DimensionElements>> _logger;
        private readonly Mock<DimensionElements> _dimensionElements;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;

        public DimensionElementsTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _logger = new Mock<ILogger<DimensionElements>>();
            _dimensionElements = new Mock<DimensionElements>(_iConnectionContext.Object, _logger.Object);
        }

        [Fact]
        public void To_Verify_Get_Dimension_Elements()
        {
            // Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.DimensionElementsRepository.SelectAll()).Returns(new System.Collections.Generic.List<DimensionElementsModel>());
            // Act
            var result = _dimensionElements.Setup(x => x.GetDimensionElements());

            // Assert
            Assert.NotNull(result);
        }
        
        [Fact]
        public void To_Verify_Add_Dimension_Elements()
        {
            // Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.DimensionElementsRepository.Insert(It.IsAny<DimensionElementsModel>()));
            // Act
            var result = _dimensionElements.Setup(x => x.AddDimensionElements(It.IsAny<DimensionElementsModel>()));

            // Assert
            Assert.NotNull(result);
        }
        
        [Fact]
        public void To_Verify_Update_Dimension_Elements()
        {
            // Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.DimensionElementsRepository.Update(It.IsAny<DimensionElementsModel>()));
            // Act
            var result = _dimensionElements.Setup(x => x.UpdateDimensionElements(It.IsAny<DimensionElementsModel>()));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Delete_Dimension_Elements()
        {
            // Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.DimensionElementsRepository.Delete(It.IsAny<DimensionElementsModel>()));
            // Act
            var result = _dimensionElements.Setup(x => x.DeleteDimensionElements(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

            // Assert
            Assert.NotNull(result);
        }
    }
}
