using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.AssessmentYear
{
    public class AssessmentYear
    {       
        private readonly ILogger<AssessmentYear> _logger;
        private readonly IConnectionContext _connectionContext;
        public AssessmentYear(IConnectionContext connectionContext, ILogger<AssessmentYear> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        public async Task<CustomJsonData> GetAssessmentYearAsync(int CompanyId, int ManagerId)
        {
            try
            {
                var assessmentYearModel = new AssessmentYearModel { CompanyId = CompanyId, ManagerId = ManagerId };
                var assessmentYears = await Task.FromResult(_connectionContext.TriggerContext.AssessmentYearRepository.GetAssessmentYear(assessmentYearModel));                
                if (assessmentYears.Count > 0)
                {                    
                    return JsonSettings.UserCustomDataWithStatusMessage(assessmentYears, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {                    
                    return JsonSettings.UserCustomDataWithStatusMessage(assessmentYears, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.dataNotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
