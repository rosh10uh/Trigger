using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Trigger.BLL.OrganizationType;
using Trigger.DAL;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.BLL.OrganizationTypes
{
    public class OrganizationTypeBllTest
    {
        private readonly Mock<ILogger<OrganizationType>> _iLogger;
        private readonly Mock<TriggerCatalogContext> _catalogContext;
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;

        public OrganizationTypeBllTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _catalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iLogger = new Mock<ILogger<OrganizationType>>();
            
        }

       [Fact]
       public void To_Verify_SelectAllAsync()
        {
            //Arrange
            _catalogContext.Setup(x=>x.OrganizationTypeRepository.SelectAll());
            var organizationType = new Mock<OrganizationType>(_catalogContext.Object, _iLogger.Object);

            //Act
            var result = organizationType.Setup(x=>x.SelectAllAsync());

            //Assert
            Assert.NotNull(result);
        }
    }
}
