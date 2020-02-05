using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Ethnicity = Trigger.BLL.Ethnicity.Ethnicity;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
namespace Trigger.Test.Controller
{
    public class EthnicityControllerTest
    {
        [Fact]
        public void To_Verify_Get_Method_ReturnEthnicityList()
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<BLL_Ethnicity>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            var ethnicity = new Mock<BLL_Ethnicity>(catalogDbContext.Object, logger.Object);
            ethnicity.Setup(x => x.GetAllEthnicityAsync()).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetRaceOrEthnicityModels()));
            var ethnicityController = new EthnicityController(ethnicity.Object);

            // Act
            var result = ethnicityController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
