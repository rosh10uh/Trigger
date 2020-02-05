using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.EmailTemplate;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using Bll_ContactUs = Trigger.BLL.ContactUs.ContactUs;

namespace Trigger.Test.BLL.ContactUs
{
    public class ContactUsTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<Bll_ContactUs>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<IActionPermission> _iActionPermission;

        public ContactUsTest()
        {
            _logger = new Mock<ILogger<Bll_ContactUs>>();
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iActionPermission = new Mock<IActionPermission>();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
        }

        [Fact]
        public void To_Verify_Invoke_Contact_Us_Support()
        {
            //Arrange
            var claims = new Mock<IClaims>();
            var contactUs = new Mock<Bll_ContactUs>(_triggerCatalogContext.Object, claims.Object, _logger.Object, _appSettings);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _triggerCatalogContext.Setup(x=>x.EmailTemplateRepository.GetEmailTemplateByName(It.IsAny<EmailTemplateDetails>())).Returns(It.IsAny<string>());

            //Act
            var result = contactUs.Setup(x=>x.InvokeContactUsSupport(It.IsAny<ContactDetails>()));

            //Assert
            Assert.NotNull(result);
        }
    }
}
