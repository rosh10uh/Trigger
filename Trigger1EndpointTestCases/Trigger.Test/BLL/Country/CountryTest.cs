using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Country
{
    public class CountryTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAllCountryAsync_WithException(bool isException)
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<Trigger.BLL.Country.Country>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            if (!isException)
            {
                catalogDbContext.Setup(x => x.CountryRepository.SelectAll()).Returns(TestUtility.GetCountryModels());
            }
            var country = new Trigger.BLL.Country.Country(catalogDbContext.Object, logger.Object);

            // Act
            var result = country.GetAllCountryAsync();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}

