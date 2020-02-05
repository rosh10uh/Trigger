using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Role = Trigger.BLL.Role.Role;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller
{
    public class RoleControllerTest
    {
        [Theory]
        [InlineData(1)]
        public void To_Verify_Get_Method_ReturnRoleList(int companyId)
        {
            // Arrange
            var setupTest = new SetupTest();
            setupTest.Setup();
            var logger = new Mock<ILogger<BLL_Role>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            var role = new Mock<BLL_Role>(iConnectionContext.Object, logger.Object);
            role.Setup(x => x.GetAllRoleAsync()).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetRoleModels()));
            var roleController = new RoleController(role.Object);

            // Act
            var result = roleController.Get(companyId);

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
