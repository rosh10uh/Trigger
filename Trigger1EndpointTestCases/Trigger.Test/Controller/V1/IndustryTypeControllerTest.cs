using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_IndustryType = Trigger.BLL.IndustryType.IndustryType;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
namespace Trigger.Test.Controller
{
    public class IndustryTypeControllerTest
    {
        [Fact]
        public void To_Verify_Get_Method_ReturnIndustryTypeList()
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<BLL_IndustryType>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            var industryType = new Mock<BLL_IndustryType>(catalogDbContext.Object, logger.Object);
            industryType.Setup(x => x.SelectAllAsync()).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetIndustryModels()));
            var industryTypeController = new IndustryTypeController(industryType.Object);

            // Act
            var result = industryTypeController.Get();

            // Assert
            Assert.IsType<Task<ActionResult<CustomJsonData>>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
