using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Trigger.BLL.OrganizationType;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.Controller.V1
{
    public class OrganizationTypeControllerTest
    {
        private readonly Mock<OrganizationType> _organizationType;
        private readonly Mock<ILogger<OrganizationType>> _iLogger;
        private readonly Mock<TriggerCatalogContext> _catalogContext;
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;

        public OrganizationTypeControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _catalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iLogger = new Mock<ILogger<OrganizationType>>();
            _organizationType = new Mock<OrganizationType>(_catalogContext.Object,_iLogger.Object);
        }

        [Fact]
        public void To_Verify_Get_OrganizationType()
        {
            //Arrange
            _organizationType.Setup(x=>x.SelectAllAsync()).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetOrganizationType()));
            var organizationTypeController = new OrganizationTypeController(_organizationType.Object);

            //Act
            var result = organizationTypeController.Get();

            //Assert
            Assert.NotNull(result);
        }
    }
}
