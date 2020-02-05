using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Role
{
    public class RoleTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAllRole_WithException(bool isException)
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var logger = new Mock<ILogger<Trigger.BLL.Role.Role>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            var triggerContext = new Mock<TriggerContext>(serviceProvider);
            iConnectionContext.Setup(x => x.TriggerContext).Returns(triggerContext.Object);
            if (!isException)
            {
                iConnectionContext.Setup(x => x.TriggerContext.RoleRepository.SelectAll()).Returns(TestUtility.GetRoleModels());
            }
            var role = new Trigger.BLL.Role.Role(iConnectionContext.Object, logger.Object);

            // Act
            var result = role.GetAllRoleAsync();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
