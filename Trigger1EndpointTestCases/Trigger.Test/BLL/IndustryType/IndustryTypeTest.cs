using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.IndustryType
{
    public class IndustryTypeTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAllIndustryType_WithException(bool isException)
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<Trigger.BLL.IndustryType.IndustryType>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            if (!isException)
            {
                catalogDbContext.Setup(x => x.IndustryTypeRepository.SelectAll()).Returns(TestUtility.GetIndustryModels());
            }
            var industryType = new Trigger.BLL.IndustryType.IndustryType(catalogDbContext.Object, logger.Object);

            // Act
            var result = industryType.SelectAllAsync();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
