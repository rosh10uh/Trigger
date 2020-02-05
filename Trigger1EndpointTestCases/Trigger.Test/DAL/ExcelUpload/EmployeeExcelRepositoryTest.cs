using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.DAL.ExcelUpload
{
    public class EmployeeExcelRepositoryTest: BaseRepositoryTest
    {
        public EmployeeExcelRepositoryTest():base("EmployeeExcelRepository")
        {

        }

        [Fact]
        public void Verify_To_Add_Auth_Claims()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeExcelModel, EmployeeExcelModel, TriggerContext>(new EmployeeExcelModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeExcelRepository.AddEmpolyee(It.IsAny<EmployeeExcelModel>());

            //Assert      
            Assert.NotNull(result);
        }

        [Fact]
        public void Verify_To_Add_Company_Admin_Details()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeExcelModel, EmployeeExcelModel, TriggerContext>(new EmployeeExcelModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeExcelRepository.AddCompanyAdminDetails(It.IsAny<EmployeeExcelModel>());

            //Assert      
            Assert.NotNull(result);
        }
    }
}
