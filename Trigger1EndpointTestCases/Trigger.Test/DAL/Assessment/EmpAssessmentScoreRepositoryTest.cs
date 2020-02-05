using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;


namespace Trigger.Test.DAL.Assessment
{
    public class EmpAssessmentScoreRepositoryTest : BaseRepositoryTest
    {
        public EmpAssessmentScoreRepositoryTest() : base("EmpAssessmentScoreRepository")
        {

        }

        [Fact]
        public void To_Verify_UpdateScoreRank_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmpAssessmentScore, EmpAssessmentScore, TriggerContext>(CommonUtility.GetEmpAssessmentScore()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmpAssessmentScoreRepository.UpdateScoreRank(It.IsAny<EmpAssessmentScore>());

            //Assert      
            Assert.IsType<EmpAssessmentScore>(result);
            Assert.NotNull(result);
        }
    }
}
