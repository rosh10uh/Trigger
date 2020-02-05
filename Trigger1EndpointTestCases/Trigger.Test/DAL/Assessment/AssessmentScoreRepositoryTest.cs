using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.Assessment
{
    public class AssessmentScoreRepositoryTest : BaseRepositoryTest
    {
        public AssessmentScoreRepositoryTest() : base("AssessmentScoreRepository")
        {

        }

        [Fact]
        public void To_Verify_GetAssessmentScore_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<AssessmentScoreModel, AssessmentScoreModel, TriggerContext>(TestUtility.GetAssessmentScoreModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>());

            //Assert      
            Assert.IsType<AssessmentScoreModel>(result);
            Assert.NotNull(result);
        }
    }
}
