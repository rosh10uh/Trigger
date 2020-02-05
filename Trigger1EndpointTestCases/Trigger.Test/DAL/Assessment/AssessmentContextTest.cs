using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;
using Dal_AssessmentContext = Trigger.DAL.Assessment.AssessmentContext;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.Assessment
{
    public class AssessmentContextTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<Dal_AssessmentContext>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly IOptions<AppSettings> _appSettings;

        public AssessmentContextTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<Dal_AssessmentContext>>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void To_Verify_AddAssessmentMain_WithInsertAssessmentResult(int InsertAssessmentResult)
        {
            //Arrange
            EmpAssessmentModel empAssessmentModel = TestUtility.GetEmpAssessmentModel();
            empAssessmentModel.id = InsertAssessmentResult;
            _iConnectionContext.Setup(x => x.TriggerContext.AssessmentRepository.InsertAssessment(It.IsAny<EmpAssessmentModel>())).Returns(empAssessmentModel);
            _iConnectionContext.Setup(x => x.TriggerContext.AssessmentDetailRepository.InsertAssessmentDetails(It.IsAny<List<EmpAssessmentDet>>())).Returns(TestUtility.GetEmpAssessmentDets());
            _iConnectionContext.Setup(x => x.TriggerContext.EmpAssessmentScoreRepository.UpdateScoreRank(It.IsAny<EmpAssessmentScore>())).Returns(TestUtility.GetEmpAssessmentScore());
            Dal_AssessmentContext assessmentContext = new Dal_AssessmentContext(_iConnectionContext.Object, _appSettings, _logger.Object);

            //Act
            var result = assessmentContext.AddAssessmentMainV1(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<int>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void To_Verify_AddAssessmentMain_WithInsertAssessmentDetailsResult(int InsertAssessmentDetailsResult)
        {
            //Arrange
            var empAssessmentModel = TestUtility.GetEmpAssessmentDets();
            empAssessmentModel[empAssessmentModel.Count - 1].result = InsertAssessmentDetailsResult;
            _iConnectionContext.Setup(x => x.TriggerContext.AssessmentRepository.InsertAssessment(It.IsAny<EmpAssessmentModel>())).Returns(TestUtility.GetEmpAssessmentModel());
            _iConnectionContext.Setup(x => x.TriggerContext.AssessmentDetailRepository.InsertAssessmentDetails(It.IsAny<List<EmpAssessmentDet>>())).Returns(empAssessmentModel);
            _iConnectionContext.Setup(x => x.TriggerContext.EmpAssessmentScoreRepository.UpdateScoreRank(It.IsAny<EmpAssessmentScore>())).Returns(TestUtility.GetEmpAssessmentScore());
            Dal_AssessmentContext assessmentContext = new Dal_AssessmentContext(_iConnectionContext.Object, _appSettings, _logger.Object);

            //Act
            var result = assessmentContext.AddAssessmentMainV1(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<int>(result);

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_AddAssessmentMain_WithException(bool isException)
        {
            //Arrange
            if (!isException)
            {
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentRepository.InsertAssessment(It.IsAny<EmpAssessmentModel>())).Returns(TestUtility.GetEmpAssessmentModel());
                _iConnectionContext.Setup(x => x.TriggerContext.AssessmentDetailRepository.InsertAssessmentDetails(It.IsAny<List<EmpAssessmentDet>>())).Returns(TestUtility.GetEmpAssessmentDets());
                _iConnectionContext.Setup(x => x.TriggerContext.EmpAssessmentScoreRepository.UpdateScoreRank(It.IsAny<EmpAssessmentScore>())).Returns(TestUtility.GetEmpAssessmentScore());
            }
            Dal_AssessmentContext assessmentContext = new Dal_AssessmentContext(_iConnectionContext.Object, _appSettings, _logger.Object);

            //Act
            var result = assessmentContext.AddAssessmentMainV1(TestUtility.GetEmpAssessmentModel());

            //Assert
            Assert.IsType<int>(result);
        }
    }
}
