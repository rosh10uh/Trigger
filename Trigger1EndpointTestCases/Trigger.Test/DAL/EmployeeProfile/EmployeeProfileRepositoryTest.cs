using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.DAL.EmployeeProfile
{
    public class EmployeeProfileRepositoryTest : BaseRepositoryTest
    {
        public EmployeeProfileRepositoryTest() : base("EmployeeProfileRepository")
        {

        }

        [Fact]
        public void To_Verify_UpdateAllowSMS()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeProfileModel, EmployeeProfileModel, TriggerContext>(EmployeeUtility.GetEmployeeProfile()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeProfileRepository.UpdateAllowSMS(It.IsAny<EmployeeProfileModel>());

            //Assert      
            Assert.IsType<EmployeeProfileModel>(result);
            Assert.NotNull(result);
        }
    }
}
