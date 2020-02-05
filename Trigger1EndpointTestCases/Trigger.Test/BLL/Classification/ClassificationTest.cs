using Microsoft.Extensions.Logging;
using Moq;
using Trigger.DAL.Shared.Interfaces;
using Xunit;
using Bll_Classification = Trigger.BLL.Spark.Classification;

namespace Trigger.Test.BLL.Classification
{
    public class ClassificationTest
    {
        [Fact]
        public void To_Verify_Get_All_Classifications()
        {
            // Arrange
            var logger = new Mock<ILogger<Bll_Classification>>();
            var iConnectionContext = new Mock<IConnectionContext>();
            var classification = new Mock<Bll_Classification>(iConnectionContext.Object, logger.Object);
            
            // Act
            var result = classification.Setup(x => x.GetAllClassifications());

            // Assert
            Assert.NotNull(result);
        }
    }
}
