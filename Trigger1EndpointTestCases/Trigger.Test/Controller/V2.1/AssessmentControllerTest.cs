using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.Controllers.V2_1;
using Trigger.DAL.Assessment;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_Assessment = Trigger.BLL.Assessment.Assessment;
using SharedClaims = Trigger.DAL.Shared.Claims;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.Controller.V2._1
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
        public void To_Verify_Add_Assessment_Async_V2()
        {
            //Arrange
            _assessment.Setup(x => x.AddAssessmentAsyncV2_1(It.IsAny<EmpAssessmentModel>())).ReturnsAsync(TestUtility.GetJsonData(TestUtility.GetEmpAssessmentModel()));
            var assessmentContoller = new AssessmentController(_assessment.Object);

            //Act
            var result = assessmentContoller.PostAsync(new EmpAssessmentModel());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
