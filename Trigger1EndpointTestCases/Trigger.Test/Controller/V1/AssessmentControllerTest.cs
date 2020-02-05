using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V1;
using Trigger.DAL;
using Trigger.DAL.Assessment;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Assessment = Trigger.BLL.Assessment.Assessment;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;
using SharedClaims = Trigger.DAL.Shared.Claims;
using Microsoft.Extensions.Options;

namespace Trigger.Test.Controller
{
    public class AssessmentControllerTest
    {

        private readonly SetupTest _setupTest;
        private readonly Mock<ILogger<BLL_Assessment>> _logger;
        private readonly Mock<ILogger<AssessmentContext>> _loggerAssessmentContext;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<AssessmentContext> _assessmentContext;
        private readonly Mock<BLL_Assessment> _assessment;
        private readonly Mock<IActionPermission> _iActionPermission;
        private readonly Mock<SharedClaims> _iClaims;
        private readonly IOptions<AppSettings> _appSettings;

        public AssessmentControllerTest()
        {
            _setupTest = new SetupTest();
            _setupTest.Setup();
            _iActionPermission = new Mock<IActionPermission>();
            _logger = new Mock<ILogger<BLL_Assessment>>();
            _iClaims = _setupTest.SetupClaims();
            _loggerAssessmentContext = new Mock<ILogger<AssessmentContext>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _assessmentContext = new Mock<AssessmentContext>(_iConnectionContext.Object, _appSettings, _loggerAssessmentContext.Object);
            _assessment = new Mock<BLL_Assessment>(_iConnectionContext.Object, _assessmentContext.Object, _logger.Object, _iClaims.Object, _iActionPermission.Object);
        }

        [Fact]
        public void To_Verify_Get_Method_ReurnAssessmentScore()
        {
            //Arrange
            _assessment.Setup(x => x.GetAssessmentScore(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(TestUtility.GetJsonData(TestUtility.GetAssessmentScoreModel()));
            var assessmentContoller = new AssessmentController(_assessment.Object);

            //Act
            var result = assessmentContoller.Get(It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_Post_ReturnSuccessMessage()
        {
            //Arrange
            _assessment.Setup(x => x.AddAssessmentAsyncV1(It.IsAny<EmpAssessmentModel>())).ReturnsAsync(TestUtility.GetJsonData(TestUtility.GetEmpAssessmentModel()));
            var assessmentContoller = new AssessmentController(_assessment.Object);

            //Act
            var result = assessmentContoller.PostAsync(It.IsAny<EmpAssessmentModel>());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }


    }
}
