using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.Employee;
using Trigger.BLL.Notification;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using BLL_EmpyeeProfile = Trigger.BLL.Employee.EmployeeProfile;
using SharedClaims = Trigger.DAL.Shared.Claims;

namespace Trigger.Test.Controller.V1
{
    public class EmployeeControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly IOptions<AppSettings> _AppSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private readonly Mock<EmployeeCommon> _employeeCommon;
        private readonly Mock<BLL_EmpyeeProfile> _employeeProfile;
        private readonly Mock<EmployeeForDashboard> _employeeForDashboard;
        private readonly Mock<EmployeeSendEmail> _employeeSendEmail;
        private readonly Mock<Employee> _employee;
        private readonly Mock<Notification> _Notification;
        private readonly Mock<IActionPermission> _iActionPermission;

        public EmployeeControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _AppSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _iActionPermission = new Mock<IActionPermission>();
            _employeeCommon = new Mock<EmployeeCommon>(_triggerCatalogContext.Object, _iConnectionContext.Object, null, null, _AppSettings, _iClaims.Object, null);
            _employeeProfile = new Mock<BLL_EmpyeeProfile>(_employeeCommon.Object, _iConnectionContext.Object, null, _triggerCatalogContext.Object, _AppSettings);
            _employeeForDashboard = new Mock<EmployeeForDashboard>(_triggerCatalogContext.Object, _iConnectionContext.Object, null, _employeeCommon.Object);
            _employeeSendEmail = new Mock<EmployeeSendEmail>(null, null, _AppSettings, _triggerCatalogContext.Object, null, _employeeCommon.Object);
            _Notification = new Mock<Notification>(_iConnectionContext.Object, null, _AppSettings);
            _employee = new Mock<Employee>(_iConnectionContext.Object, null, _triggerCatalogContext.Object,
                _employeeSendEmail.Object, _employeeCommon.Object, _Notification.Object,_iActionPermission.Object);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByIdForEditProfile()
        {
            // Arrange
            _employeeProfile.Setup(x => x.GetEmployeeByIdForEditProfileAsync(It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeProfiles()));

            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object,
                _employeeForDashboard.Object, _employeeSendEmail.Object);

            // Act
            var result = employeeController.GetEmployeeByIdForEditProfile(It.IsAny<int>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_EditProfile()
        {
            // Arrange
            _employeeProfile.Setup(x => x.UpdateProfileAsync(It.IsAny<int>(), It.IsAny<DTO.EmployeeProfileModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeProfile()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            // Act
            var result = employeeController.EditProfile(It.IsAny<int>(), It.IsAny<DTO.EmployeeProfileModel>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_AllowSms()
        {
            // Arrange
            _employeeProfile.Setup(x => x.UpdateAllowSmsAsync(It.IsAny<int>(), It.IsAny<DTO.EmployeeProfileModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeSmsNotification()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            // Act
            var result = employeeController.AllowSms(It.IsAny<int>(), It.IsAny<DTO.EmployeeProfileModel>());

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_ChangeProfile()
        {
            // Arrange
            _employeeProfile.Setup(x => x.UpdateProfilePicAsync(It.IsAny<int>(), It.IsAny<DTO.EmployeeProfilePicture>())).ReturnsAsync(CommonUtility.GetJsonData());
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            // Act

            var result = employeeController.ChangeProfile(It.IsAny<int>(), It.IsAny<DTO.EmployeeProfilePicture>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_AllEmployeesWithPaginationYearWise()
        {
            //Arrange
            _employeeForDashboard.Setup(x => x.GetAllEmployeesWithPaginationYearWiseAsync(It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetAllEmployeesWithPaginationYearWise(1, 1, 1, 1, 10, "", "1,2,3,4");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByGradeWithPagination()
        {
            //Arrange
            _employeeForDashboard.Setup(x => x.GetEmployeeByGradeWithPaginationAsync(It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByGradeWithPagination(1, 1, 1, "A", 1, 10, "", "1,2,3,4");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByGradeAndMonthWithPagination()
        {
            //Arrange
            _employeeForDashboard.Setup(x => x.GetEmployeeByGradeAndMonthWithPaginationAsync(It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByGradeAndMonthWithPagination(1, 1, 1, "March", "1", 1, 10, "", "1,2,3,4");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByGradeWithoutPagination()
        {
            //Arrange
            _employeeForDashboard.Setup(x => x.GetEmployeeByGradeWithoutPaginationAsync(It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByGradeWithoutPagination(1, 1, 1, "A", "1,2,3,4");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_AllEmployeesWithoutPaginationYearWise()
        {
            //Arrange
            _employeeForDashboard.Setup(x => x.GetAllEmployeesWithoutPaginationYearWiseAsync(It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetAllEmployeesWithoutPaginationYearWise(1, 1, 1, "1,2,3,4");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByGradeAndMonthWithoutPagination()
        {
            //Arrange
            _employeeForDashboard.Setup(x => x.GetEmployeeByGradeAndMonthWithoutPaginationAsync(It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByGradeAndMonthWithoutPagination(1, 1, 1, "March", "A", "1,2,3,4");

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Post()
        {
            //Arrange
            _employee.Setup(x => x.InsertAsync(It.IsAny<int>(), It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData());
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.Post(It.IsAny<int>(), It.IsAny<EmployeeModel>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Put()
        {
            //Arrange
            _employee.Setup(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData());
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.Put(It.IsAny<int>(), It.IsAny<EmployeeModel>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Delete()
        {
            //Arrange
            _employee.Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData());
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.Delete(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_ChangeEmpSalary()
        {
            //Arrange
            _employee.Setup(x => x.UpdateEmpSalaryAsync(It.IsAny<int>(), It.IsAny<EmployeeSalary>())).ReturnsAsync(CommonUtility.GetJsonData());
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.ChangeEmpSalary(It.IsAny<int>(), It.IsAny<EmployeeSalary>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_EmployeeSendMail()
        {
            //Arrange
            _employeeSendEmail.Setup(x => x.SendMailAndUpdateFlag(It.IsAny<int>(), It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData());
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.EmployeeSendMail(It.IsAny<int>(), It.IsAny<EmployeeModel>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeById()
        {
            //Arrange
            _employee.Setup(x => x.GetEmployeeByIdAsync(It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModel()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeById(It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByIdWithCompanyId()
        {
            //Arrange
            _employee.Setup(x => x.GetEmployeeByIdAsync(It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModel()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByIdWithCompanyId(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_CompanyAdimnById()
        {
            //Arrange
            _employee.Setup(x => x.GetEmployeeByIdAsync(It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModel()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetCompanyAdimnById(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get()
        {
            //Arrange
            _employee.Setup(x => x.GetAllEmployeesAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.Get(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_AllManager()
        {
            //Arrange
            _employee.Setup(x => x.GetCompanyWiseEmployeeAsync(It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetAllManager(It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_AllManagers_SD()
        {
            //Arrange
            _employee.Setup(x => x.GetCompanyWiseEmployeeWithPaginationAsync(It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetAllManagers_SD(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_AllEmployee_SD()
        {
            //Arrange
            _employee.Setup(x => x.GetAllEmployeesWithPaginationAsync(It.IsAny<EmployeeModel>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetAllEmployee_SD(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByGrade()
        {
            //Arrange
            _employee.Setup(x => x.GetEmployee()).ReturnsAsync(CommonUtility.GetJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByGrade(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByGradeAndMonth()
        {
            //Arrange
            _employee.Setup(x => x.GetEmployee()).ReturnsAsync(CommonUtility.GetJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByGradeAndMonth(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByGradeWithPagination1()
        {
            //Arrange
            _employee.Setup(x => x.GetEmployee()).ReturnsAsync(CommonUtility.GetJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByGradeWithPagination(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_EmployeeByGradeAndMonthWithPagination1()
        {
            //Arrange
            _employee.Setup(x => x.GetEmployee()).ReturnsAsync(CommonUtility.GetJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetEmployeeByGradeAndMonthWithPagination(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_AllEmployees()
        {
            //Arrange
            _employee.Setup(x => x.GetAllEmployeesWithoutPaginationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetAllEmployees(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_AllEmployeesListForTriggerAdmin()
        {
            //Arrange
            _employee.Setup(x => x.GetCompanyWiseEmployeeWithoutPaginationAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetAllEmployeesListForTriggerAdmin(It.IsAny<int>(), It.IsAny<string>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_TriggerEmpList()
        {
            //Arrange
            _employee.Setup(x => x.GetTriggerEmpList(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetTriggerEmpList(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Get_DashboardEmpList()
        {
            //Arrange
            _employeeForDashboard.Setup(x => x.GetDashboardEmpList(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(CommonUtility.GetCustomJsonData(EmployeeUtility.GetEmployeeModels()));
            var employeeController = new EmployeeController(_employee.Object, _employeeProfile.Object, _employeeForDashboard.Object, _employeeSendEmail.Object);

            //Act
            var result = employeeController.GetDashboardEmpList(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
