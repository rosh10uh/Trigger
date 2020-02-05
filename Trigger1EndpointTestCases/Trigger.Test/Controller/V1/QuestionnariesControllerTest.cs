using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Questionnaries = Trigger.BLL.Questionnaries.Questionnaries;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
namespace Trigger.Test.Controller
{
    public class QuestionnariesControllerTest
    {
        [Fact]
        public void To_Verify_Get_Method_ReturnQuestionAnswerList()
        {
            // Arrange
            var _setupTest = new SetupTest();
            _setupTest.Setup();
            var _logger = new Mock<ILogger<BLL_Questionnaries>>();
            var _iConnectionContext = new Mock<IConnectionContext>();
            var questionnaries = new Mock<BLL_Questionnaries>(_iConnectionContext.Object, _logger.Object);
            questionnaries.Setup(x => x.GetAllQuestionnaries()).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetQuestionAnswers()));
            var questionnariesController = new QuestionnariesController(questionnaries.Object);

            // Act
            var result = questionnariesController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
