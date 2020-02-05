using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Trigger.BLL.ContactUs;
using Trigger.BLL.Shared.Interfaces;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.Controller.V1
{
    public class ContactUsControllerTest
    {
        private readonly IOptions<AppSettings> _appSettings;

        public ContactUsControllerTest()
        {
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
        }
        [Fact]
        public void To_Verfiy_Invoke_Contact_UsSupport()
        {
            // Arrange
            var setupTest = new SetupTest();
            var serviceProvider = setupTest.Setup();
            var claims = new Mock<IClaims>();
            var logger = new Mock<ILogger<ContactUs>>();
            var catalogDbContext = new Mock<TriggerCatalogContext>(serviceProvider);
            var contactUs = new Mock<ContactUs>(catalogDbContext.Object,claims.Object,logger.Object,_appSettings);
            contactUs.Setup(x=>x.InvokeContactUsSupport(It.IsAny<ContactDetails>()));
            var contactUsController = new ContactUsController(contactUs.Object);

            // Act
            var result = contactUsController.post(new DTO.ContactDetails());

            // Assert
            Assert.NotNull(result);
        }
    }
}
