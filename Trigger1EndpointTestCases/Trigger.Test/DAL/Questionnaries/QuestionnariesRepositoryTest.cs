using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.DAL.Questionnaries
{
    public class QuestionnariesRepositoryTest : BaseRepositoryTest
    {
        public QuestionnariesRepositoryTest() : base("QuestionnariesRepository")
        {

        }

        [Fact]
        public void To_Verify_GetQuestionnaries_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<QuestionAnswer, List<QuestionAnswer>, TriggerContext>(new List<QuestionAnswer>()).BuildServiceProvider());

            //Act
            var result = triggerContext.QuestionnariesRepository.InvokeGetQuestionnaries();

            //Assert      
            Assert.IsType<List<QuestionAnswer>>(result);
            Assert.NotNull(result);
        }
    }
}
