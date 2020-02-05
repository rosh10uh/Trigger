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
    public class DimensionTest
    {
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<ILogger<Dimension>> _logger;
        private readonly Mock<Dimension> _dimension;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;

        public DimensionTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _logger = new Mock<ILogger<Dimension>>();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _dimension = new Mock<Dimension>(_iConnectionContext.Object, _logger.Object);

        }

        [Fact]
        public void To_Verify_Get_Dimension_Elements()
        {
            // Arrange
            _triggerCatalogContext.Setup(x=>x.DimensionRepository.SelectAll());
            // Act
            var result = _dimension.Setup(x => x.GetAllDimension());

            // Assert
            Assert.NotNull(result);
        }
        
        [Fact]
        public void To_Verify_Add_Dimension()
        {
            // Arrange
            _triggerCatalogContext.Setup(x => x.DimensionRepository.Insert(It.IsAny<DimensionModel>()));
            // Act
            var result = _dimension.Setup(x => x.AddDimension(It.IsAny<DimensionModel>()));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Update_Dimension()
        {
            // Arrange
            _triggerCatalogContext.Setup(x => x.DimensionRepository.Update(It.IsAny<DimensionModel>()));
            // Act
            var result = _dimension.Setup(x => x.UpdateDimension(It.IsAny<DimensionModel>()));

            // Assert
            Assert.NotNull(result);
        }
    }
}
