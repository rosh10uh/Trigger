using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OneRPP.Restful.Contracts.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Company;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.InActivityManager;
using Trigger.Test.Shared;
using Trigger.Utility;
using Xunit;
using BLL_Company = Trigger.BLL.Company.Company;
using TestUtility = Trigger.Test.Shared.Utility;
namespace Trigger.Test.DAL.Company
{
    public class CompanyContextTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<IEnvironmentVariables> _iEnvironmentVariables;
        private readonly Mock<TriggerCatalogContext> _catalogContext;
        private readonly Mock<ILogger<CompanyContext>> _logger;
        private readonly Mock<TriggerResourceReader> _triggerResourceReader;
        private readonly Mock<EmployeeContext> _employeeContext;
        private readonly Mock<OneRPP.Restful.Contracts.Utility.IResourceReader> _iResourceReader;

        //EmployeeContext constructor parametere
        private readonly Mock<ILogger<EmployeeContext>> _loggerEmployeeContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerContext> _triggerContext;

        public CompanyContextTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup(true);


            //EmployeeContext
            _catalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _loggerEmployeeContext = new Mock<ILogger<EmployeeContext>>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _employeeContext = new Mock<EmployeeContext>(_loggerEmployeeContext.Object, _iConnectionContext.Object, _catalogContext.Object);

            _logger = new Mock<ILogger<CompanyContext>>();
            _iEnvironmentVariables = _setupTest.SetUpEnvirementVariable();
            
            _iResourceReader = new Mock<OneRPP.Restful.Contracts.Utility.IResourceReader>();
            _triggerResourceReader = new Mock<TriggerResourceReader>(_iResourceReader.Object);
            _companyContext = new Mock<CompanyContext>(_catalogContext.Object, _iEnvironmentVariables.Object, _logger.Object, _triggerResourceReader.Object, _employeeContext.Object);

        }
    }
}
