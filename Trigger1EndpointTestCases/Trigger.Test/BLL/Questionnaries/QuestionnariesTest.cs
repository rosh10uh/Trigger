using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Questionnaries = Trigger.BLL.Questionnaries.Questionnaries;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Questionnaries
{
    public class QuestionnariesTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_Questionnaries>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;


        public QuestionnariesTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<BLL_Questionnaries>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAllQuestionnaries_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.QuestionnariesRepository.InvokeGetQuestionnaries()).Returns(TestUtility.GetQuestionAnswers());
            }
            var questionnaries = new BLL_Questionnaries(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = questionnaries.GetAllQuestionnaries();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_GetAllQuestionnaries_WithRecordCount(int recordCount)
        {
            // Arrange
            List<QuestionAnswer> questionAnswers = recordCount == 0 ? new List<QuestionAnswer>() : TestUtility.GetQuestionAnswers();
            _iConnectionContext.Setup(x => x.TriggerContext.QuestionnariesRepository.InvokeGetQuestionnaries()).Returns(questionAnswers);
            var questionnaries = new BLL_Questionnaries(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = questionnaries.GetAllQuestionnaries();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
