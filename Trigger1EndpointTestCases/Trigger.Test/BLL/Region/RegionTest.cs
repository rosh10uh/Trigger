using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility;
namespace Trigger.Test.BLL.Region
{
    public class RegionTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAllRegion_WithException(bool isException)
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<Trigger.BLL.Region.Region>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            if (!isException)
            {
                catalogDbContext.Setup(x => x.RegionRepository.SelectAll()).Returns(TestUtility.GetRegionModels());
            }

            var region = new Trigger.BLL.Region.Region(catalogDbContext.Object, logger.Object);

            // Act
            var result = region.GetAllRegionAsync();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
