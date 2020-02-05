using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.Employee;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using SharedClaims = Trigger.DAL.Shared.Claims;

namespace Trigger.Test.BLL.Employee
{
    public class EmployeeForDashboardTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<EmployeeForDashboard>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<EmployeeCommon> _employeeCommon;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private EmployeeForDashboard _employeeForDashboard;

        public EmployeeForDashboardTest()
        {
            _logger = new Mock<ILogger<EmployeeForDashboard>>();
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _employeeCommon = new Mock<EmployeeCommon>(_triggerCatalogContext.Object, _iConnectionContext.Object, null, null, _appSettings, _iClaims.Object, null);
        }

        private void Init_EmployeeForDashboard()
        {
            _employeeCommon.Setup(x => x.SetEmployeeProfilePic(It.IsAny<List<EmployeeModel>>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeCommon.Setup(x => x.GetRedirectEmpListWithHierachyForActions(It.IsAny<EmployeeModel>(), It.IsAny<List<EmployeeModel>>()));
            _employeeCommon.Setup(x => x.GetResponseWithCheckNoRecord(It.IsAny<List<EmployeeModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));

            _employeeForDashboard = new EmployeeForDashboard(_triggerCatalogContext.Object, _iConnectionContext.Object, _logger.Object, _employeeCommon.Object);
        }

        [Theory]
        [InlineData(0, 0, 1)] //Trigger Admin
        [InlineData(0, 1, 3)] //Manager
        [InlineData(1, 1, 2)] //Company Admin
        public void To_Verify_GetAllEmployeesWithPaginationYearWiseAsync_WithRole(int managerId, int companyId, int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.ManagerId = managerId;
            employeeModel.CompanyId = companyId;
            employeeModel.RoleId = roleId;

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _employeeCommon.Setup(x => x.GetEmployeeIdFromClaims()).Returns(managerId);
            _triggerCatalogContext.Setup(x => x.EmployeeRepository.GetAllEmployeeForTriggerAdminWithPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerYearwise(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeYearwise(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetAllEmployeesWithPaginationYearWiseAsync(employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeesWithPaginationYearWiseAsync_WithException()
        {
            //Arrange
            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetAllEmployeesWithPaginationYearWiseAsync(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0, 3)] //Manager
        [InlineData(1, 2)] //Company Admin
        public void To_Verify_GetEmployeeByGradeWithPaginationAsync_WithRole(int managerId, int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.ManagerId = managerId;
            employeeModel.RoleId = roleId;

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _employeeCommon.Setup(x => x.GetEmployeeIdFromClaims()).Returns(managerId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwise(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetEmployeeByGradeWithPaginationAsync(employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeWithPaginationAsync_WithException()
        {
            //Arrange
            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetEmployeeByGradeWithPaginationAsync(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0, 3)] //Manager
        [InlineData(1, 2)] //Company Admin
        public void To_Verify_GetEmployeeByGradeAndMonthWithPaginationAsync_WithRole(int managerId, int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.ManagerId = managerId;
            employeeModel.RoleId = roleId;

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _employeeCommon.Setup(x => x.GetEmployeeIdFromClaims()).Returns(managerId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthByManagerYearwise(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthYearwise(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetEmployeeByGradeAndMonthWithPaginationAsync(employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeAndMonthWithPaginationAsyncc_WithException()
        {
            //Arrange
            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetEmployeeByGradeAndMonthWithPaginationAsync(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0, 3)] //Manager
        [InlineData(1, 2)] //Company Admin
        public void To_Verify_GetEmployeeByGradeAndMonthWithoutPaginationAsync_WithRole(int managerId, int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.ManagerId = managerId;
            employeeModel.RoleId = roleId;

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _employeeCommon.Setup(x => x.GetEmployeeIdFromClaims()).Returns(managerId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmpByGradeAndMonthYearwiseWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetEmployeeByGradeAndMonthWithoutPaginationAsync(employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeAndMonthWithoutPaginationAsync_WithException()
        {
            //Arrange
            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetEmployeeByGradeAndMonthWithoutPaginationAsync(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0, 0, 1)] //Trigger Admin
        [InlineData(0, 1, 3)] //Manager
        [InlineData(1, 1, 2)] //Company Admin
        public void To_Verify_GetAllEmployeesWithoutPaginationYearWiseAsync_WithRole(int managerId, int companyId, int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.ManagerId = managerId;
            employeeModel.CompanyId = companyId;
            employeeModel.RoleId = roleId;

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _employeeCommon.Setup(x => x.GetEmployeeIdFromClaims()).Returns(managerId);
            _triggerCatalogContext.Setup(x => x.EmployeeRepository.GetAllEmployeeForTriggerAdminWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeByManagerYearwiseWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetAllEmployeeYearwiseWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetAllEmployeesWithoutPaginationYearWiseAsync(employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeesWithoutPaginationYearWiseAsync_WithException()
        {
            //Arrange
            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetAllEmployeesWithoutPaginationYearWiseAsync(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0, 3)] //Manager
        [InlineData(1, 2)] //Company Admin
        public void To_Verify_GetEmployeeByGradeWithoutPaginationAsync_WithRole(int managerId, int roleId)
        {
            //Arrange
            EmployeeModel employeeModel = EmployeeUtility.GetEmployeeModel();
            employeeModel.ManagerId = managerId;
            employeeModel.RoleId = roleId;

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _employeeCommon.Setup(x => x.GetEmployeeIdFromClaims()).Returns(managerId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwiseWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetEmployeeByGradeWithoutPaginationAsync(employeeModel);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeWithoutPaginationAsync_WithException()
        {
            //Arrange
            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetEmployeeByGradeWithoutPaginationAsync(EmployeeUtility.GetEmployeeModel());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(2)] //Company Admin
        [InlineData(3)] //Manager
        public void To_Verify_GetDashboardEmpList_WithRole(int roleId)
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(roleId);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetAllPermission(It.IsAny<int>(), It.IsAny<int>())).Returns(CommonUtility.GetActionList());
            _employeeCommon.Setup(x => x.GetEmployeeListForActions(It.IsAny<List<ActionwisePermissionModel>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetResponseWitCheckAccessDenied(It.IsAny<List<EmployeeListModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeListModels()));

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetDashboardEmpList(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(0)] // Manager Id is 0
        [InlineData(1)] //Manager Id is 1 or other
        public void To_Verify_GetDashboardEmpList_WithManagerId(int managerId)
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(3);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetAllPermission(It.IsAny<int>(), It.IsAny<int>())).Returns(CommonUtility.GetActionList());
            _employeeCommon.Setup(x => x.GetEmployeeListForActions(It.IsAny<List<ActionwisePermissionModel>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetResponseWitCheckAccessDenied(It.IsAny<List<EmployeeListModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeListModels()));

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetDashboardEmpList(1, managerId);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)] //user has action permission 
        [InlineData(false)] //user has not action permission 
        public void To_Verify_GetDashboardEmpList_WithActionPermission(bool hasActionPermission)
        {
            //Arrange
            ActionList actionList = CommonUtility.GetActionList();
            actionList.ActionPermissions.ForEach(x => x.CanView = hasActionPermission);

            _employeeCommon.Setup(x => x.GetRoleIdFromClaims()).Returns(3);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeListRepository.GetAllEmployee(It.IsAny<EmployeeListModel>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetAllPermission(It.IsAny<int>(), It.IsAny<int>())).Returns(actionList);
            _employeeCommon.Setup(x => x.GetEmployeeListForActions(It.IsAny<List<ActionwisePermissionModel>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(EmployeeUtility.GetEmployeeListModels());
            _employeeCommon.Setup(x => x.GetResponseWitCheckAccessDenied(It.IsAny<List<EmployeeListModel>>())).Returns(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeListModels()));

            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetDashboardEmpList(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetDashboardEmpList_WithException()
        {
            //Arrange
            Init_EmployeeForDashboard();

            //Act
            var result = _employeeForDashboard.GetDashboardEmpList(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
