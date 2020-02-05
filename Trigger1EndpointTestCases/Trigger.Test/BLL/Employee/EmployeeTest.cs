using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.Employee;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using BLL_Employee = Trigger.BLL.Employee.Employee;
using BLL_Notification = Trigger.BLL.Notification.Notification;
using SharedClaims = Trigger.DAL.Shared.Claims;

namespace Trigger.Test.BLL.Employee
{
    public class EmployeeTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_Employee>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private readonly Mock<EmployeeCommon> _employeeCommon;
        private readonly Mock<EmployeeSendEmail> _employeeSendEmail;
        private readonly Mock<BLL_Notification> _notification;
        private readonly Mock<IActionPermission> _iActionPermission;
        private BLL_Employee _employee;


        public EmployeeTest()
        {
            _logger = new Mock<ILogger<BLL_Employee>>();
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iActionPermission = new Mock<IActionPermission>();

            _employeeCommon = new Mock<EmployeeCommon>(_triggerCatalogContext.Object, _iConnectionContext.Object, null, null, _appSettings, _iClaims.Object, null);
            _employeeSendEmail = new Mock<EmployeeSendEmail>(null, null, _appSettings, _triggerCatalogContext.Object, null, _employeeCommon.Object);
            _notification = new Mock<BLL_Notification>(_iConnectionContext.Object, null, _appSettings);

        }

        private void Init_Employee()
        {
            _employee = new BLL_Employee(_iConnectionContext.Object, _logger.Object, _triggerCatalogContext.Object,
                _employeeSendEmail.Object, _employeeCommon.Object, _notification.Object,_iActionPermission.Object);
        }

        [Theory]
        [InlineData(2)]// Admin
        [InlineData(3)]//manager
        [InlineData(4)]//Executive
        [InlineData(5)]//Nonmanager
        public void To_Verify_InsertAsync_WithRoles(int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.RoleId = roleId;

            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(false);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Insert(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Insert(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns("Dev");
            _triggerCatalogContext.Setup(x => x.ClaimsRepository.Insert(It.IsAny<List<Claims>>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.AddCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _employeeSendEmail.Setup(x => x.SendEmployeeRegistrationMail(It.IsAny<EmployeeModel>(), It.IsAny<string>(), It.IsAny<string>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));

            Init_Employee();

            //Act

            var result = _employee.InsertAsync(1, employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_InsertAsync_WithNewAspnetusers()
        {
            //Arrange
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(false);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Insert(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns((AuthUserDetails)null);
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Insert(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns("Dev");
            _triggerCatalogContext.Setup(x => x.ClaimsRepository.Insert(It.IsAny<List<Claims>>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.AddCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _employeeSendEmail.Setup(x => x.SendEmployeeRegistrationMail(It.IsAny<EmployeeModel>(), It.IsAny<string>(), It.IsAny<string>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));

            Init_Employee();

            //Act
            var result = _employee.InsertAsync(1, EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)] //Employee already exist
        [InlineData(1)] //Employee submited
        [InlineData(-2)] //Email Id already exist
        [InlineData(-3)] //Phone Number already exist
        [InlineData(-4)] // Unauthorized
        public void To_Verify_InsertAsync_WithResultType(int resultType)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.ResultId = resultType;

            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(false);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Insert(It.IsAny<EmployeeModel>())).Returns(employeeModel);
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns((AuthUserDetails)null);
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Insert(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns("Dev");
            _triggerCatalogContext.Setup(x => x.ClaimsRepository.Insert(It.IsAny<List<Claims>>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.AddCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _employeeSendEmail.Setup(x => x.SendEmployeeRegistrationMail(It.IsAny<EmployeeModel>(), It.IsAny<string>(), It.IsAny<string>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));

            Init_Employee();

            //Act
            var result = _employee.InsertAsync(1, employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)] //InsertAsync method exception
        [InlineData(1)] //AddUserDetailsInCatalog/AddAuthClaims method exception
        public void To_Verify_InsertAsync_WithException(int exceptionType)
        {
            //Arrange
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(false);
            if (exceptionType != 0)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Insert(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            }
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns((AuthUserDetails)null);
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Insert(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());

            if (exceptionType != 1)
            {
                _triggerCatalogContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns("Dev");
                _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.AddCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            }
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));
            _employeeSendEmail.Setup(x => x.SendEmployeeRegistrationMail(It.IsAny<EmployeeModel>(), It.IsAny<string>(), It.IsAny<string>()));

            Init_Employee();

            //Act
            var result = _employee.InsertAsync(1, EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_InsertAsync_WithPhoneNumberExist()
        {
            //Arrange
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(true);
            Init_Employee();

            //Act
            var result = _employee.InsertAsync(1, EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(2)]// Admin
        [InlineData(3)]//manager
        [InlineData(4)]//Executive
        [InlineData(5)]//Nonmanager
        public void To_Verify_UpdateAsync_WithRoleId(int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.RoleId = roleId;

            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(false);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Update(It.IsAny<EmployeeModel>())).Returns(employeeModel);
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.GetUserDetails(It.IsAny<UserDetails>())).Returns(EmployeeUtility.GetUserDetails());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteUser(It.IsAny<EmployeeModel>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Delete(It.IsAny<AuthUserDetails>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.DeleteCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>()));
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Insert(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns("Dev");
            _triggerCatalogContext.Setup(x => x.ClaimsRepository.Insert(It.IsAny<List<Claims>>()));
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.UpdateUser(It.IsAny<UserDetails>()));
            _triggerCatalogContext.Setup(x => x.ClaimsCommonRepository.Update(It.IsAny<Claims>()));
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _employeeSendEmail.Setup(x => x.SendEmailOnUpdateEmployeeDetails(It.IsAny<EmployeeModel>(), It.IsAny<string>(), It.IsAny<string>()));
            _employeeCommon.Setup(x => x.InsertCompanyAdmin(It.IsAny<EmployeeModel>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));
            _employeeCommon.Setup(x => x.GetResponseMessageForUpdate(It.IsAny<int>())).Returns(CommonUtility.GetCustomJsonData());

            Init_Employee();

            //Act
            var result = _employee.UpdateAsync(1, EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(2, 5)]// Admin to non manager
        [InlineData(5, 2)]//non manager to admin
        [InlineData(4, 5)]//Executive to non manager
        [InlineData(2, 2)]//Admin to Admin
        [InlineData(5, 5)]//Non manager to non manager
        [InlineData(5, 4)]//Nonmanager to excecutive
        public void To_Verify_UpdateAsync_WithChangeRole(int oldRoleId, int newRoleID)
        {
            //Arrange
            EmployeeModel olEmployeeModel = EmployeeUtility.GetEmployeeModel();
            olEmployeeModel.RoleId = oldRoleId;
            EmployeeModel newEmployeeModel = EmployeeUtility.GetEmployeeModel();
            newEmployeeModel.RoleId = newRoleID;

            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(olEmployeeModel);
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(false);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Update(It.IsAny<EmployeeModel>())).Returns(newEmployeeModel);
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.GetUserDetails(It.IsAny<UserDetails>())).Returns(EmployeeUtility.GetUserDetails());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteUser(It.IsAny<EmployeeModel>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Delete(It.IsAny<AuthUserDetails>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.DeleteCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>()));
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Insert(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns("Dev");
            _triggerCatalogContext.Setup(x => x.ClaimsRepository.Insert(It.IsAny<List<Claims>>()));
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.UpdateUser(It.IsAny<UserDetails>()));
            _triggerCatalogContext.Setup(x => x.ClaimsCommonRepository.Update(It.IsAny<Claims>()));
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _employeeSendEmail.Setup(x => x.SendEmailOnUpdateEmployeeDetails(It.IsAny<EmployeeModel>(), It.IsAny<string>(), It.IsAny<string>()));
            _employeeCommon.Setup(x => x.InsertCompanyAdmin(It.IsAny<EmployeeModel>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));
            _employeeCommon.Setup(x => x.GetResponseMessageForUpdate(It.IsAny<int>())).Returns(CommonUtility.GetCustomJsonData());

            Init_Employee();

            //Act
            var result = _employee.UpdateAsync(1, newEmployeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData("abc@yopmail", "xyz@yopmail.com")]
        public void To_Verify_UpdateAsync_WithChangeEmail(string oldEmail, string newEmail)
        {
            //Arrange
            EmployeeModel olEmployeeModel = EmployeeUtility.GetEmployeeModel();
            olEmployeeModel.Email = oldEmail;
            EmployeeModel newEmployeeModel = EmployeeUtility.GetEmployeeModel();
            newEmployeeModel.Email = newEmail;

            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(olEmployeeModel);
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(false);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Update(It.IsAny<EmployeeModel>())).Returns(newEmployeeModel);
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.GetUserDetails(It.IsAny<UserDetails>())).Returns(EmployeeUtility.GetUserDetails());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteUser(It.IsAny<EmployeeModel>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Delete(It.IsAny<AuthUserDetails>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.DeleteCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>()));
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Insert(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns("Dev");
            _triggerCatalogContext.Setup(x => x.ClaimsRepository.Insert(It.IsAny<List<Claims>>()));
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.UpdateUser(It.IsAny<UserDetails>()));
            _triggerCatalogContext.Setup(x => x.ClaimsCommonRepository.Update(It.IsAny<Claims>()));
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _employeeSendEmail.Setup(x => x.SendEmailOnUpdateEmployeeDetails(It.IsAny<EmployeeModel>(), It.IsAny<string>(), It.IsAny<string>()));
            _employeeCommon.Setup(x => x.InsertCompanyAdmin(It.IsAny<EmployeeModel>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));
            _employeeCommon.Setup(x => x.GetResponseMessageForUpdate(It.IsAny<int>())).Returns(CommonUtility.GetCustomJsonData());

            Init_Employee();

            //Act
            var result = _employee.UpdateAsync(1, newEmployeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UpdateAsync_WithPhoneNumberExist()
        {
            //Arrange
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), false)).Returns(true);
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.GetResponseMessageForUpdate(It.IsAny<int>())).Returns(CommonUtility.GetCustomJsonData());

            Init_Employee();

            //Act
            var result = _employee.UpdateAsync(1, EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)] // Exception in UpdateAsync method 
        [InlineData(1)] // Exception in UpdateUserLoginDetails method  (not working)...........................................................................................................................
        public void To_Verify_UpdateAsync_WithException(int exceptionType)
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), true)).Returns(false);

            if (exceptionType != 0)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Update(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            }
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteUser(It.IsAny<EmployeeModel>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Delete(It.IsAny<AuthUserDetails>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.DeleteCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.AddUser(It.IsAny<UserDetails>()));
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Insert(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _triggerCatalogContext.Setup(x => x.ClaimsRepository.Insert(It.IsAny<List<Claims>>()));
            if (exceptionType != 1)
            {
                _triggerCatalogContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns("Dev");
                _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.UpdateUser(It.IsAny<UserDetails>()));
            }
            _triggerCatalogContext.Setup(x => x.ClaimsCommonRepository.Update(It.IsAny<Claims>()));
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _employeeSendEmail.Setup(x => x.SendEmailOnUpdateEmployeeDetails(It.IsAny<EmployeeModel>(), It.IsAny<string>(), It.IsAny<string>()));
            _employeeCommon.Setup(x => x.InsertCompanyAdmin(It.IsAny<EmployeeModel>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));
            _employeeCommon.Setup(x => x.GetResponseMessageForUpdate(It.IsAny<int>())).Returns(CommonUtility.GetCustomJsonData());

            Init_Employee();

            //Act
            var result = _employee.UpdateAsync(1, EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(2)] // Admin
        [InlineData(3)] //manager
        [InlineData(4)] //Executive
        [InlineData(5)] //Nonmanager
        public void To_Verify_DeleteAsync_WithRoleId(int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.RoleId = roleId;

            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(employeeModel);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteEmployee(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetUserDetails());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteUser(It.IsAny<EmployeeModel>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.DeleteCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));

            Init_Employee();

            //Act
            var result = _employee.DeleteAsync(1, 1, 2);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
        [Theory]
        [InlineData(-1)] //Default case
        [InlineData(1)] // Delete successfull
        [InlineData(0)] // employee not exist
        public void To_Verify_DeleteAsync_WithResultType(int resultType)
        {
            //Arrange
            UserDetails userDetails = EmployeeUtility.GetUserDetails();
            userDetails.Result = resultType;

            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteEmployee(It.IsAny<EmployeeModel>())).Returns(userDetails);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteUser(It.IsAny<EmployeeModel>()));
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.DeleteCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _notification.Setup(x => x.SendNotifications(It.IsAny<int>(), It.IsAny<EmployeeModel>()));

            Init_Employee();

            //Act
            var result = _employee.DeleteAsync(1, 1, 2);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_DeleteAsync_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.DeleteAsync(1, 1, 2);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(0)] //Employee not exist
        [InlineData(1)] //Salary Updated
        [InlineData(2)] //Employee inactive
        [InlineData(-1)] //Default
        public void To_Verify_UpdateEmpSalaryAsync_WithResultType(int resultType)
        {
            //Arrange
            EmployeeSalary employeeSalary = EmployeeUtility.GetEmployeeSalary();
            employeeSalary.Result = resultType;
            _iConnectionContext.Setup(x => x.TriggerContext.EmpSalaryRepository.Update(It.IsAny<EmployeeSalary>())).Returns(employeeSalary);

            Init_Employee();

            //Act
            var result = _employee.UpdateEmpSalaryAsync(1, EmployeeUtility.GetEmployeeSalary());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UpdateEmpSalaryAsync_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.UpdateEmpSalaryAsync(1, EmployeeUtility.GetEmployeeSalary());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Fact]
        public void To_Verify_GetEmployeeByIdAsync()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.Select(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.SetEmployeeProfilePic(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());

            Init_Employee();

            //Act
            var result = _employee.GetEmployeeByIdAsync(1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByIdAsync_WithExcepion()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetEmployeeByIdAsync(1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Fact]
        public void To_Verify_GetAllEmployeesAsync()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployee(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.SetEmployeeProfilePic(It.IsAny<List<EmployeeModel>>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.GetResponseWitCheckAccessDenied(It.IsAny<List<EmployeeModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));

            Init_Employee();

            //Act
            var result = _employee.GetAllEmployeesAsync(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeesAsync_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetAllEmployeesAsync(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Fact]
        public void To_Verify_GetCompanyWiseEmployeeAsync()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetCompanyWiseEmployee(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.SetEmployeeProfilePic(It.IsAny<List<EmployeeModel>>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.GetResponseWitCheckAccessDenied(It.IsAny<List<EmployeeModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));

            Init_Employee();

            //Act
            var result = _employee.GetCompanyWiseEmployeeAsync(1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetCompanyWiseEmployeeAsync_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetCompanyWiseEmployeeAsync(1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Fact]
        public void To_Verify_GetCompanyWiseEmployeeWithPaginationAsync()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetCompanyWiseEmployeeWithPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.SetEmployeeProfilePic(It.IsAny<List<EmployeeModel>>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.GetResponseWithCheckNoRecord(It.IsAny<List<EmployeeModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));

            Init_Employee();

            //Act
            var result = _employee.GetCompanyWiseEmployeeWithPaginationAsync(It.IsAny<EmployeeModel>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetCompanyWiseEmployeeWithPaginationAsync_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetCompanyWiseEmployeeWithPaginationAsync(It.IsAny<EmployeeModel>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(1, 0, 0)] //Trigger Admin
        [InlineData(2, 1, 1)] //Company Admin 
        [InlineData(3, 0, 1)] //Manager
        public void To_Verify_GetAllEmployeesWithPaginationAsync_WithRoleId(int roleId, int managerId, int companyId)
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _triggerCatalogContext.Setup(x => x.EmployeeRepository.GetAllEmployeeForTriggerAdminWithPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.GetEmployeeIdFromClaims()).Returns(managerId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerWithPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeWithPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeWithHierachy(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.SetEmployeeProfilePic(It.IsAny<List<EmployeeModel>>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.GetResponseWithCheckNoRecord(It.IsAny<List<EmployeeModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));

            Init_Employee();

            //Act
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.ManagerId = managerId;
            employeeModel.CompanyId = companyId;

            var result = _employee.GetAllEmployeesWithPaginationAsync(employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeesWithPaginationAsync_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetAllEmployeesWithPaginationAsync(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(1, 0, 0)] //Trigger Admin
        [InlineData(2, 1, 1)] //Company Admin 
        public void To_Verify_GetAllEmployeesWithoutPaginationAsync_WithRoleId(int roleId, int managerId, int companyId)
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _triggerCatalogContext.Setup(x => x.EmployeeRepository.GetAllEmployeeForTriggerAdminWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeWithHierachy(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.SetEmployeeProfilePic(It.IsAny<List<EmployeeModel>>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.GetResponseWithCheckNoRecord(It.IsAny<List<EmployeeModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));

            Init_Employee();

            //Act
            var result = _employee.GetAllEmployeesWithoutPaginationAsync(companyId, managerId, "1,2,3");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeesWithoutPaginationAsync_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetAllEmployeesWithoutPaginationAsync(1, 1, "1,2,3");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Fact]
        public void To_Verify_GetCompanyWiseEmployeeWithoutPaginationAsync()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetCompanyWiseEmployeeWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.SetEmployeeProfilePic(It.IsAny<List<EmployeeModel>>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.GetResponseWithCheckNoRecord(It.IsAny<List<EmployeeModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));

            Init_Employee();

            //Act
            var result = _employee.GetCompanyWiseEmployeeWithoutPaginationAsync(1, "1,2,3");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetCompanyWiseEmployeeWithoutPaginationAsync_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetCompanyWiseEmployeeWithoutPaginationAsync(1, "1,2,3");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void To_Verify_GetTriggerEmpList(int roleId)
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetAllPermission(It.IsAny<int>(), It.IsAny<int>())).Returns(CommonUtility.GetActionList());
            _employeeCommon.Setup(x => x.GetEmployeeListForActions(It.IsAny<List<ActionwisePermissionModel>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetResponseWitCheckAccessDenied(It.IsAny<List<EmployeeListModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeListModels()));

            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpList(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetTriggerEmpList_WithNoPermission()
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(3);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            ActionList actionList = CommonUtility.GetActionList();
            actionList.ActionPermissions.ForEach(x => x.CanAdd = false); //set canAdd permission as false
            _employeeCommon.Setup(x => x.GetAllPermission(It.IsAny<int>(), It.IsAny<int>())).Returns(actionList);
            _employeeCommon.Setup(x => x.GetEmployeeListForActions(It.IsAny<List<ActionwisePermissionModel>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetResponseWitCheckAccessDenied(It.IsAny<List<EmployeeListModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeListModels()));

            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpList(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetTriggerEmpList_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpList(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(2)] //Company Admin
        [InlineData(3)] //Manager
        public void To_Verify_GetTriggerEmpListV2_WithRoleId(int roleId)
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _iActionPermission.Setup(x => x.GetPermissionsV2(It.IsAny<int>())).Returns(CommonUtility.GetActionLists());

            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.GetEmpListWithHierachyForActions(It.IsAny<EmployeeListModel>(), It.IsAny<List<EmployeeListModel>>()));

            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetTriggerEmpListV2_WithEmployeeAccessDenied()
        {
            //Arrange
            List<ActionList> actionLists = CommonUtility.GetActionLists();
            actionLists.ForEach(x => x.ActionPermissions.ForEach(y => y.CanAdd = false));

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(3);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _iActionPermission.Setup(x => x.GetPermissionsV2(It.IsAny<int>())).Returns(actionLists);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.GetEmpListWithHierachyForActions(It.IsAny<EmployeeListModel>(), It.IsAny<List<EmployeeListModel>>()));

            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(1)] //Role
        [InlineData(2)] //Relation
        [InlineData(3)] //Department
        [InlineData(-1)] //Default or nothing
        public void To_Verify_GetTriggerEmpListV2_WithDimentionType(int dimentionType)
        {
            //Arrange
            List<ActionList> actionLists = CommonUtility.GetActionLists();
            actionLists.ForEach(x => x.ActionPermissions.ForEach(y => y.DimensionId = dimentionType));

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(3);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _iActionPermission.Setup(x => x.GetPermissionsV2(It.IsAny<int>())).Returns(actionLists);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.GetEmpListWithHierachyForActions(It.IsAny<EmployeeListModel>(), It.IsAny<List<EmployeeListModel>>()));

            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpListV2(1, 0);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(1)] //Inside
        [InlineData(2)] //Outside
        [InlineData(-1)] //Default or All
        public void To_Verify_GetTriggerEmpListV2_WithDepatmentDimentnValue(int dimentionValueId)
        {
            //Arrange
            List<ActionList> actionLists = CommonUtility.GetActionLists();
            actionLists.ForEach(x => x.ActionPermissions.ForEach(y => y.DimensionId = 3));
            actionLists.ForEach(x => x.ActionPermissions.ForEach(y => y.DimensionValueid = dimentionValueId));

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(3);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _iActionPermission.Setup(x => x.GetPermissionsV2(It.IsAny<int>())).Returns(actionLists);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.GetEmpListWithHierachyForActions(It.IsAny<EmployeeListModel>(), It.IsAny<List<EmployeeListModel>>()));

            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetTriggerEmpListV2_WithAccessDenied()
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(2);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(new List<EmployeeListModel>());

            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetTriggerEmpListV2_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetTriggerEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(2)] //Company Admin
        [InlineData(3)] //Manager
        public void To_Verify_GetDashboardEmpListV2_WithRoleId(int roleId)
        {
            //Arrange
            List<ActionList> actionLists = CommonUtility.GetActionLists();
            actionLists.ForEach(x => x.ActionId = Enums.Actions.EmployeeDashboard.GetHashCode());

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _iActionPermission.Setup(x => x.GetPermissionsV2(It.IsAny<int>())).Returns(actionLists);

            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.GetEmpListWithHierachyForActions(It.IsAny<EmployeeListModel>(), It.IsAny<List<EmployeeListModel>>()));

            Init_Employee();

            //Act
            var result = _employee.GetDashboardEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetDashboardEmpListV2_WithEmployeeAccessDenied()
        {
            //Arrange
            List<ActionList> actionLists = CommonUtility.GetActionLists();
            actionLists.ForEach(x => x.ActionId = Enums.Actions.EmployeeDashboard.GetHashCode());
            actionLists.ForEach(x => x.ActionPermissions.ForEach(y => y.CanView = false));

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(3);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _iActionPermission.Setup(x => x.GetPermissionsV2(It.IsAny<int>())).Returns(actionLists);
           
            Init_Employee();

            //Act
            var result = _employee.GetDashboardEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetDashboardEmpListV2_WithAccessDenied()
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(2);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(new List<EmployeeListModel>());

            Init_Employee();

            //Act
            var result = _employee.GetDashboardEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetDashboardEmpListV2_WithException()
        {
            //Arrange
            Init_Employee();

            //Act
            var result = _employee.GetDashboardEmpListV2(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

    }
}
