using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_AssessmentYear = Trigger.BLL.AssessmentYear.AssessmentYear;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller
{
    public class AssessmentYearControllerTest
    {
        [Fact]
        public void To_Verify_Get_Method_ReturnAssessmetYearList()
        {
            //Arrange
            var setupTest = new SetupTest();
            setupTest.Setup();
            var logger = new Mock<ILogger<BLL_AssessmentYear>>();
            var iConnection = new Mock<IConnectionContext>();
            var assessmentYear = new Mock<BLL_AssessmentYear>(iConnection.Object, logger.Object);
            assessmentYear.Setup(x => x.GetAssessmentYearAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetAssessmentYearModels()));
            var assessmentYearController = new AssessmentYearController(assessmentYear.Object);

            //Act
            var result = assessmentYearController.Get(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

    }
}
