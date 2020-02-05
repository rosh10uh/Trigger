using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Country = Trigger.BLL.Country.Country;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller
{
    public class CountryControllerTest
    {
        [Fact]
        public void To_Verify_Get_Method_ReturnCountryList()
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<BLL_Country>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            var country = new Mock<BLL_Country>(catalogDbContext.Object, logger.Object);
            country.Setup(x => x.GetAllCountryAsync()).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetCountryModels()));
            var countryController = new CountryController(country.Object);

            // Act
            var result = countryController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
