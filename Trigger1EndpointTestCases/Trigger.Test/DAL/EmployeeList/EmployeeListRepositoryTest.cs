using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.DAL.EmployeeList
{
    public class EmployeeListRepositoryTest : BaseRepositoryTest
    {
        public EmployeeListRepositoryTest() : base("EmployeeListRepository")
        {

        }

        [Fact]
        public void To_Verify_GetAllEmployee()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeListModel, List<EmployeeListModel>, TriggerContext>(EmployeeUtility.GetEmployeeListModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>());

            //Assert      
            Assert.IsType<List<EmployeeListModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeWithHierachy()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeListModel, List<EmployeeListModel>, TriggerContext>(EmployeeUtility.GetEmployeeListModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeListRepository.GetAllEmployeeWithHierachy(It.IsAny<EmployeeListModel>());

            //Assert      
            Assert.IsType<List<EmployeeListModel>>(result);
            Assert.NotNull(result);
        }
    }
}
