using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using BLL_AssessmentYear = Trigger.BLL.AssessmentYear.AssessmentYear;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.BLL.AssessmentYear
{
    public class AssessmentYearTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<BLL_AssessmentYear>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;

        public AssessmentYearTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<BLL_AssessmentYear>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAssessmentYearAsync_WithAssessmentYears(bool hasAssessmentYear)
        {
            //Arrange
            List<AssessmentYearModel> assessmentYearModels = hasAssessmentYear ? TestUtility.GetAssessmentYearModels() : new List<AssessmentYearModel>();
            _iConnectionContext.Setup(x => x.TriggerContext.AssessmentYearRepository.GetAssessmentYear(It.IsAny<AssessmentYearModel>())).Returns(assessmentYearModels);
            var assessmentYear = new BLL_AssessmentYear(_iConnectionContext.Object, _logger.Object);

            //Act
            var result = assessmentYear.GetAssessmentYearAsync(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_GetAssessmentYearAsync_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentYearRepository.GetAssessmentYear(It.IsAny<AssessmentYearModel>())).Returns(TestUtility.GetAssessmentYearModels());
            }
            var assessmentYear = new BLL_AssessmentYear(_iConnectionContext.Object, _logger.Object);

            //Act
            var result = assessmentYear.GetAssessmentYearAsync(1, 1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
