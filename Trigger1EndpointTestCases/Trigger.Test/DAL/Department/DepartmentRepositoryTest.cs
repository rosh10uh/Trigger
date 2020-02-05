using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.Department
{
    public class DepartmentRepositoryTest : BaseRepositoryTest
    {
        public DepartmentRepositoryTest() : base("DepartmentRepository")
        {

        }

        [Fact]
        public void To_Verify_GetCompanyAndYearwiseDepartment_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<DepartmentModel, List<CompanyWiseDepartmentModel>, TriggerContext>(new List<CompanyWiseDepartmentModel>()).BuildServiceProvider());

            //Act
            var result = triggerContext.DepartmentRepository.GetCompanyAndYearwiseDepartment(It.IsAny<DepartmentModel>());

            //Assert      
            Assert.IsType<List<CompanyWiseDepartmentModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetCompanywiseDepartment_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<DepartmentModel, List<CompanyWiseDepartmentModel>, TriggerContext>(new List<CompanyWiseDepartmentModel>()).BuildServiceProvider());

            //Act
            var result = triggerContext.DepartmentRepository.GetCompanywiseDepartment(It.IsAny<DepartmentModel>());

            //Assert            
            Assert.IsType<List<CompanyWiseDepartmentModel>>(result);
            Assert.NotNull(result);
        }
    }
}
