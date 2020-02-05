using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO.EmailTemplate;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.DAL.EmailTemplate
{
    public class EmailTemplateRepositoryTest : BaseRepositoryTest
    {
        public EmailTemplateRepositoryTest() : base("EmailTemplateRepository")
        {

        }

        [Fact]
        public void To_Verify_GetEmailTemplateByName()
        {
            // Arrange
            var triggerCatalogContext = new TriggerCatalogContext(GetServiceCollectionString<EmailTemplateDetails, TriggerCatalogContext>("ABC").BuildServiceProvider());

            //Act
            var result = triggerCatalogContext.EmailTemplateRepository.GetEmailTemplateByName(It.IsAny<EmailTemplateDetails>());

            //Assert      
            Assert.IsType<string>(result);
        }
    }
}
