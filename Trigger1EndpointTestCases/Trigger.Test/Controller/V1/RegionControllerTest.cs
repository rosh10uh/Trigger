using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Region = Trigger.BLL.Region.Region;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller
{
    public class RegionControllerTest
    {
        [Fact]
        public void To_Verify_Get_Method_ReturnRegionList()
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<BLL_Region>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            var region = new Mock<BLL_Region>(catalogDbContext.Object, logger.Object);
            region.Setup(x => x.GetAllRegionAsync()).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetRegionModels()));
            var regionController = new RegionController(region.Object);

            // Act
            var result = regionController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
