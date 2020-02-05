using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Assessment;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;
using BLL_Assessment = Trigger.BLL.Assessment.Assessment;
using SharedClaims = Trigger.DAL.Shared.Claims;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.Assessment
{
    public class AssessmentTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_Assessment>> _logger;
        private readonly Mock<ILogger<AssessmentContext>> _loggerAssessmentContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<AssessmentContext> _assessmentContext;
        private readonly Mock<IActionPermission> _iActionPermission;
        private readonly Mock<SharedClaims> _iclaims;
        private readonly IOptions<AppSettings> _appSettings;

        public AssessmentTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<BLL_Assessment>>();
            _loggerAssessmentContext = new Mock<ILogger<AssessmentContext>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _iActionPermission = new Mock<IActionPermission>();
            _iclaims = _setupTest.SetupClaims();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _assessmentContext = new Mock<AssessmentContext>(_iConnectionContext.Object, _appSettings, _loggerAssessmentContext.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAssessmentScore_WithNullData(bool hasNullData)
        {
            //Arrange
            AssessmentScoreModel assessmentScoreModel = hasNullData ? null : TestUtility.GetAssessmentScoreModel();
            _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(assessmentScoreModel);
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);

            //Act
            var result = assessment.GetAssessmentScore(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAssessmentScore_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(TestUtility.GetAssessmentScoreModel());
            }
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);

            //Act
            var result = assessment.GetAssessmentScore(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void To_Verify_AddAssessmentAsyncV1_WithInsertedRecordResult(int insertedRecordResult)
        {
            //Arrange
            _assessmentContext.Setup(x => x.AddAssessmentMainV1(It.IsAny<EmpAssessmentModel>())).Returns(insertedRecordResult);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(TestUtility.GetAssessmentScoreModel());
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);

            //Act
            var result = assessment.AddAssessmentAsyncV1(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddAssessmentAsyncV1_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _assessmentContext.Setup(x => x.AddAssessmentMainV1(It.IsAny<EmpAssessmentModel>())).Returns(1);
                _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(TestUtility.GetAssessmentScoreModel());
            }
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);

            //Act
            var result = assessment.AddAssessmentAsyncV1(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddAssessmentAsyncV1_WithGetAssessmentScoreException(bool isException)
        {
            //Arrange
            _assessmentContext.Setup(x => x.AddAssessmentMainV1(It.IsAny<EmpAssessmentModel>())).Returns(1);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(TestUtility.GetAssessmentScoreModel());
            }
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);
            //Act
            var result = assessment.AddAssessmentAsyncV1(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddAssessmentAsyncV2_WithCanAddPermission(bool canAdd)
        {
            //Arrange
            List<ActionList> actionLists = TestUtility.GetActionLists();
            actionLists[0].ActionPermissions[0].CanAdd = canAdd;

            _iActionPermission.Setup(x => x.GetPermissions(It.IsAny<int>())).Returns(actionLists);
            _assessmentContext.Setup(x => x.AddAssessmentMainV1(It.IsAny<EmpAssessmentModel>())).Returns(1);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(TestUtility.GetAssessmentScoreModel());
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);

            //Act
            var result = assessment.AddAssessmentAsyncV2(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddAssessmentAsyncV2_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _iActionPermission.Setup(x => x.GetPermissions(It.IsAny<int>())).Returns(TestUtility.GetActionLists());
                _assessmentContext.Setup(x => x.AddAssessmentMainV1(It.IsAny<EmpAssessmentModel>())).Returns(1);
                _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(TestUtility.GetAssessmentScoreModel());
            }
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);

            //Act
            var result = assessment.AddAssessmentAsyncV2(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddAssessmentAsyncV2_WithGetAssessmentScoreException(bool isException)
        {
            //Arrange
            _iActionPermission.Setup(x => x.GetPermissions(It.IsAny<int>())).Returns(TestUtility.GetActionLists());
            _assessmentContext.Setup(x => x.AddAssessmentMainV1(It.IsAny<EmpAssessmentModel>())).Returns(1);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(TestUtility.GetAssessmentScoreModel());
            }
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);

            //Act
            var result = assessment.AddAssessmentAsyncV2(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddAssessmentAsyncV2_1(bool isException)
        {
            //Arrange
            _iActionPermission.Setup(x => x.GetPermissions(It.IsAny<int>())).Returns(TestUtility.GetActionLists());
            _assessmentContext.Setup(x => x.AddAssessmentMainV1(It.IsAny<EmpAssessmentModel>())).Returns(1);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModel());
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentScoreRepository.GetAssessmentScore(It.IsAny<AssessmentScoreModel>())).Returns(TestUtility.GetAssessmentScoreModel());
            }
            BLL_Assessment assessment = new BLL_Assessment(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iclaims.Object, _iActionPermission.Object);

            //Act
            var result = assessment.AddAssessmentAsyncV2_1(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);

        }
    }
}
