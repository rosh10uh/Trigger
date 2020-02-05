using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.Assessment
{
    public class AssessmentRepositoryTest : BaseRepositoryTest
    {
        public AssessmentRepositoryTest() : base("AssessmentRepository")
        {

        }

        [Fact]
        public void To_Verify_InsertAssessment_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerContext(GetServiceCollection<EmpAssessmentModel, EmpAssessmentModel, TriggerContext>(TestUtility.GetEmpAssessmentModel()).BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.AssessmentRepository.InsertAssessment(It.IsAny<EmpAssessmentModel>());

            //Assert      
            Assert.IsType<EmpAssessmentModel>(result);
            Assert.NotNull(result);
        }
    }
}
