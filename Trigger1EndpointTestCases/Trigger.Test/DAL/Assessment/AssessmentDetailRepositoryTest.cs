using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.Assessment
{
    public class AssessmentDetailRepositoryTest : BaseRepositoryTest
    {
        public AssessmentDetailRepositoryTest() : base("AssessmentDetailRepository")
        {

        }

        [Fact]
        public void Verify_To_InsertAssessmentDetails_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<List<EmpAssessmentDet>, List<EmpAssessmentDet>, TriggerContext>(TestUtility.GetEmpAssessmentDets()).BuildServiceProvider());

            //Act
            var result = triggerContext.AssessmentDetailRepository.InsertAssessmentDetails(It.IsAny<List<EmpAssessmentDet>>());

            //Assert      
            Assert.IsType<List<EmpAssessmentDet>>(result);
            Assert.NotNull(result);
        }
    }
}
