using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using Trigger.BLL.Employee;
using Trigger.DAL;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using SharedClaims = Trigger.DAL.Shared.Claims;

namespace Trigger.Test.BLL.Employee
{
    public class EmployeeCommonTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<EmployeeCommon>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private readonly Mock<EmployeeContext> _employeeContext;
        private readonly Mock<IActionPermission> _iActionPermission;
        private EmployeeCommon _employeeCommon;

        public EmployeeCommonTest()
        {
            _logger = new Mock<ILogger<EmployeeCommon>>();
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _iActionPermission = new Mock<IActionPermission>();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _employeeContext = new Mock<EmployeeContext>(_iConnectionContext.Object, _triggerCatalogContext.Object);
        }

        private void Init_EmployeeCommon()
        {
            _employeeCommon = new EmployeeCommon(_triggerCatalogContext.Object, _iConnectionContext.Object, _employeeContext.Object, _logger.Object, _appSettings, _iClaims.Object, _iActionPermission.Object);
        }

        [Theory]
        [InlineData(true)] // AddMode
        [InlineData(false)] //Edit mode
        public void To_Verify_IsPhoneNumberExistAspNetUser_WithAddMode(bool isAddMode)
        {
            //Arrange
            _triggerCatalogContext.Setup(x => x.EmployeeRepository.GetAspNetUserCountByPhone(It.IsAny<EmployeeModel>())).Returns(1);
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.IsPhoneNumberExistAspNetUser(EmployeeUtility.GetEmployeeModel(), isAddMode);

            //Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void To_Verify_IsPhoneNumberExistAspNetUser_WithEmptyPhoneNumber()
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.PhoneNumber = "";

            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.IsPhoneNumberExistAspNetUser(employeeModel, true);

            //Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void To_Verify_GetExistingEmployeeDetails()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.GetExistingEmployeeDetails(1);

            //Assert
            Assert.IsType<EmployeeModel>(result);
        }

        [Theory]
        [InlineData(0)] //EmployeeNotExist
        [InlineData(1)] //EmployeeUpdate
        [InlineData(2)] //EmailIdExist
        [InlineData(3)] //EmployeeIdExist
        [InlineData(4)] //PhoneNumberExist
        [InlineData(-1)] //Default
        public void To_Verify_GetResponseMessageForUpdate(int resultId)
        {
            //Arrange
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.GetResponseMessageForUpdate(resultId);

            //Assert
            Assert.IsType<CustomJsonData>(result);
        }

        [Fact]
        public void To_Verify_UpdateAuthUser()
        {
            //Arrange
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Update(It.IsAny<AuthUserDetails>()));
            Init_EmployeeCommon();

            //Act
            _employeeCommon.UpdateAuthUser(EmployeeUtility.GetEmployeeModel(), "abc");

            //Assert
            Assert.IsType<bool>(true);
        }

        [Fact]
        public void To_Verify_UpdateAuthUser_WithException()
        {
            //Arrange
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Update(It.IsAny<AuthUserDetails>()));
            Init_EmployeeCommon();

            //Act
            _employeeCommon.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>());

            //Assert
            Assert.IsType<bool>(true);
        }

        [Fact]
        public void To_Verify_InsertCompanyAdmin()
        {
            //Arrange
            _triggerCatalogContext.Setup(x => x.CompanyAdminRepository.AddCompanyAdminDetails(It.IsAny<EmployeeModel>()));
            Init_EmployeeCommon();

            //Act
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.Update(It.IsAny<AuthUserDetails>()));
            _employeeCommon.InsertCompanyAdmin(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<bool>(true);
        }

        [Fact]
        public void To_Verify_GenerateToken()
        {
            //Arrange
            Init_EmployeeCommon();

            //Act
            _employeeCommon.GenerateToken("07d7dbef-8a2b-4f9d-96ff-cac26a42322f");

            //Assert
            Assert.IsType<bool>(true);
        }

        [Fact]
        public void To_Verify_GetRedirectEmpListWithHierachyForActions()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetYearwiseEmployeeWithHierachy(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            Init_EmployeeCommon();

            //Act
            _employeeCommon.GetRedirectEmpListWithHierachyForActions(EmployeeUtility.GetEmployeeModel(), EmployeeUtility.GetEmployeeModels());

            //Assert
            Assert.IsType<bool>(true);
        }

        [Fact]
        public void To_Verify_SetEmployeeProfilePic_WithEmployeeList()
        {
            //Arrange
            List<EmployeeModel> employeeModels = EmployeeUtility.GetEmployeeModels();
            employeeModels.Add(EmployeeUtility.GetEmployeeModel());
            employeeModels[0].EmpImgPath = string.Empty;

            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.SetEmployeeProfilePic(EmployeeUtility.GetEmployeeModels());

            //Assert
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("Pic1")]
        [InlineData(null)]
        public void To_Verify_SetEmployeeProfilePic_WithEmployee(string imgPath)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.EmpImgPath = imgPath;

            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.SetEmployeeProfilePic(employeeModel);

            //Assert
            Assert.IsType<EmployeeModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeesDetail()
        {
            //Arrange
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeByEmpIdsForMails(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.GetEmployeesDetail(1, "1,2,3");

            //Assert
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(1)] //Role
        [InlineData(3)] //Relation
        public void To_Verify_GetEmployeeListForActions(int dimensionId)
        {
            //Arrange
            List<ActionwisePermissionModel> actionwisePermissionModels = CommonUtility.GetActionwisePermissionModels();
            actionwisePermissionModels.ForEach(x => x.DimensionId = dimensionId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployeeWithHierachy(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());

            Init_EmployeeCommon();

            //Act

            var result = _employeeCommon.GetEmployeeListForActions(actionwisePermissionModels, 1, 1);

            //Assert
            Assert.IsType<List<EmployeeListModel>>(result);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetResponseWithCheckNoRecord(bool isWithData)
        {
            //Arrange
            List<EmployeeModel> employeeModels = isWithData ? EmployeeUtility.GetEmployeeModels() : null;
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.GetResponseWithCheckNoRecord(employeeModels);

            //Assert
            Assert.IsType<CustomJsonData>(result);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetResponseWitCheckAccessDenied(bool isWithData)
        {
            //Arrange
            List<EmployeeModel> employeeModels = isWithData ? EmployeeUtility.GetEmployeeModels() : null;
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.GetResponseWitCheckAccessDenied(employeeModels);

            //Assert
            Assert.IsType<CustomJsonData>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetRoleIdFromClaims()
        {
            //Arrange
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.GetRoleIdFromClaims();

            //Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeIdFromClaims()
        {
            //Arrange
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.GetEmployeeIdFromClaims();

            //Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public void To_Verify_GetAllPermission()
        {
            //Arranger
            _iActionPermission.Setup(x => x.GetPermissions(It.IsAny<int>())).Returns(CommonUtility.GetActionLists());
            Init_EmployeeCommon();

            //Act
            var result = _employeeCommon.GetAllPermission(1, 1);

            //Assert
            Assert.IsType<ActionList>(result);
            Assert.NotNull(result);
        }
    }
}
