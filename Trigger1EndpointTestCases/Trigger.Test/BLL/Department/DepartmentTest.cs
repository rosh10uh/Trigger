using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Department = Trigger.BLL.Department.Department;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Department
{
    public class DepartmentTest
    {

        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_Department>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;

        public DepartmentTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<BLL_Department>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAllDepartment_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.SelectAll()).Returns(TestUtility.GetDepartmentModels());
            }
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.GetAllDepartmentAsync();

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetDepartmentByCompanyAndYear_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.GetCompanyAndYearwiseDepartment(It.IsAny<DepartmentModel>())).Returns(TestUtility.GetCompanyWiseDepartmentModels());
            }
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.GetCompanyAndYearwiseDepartments(1, "2019");

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetCopmanyWiseDepartment_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.GetCompanywiseDepartment(It.IsAny<DepartmentModel>())).Returns(TestUtility.GetCompanyWiseDepartmentModels());
            }
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.GetCompanywiseDepartments(1);

            // Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_AddDepartment_WithExpectedResult(int expectedResult)
        {
            // Arrange
            var departmentData = TestUtility.GetDepartmentModel();
            departmentData.result = expectedResult;
            _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.Insert(It.IsAny<DepartmentModel>())).Returns(departmentData);
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.AddDepartmentAsync(It.IsAny<DepartmentModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddDepartment_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.Insert(It.IsAny<DepartmentModel>())).Returns(TestUtility.GetDepartmentModel());
            }
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.AddDepartmentAsync(It.IsAny<DepartmentModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_UpdateDepartment_WithExpectedResult(int expectedResult)
        {
            // Arrange
            var departmentData = TestUtility.GetDepartmentModel();
            departmentData.result = expectedResult;

            _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.Update(It.IsAny<DepartmentModel>())).Returns(departmentData);
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.UpdateDepartmentAsync(It.IsAny<DepartmentModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_UpdateDepartment_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.Update(It.IsAny<DepartmentModel>())).Returns(TestUtility.GetDepartmentModel());
            }
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.UpdateDepartmentAsync(It.IsAny<DepartmentModel>());

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_DeleteDepartment_WithExpectedResult(int expectedResult)
        {
            // Arrange
            var departmentData = TestUtility.GetDepartmentModel();
            departmentData.result = expectedResult;

            _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.Delete(It.IsAny<DepartmentModel>())).Returns(departmentData);
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.DeleteDepartmentAsync(1, 1, "1");

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_DeleteDepartment_WithException(bool isException)
        {
            // Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.DepartmentRepository.Delete(It.IsAny<DepartmentModel>())).Returns(TestUtility.GetDepartmentModel());
            }
            var department = new BLL_Department(_iConnectionContext.Object, _logger.Object);

            // Act
            var result = department.DeleteDepartmentAsync(1, 1, "1");

            // Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
