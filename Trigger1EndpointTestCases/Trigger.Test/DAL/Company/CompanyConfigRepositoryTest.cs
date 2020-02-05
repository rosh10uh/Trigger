using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
namespace Trigger.Test.DAL.Company
{
    public class CompanyConfigRepositoryTest :BaseRepositoryTest
    {
        public CompanyConfigRepositoryTest() :base("CompanyConfigRepository")
        {

        }

        [Fact]
        public void To_Verify_InsertCompanyConfig_Method()
        {
            // Arrange
            var triggerCatalogContext = new TriggerCatalogContext(GetServiceCollection<CompanyConfigModel, CompanyConfigModel, TriggerCatalogContext>(TestUtility.GetCompanyConfigModel()).BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.CompanyConfigRepository.InsertCompanyConfig(It.IsAny<CompanyConfigModel>());

            //Assert      
            Assert.IsType<CompanyConfigModel>(result);
            Assert.NotNull(result);
        }
    }
}
