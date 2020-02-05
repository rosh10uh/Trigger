using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.DAL.Employee
{
    public class EmployeeContextTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private EmployeeContext _employeeContext;

        public EmployeeContextTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
        }

        private void Init_EmployeeContext()
        {
            _employeeContext = new EmployeeContext(_iConnectionContext.Object, _triggerCatalogContext.Object);
        }

        [Fact]
        public void To_Verify_UpdateUser()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.UpdateUser(It.IsAny<UserDetails>())).Returns(CommonUtility.GetUserLogin());
            Init_EmployeeContext();

            //Act
            var result = _employeeContext.UpdateUser(CommonUtility.GetUserDetails());

            //Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public void To_Verify_Update1AuthUser()
        {
            //Arrange
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Update(It.IsAny<AuthUserDetails>()));
            Init_EmployeeContext();

            //Act
            _employeeContext.Update1AuthUser(CommonUtility.GetAuthUserDetails());

            //Assert
            Assert.IsType<bool>(true);
        }

        [Fact]
        public void To_Verify_UpdateAuthUserClaims()
        {
            //Arrange
            _triggerCatalogContext.Setup(x => x.ClaimsCommonRepository.Update(It.IsAny<Claims>()));
            Init_EmployeeContext();

            //Act
            _employeeContext.UpdateAuthUserClaims(CommonUtility.GetClaims());

            //Assert
            Assert.IsType<bool>(true);
        }

        [Fact]
        public void To_Verify_UpdateEmpIsMailSent()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.UpdateEmpForIsMailSent(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            Init_EmployeeContext();

            //Act
            var result = _employeeContext.UpdateEmpIsMailSent(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public void To_Verify_DeleteAuthUser()
        {
            //Arrange
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Delete(It.IsAny<AuthUserDetails>()));
            Init_EmployeeContext();

            //Act
            _employeeContext.DeleteAuthUser(CommonUtility.GetAuthUserDetails());

            //Assert
            Assert.IsType<bool>(true);
        }
    }
}
