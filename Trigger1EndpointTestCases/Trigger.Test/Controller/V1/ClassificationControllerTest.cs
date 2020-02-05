using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.Spark;
using Trigger.Controllers.V1;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Xunit;

namespace Trigger.Test.Controller.V1
{
    public class ClassificationControllerTest
    {
        [Fact]
        public void To_Verify_Get()
        {
            // Arrange
            var logger = new Mock<ILogger<Classification>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            var classification = new Mock<Classification>(iConnectionContext.Object, logger.Object);
            classification.Setup(x=>x.GetAllClassifications());
            var classificationController = new ClassificationController(classification.Object);

            // Act
            var result = classificationController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result);
        }
    }
}
