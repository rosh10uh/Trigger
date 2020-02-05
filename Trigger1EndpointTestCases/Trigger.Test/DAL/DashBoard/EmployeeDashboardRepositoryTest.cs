using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
namespace Trigger.Test.DAL.DashBoard
{
    public class EmployeeDashboardRepositoryTest : BaseRepositoryTest
    {

        public EmployeeDashboardRepositoryTest() : base("EmployeeDashboardRepository")
        {

        }

        [Fact]
        public void To_Verify_GetTenantNameByCompanyId_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerCatalogContext(GetServiceCollectionString<EmployeeDashboardModel, TriggerCatalogContext>("ABC").BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>());

            //Assert      
            Assert.IsType<string>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeDashboard_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerContext(GetServiceCollection<EmployeeDashboardModel, List<EmployeeDashboardModel>, TriggerContext>(TestUtility.GetEmployeeDashboardModels()).BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.EmployeeDashboardRepository.GetEmployeeDashboard(It.IsAny<EmployeeDashboardModel>());

            //Assert      
            Assert.IsType<List<EmployeeDashboardModel>>(result);
            Assert.NotNull(result);
        }
    }
}
