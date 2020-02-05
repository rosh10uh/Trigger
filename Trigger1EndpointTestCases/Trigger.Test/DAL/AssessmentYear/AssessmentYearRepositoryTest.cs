using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.AssessmentYear
{
    public class AssessmentYearRepositoryTest : BaseRepositoryTest
    {
        public AssessmentYearRepositoryTest() : base("AssessmentYearRepository")
        {

        }

        [Fact]
        public void To_Verify_GetAssessmentYear_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<AssessmentYearModel, List<AssessmentYearModel>, TriggerContext>(TestUtility.GetAssessmentYearModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.AssessmentYearRepository.GetAssessmentYear(It.IsAny<AssessmentYearModel>());

            //Assert      
            Assert.IsType<List<AssessmentYearModel>>(result);
            Assert.NotNull(result);
        }
    }
}
