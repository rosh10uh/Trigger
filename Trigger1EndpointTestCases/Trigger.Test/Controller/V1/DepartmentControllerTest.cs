using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Department = Trigger.BLL.Department.Department;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller
{
    public class DepartmentControllerTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<ILogger<BLL_Department>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<BLL_Department> _department;

        public DepartmentControllerTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _logger = new Mock<ILogger<BLL_Department>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _department = new Mock<BLL_Department>(_iConnectionContext.Object, _logger.Object);
        }

        [Fact]
        public void To_Verify_Get_Method_ReturnDepartmentList()
        {
            // Arrange
            _department.Setup(x => x.GetAllDepartmentAsync()).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetDepartmentModels()));
            var departmentController = new DepartmentController(_department.Object);

            // Act
            var result = departmentController.Get();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(1, "2019")]
        public void To_Verify_Get_CompanyAndYearwiseDepartments_ReturnDepartmentList(int companyId, string year)
        {
            // Arrange
            _department.Setup(x => x.GetCompanyAndYearwiseDepartments(companyId, year)).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetCompanyWiseDepartmentModels()));
            var departmentController = new DepartmentController(_department.Object);

            // Act
            var result = departmentController.GetCompanyAndYearwiseDepartments(companyId, year);

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(1)]
        public void To_Verify_Get_CompanywiseDepartments_ReturnDepartmentList(int companyId)
        {
            // Arrange
            _department.Setup(x => x.GetCompanywiseDepartments(companyId)).ReturnsAsync(TestUtility.GetCustomJsonData(TestUtility.GetCompanyWiseDepartmentModels()));
            var departmentController = new DepartmentController(_department.Object);

            // Act
            var result = departmentController.GetCompanywiseDepartments(companyId);

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Post_Method_ReturnStatusCodeWithMessage()
        {
            // Arrange
            _department.Setup(x => x.AddDepartmentAsync(It.IsAny<DepartmentModel>())).ReturnsAsync(TestUtility.GetJsonData());
            var departmentController = new DepartmentController(_department.Object);

            // Act
            var result = departmentController.PostAsync(It.IsAny<DepartmentModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Put_Method_ReturnStatusCodeWithMessage()
        {
            // Arrange
            _department.Setup(x => x.UpdateDepartmentAsync(It.IsAny<DepartmentModel>())).ReturnsAsync(TestUtility.GetJsonData());
            var departmentController = new DepartmentController(_department.Object);

            // Act
            var result = departmentController.PutAsync(It.IsAny<DepartmentModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(1, 1, "1")]
        public void To_Verify_Delete_Method_ReturnStatusCodeWithMessage(int companyId,int departmentId,string updatedBy)
        {
            // Arrange
            _department.Setup(x => x.DeleteDepartmentAsync(companyId, departmentId, updatedBy)).ReturnsAsync(TestUtility.GetJsonData());
            var departmentController = new DepartmentController(_department.Object);

            // Act
            var result = departmentController.DeleteAsync(companyId, departmentId, updatedBy);

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
