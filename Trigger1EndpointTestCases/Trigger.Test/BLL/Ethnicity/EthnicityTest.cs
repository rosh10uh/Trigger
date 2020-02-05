using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;


namespace Trigger.Test.BLL.Ethnicity
{
    public class EthnicityTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAllEthnicity_WithException(bool isException)
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<Trigger.BLL.Ethnicity.Ethnicity>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            if (!isException)
            {
                catalogDbContext.Setup(x => x.EthnicityRepository.SelectAll()).Returns(TestUtility.GetRaceOrEthnicityModels());
            }
            var ethnicity = new Trigger.BLL.Ethnicity.Ethnicity(catalogDbContext.Object, logger.Object);

            // Act
            var result = ethnicity.GetAllEthnicityAsync();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
