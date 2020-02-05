using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Ethnicity
{
    public class Ethnicity
    {
        private readonly ILogger<Ethnicity> _logger;
        private readonly TriggerCatalogContext _triggerCatalogContext;
        public Ethnicity(TriggerCatalogContext triggerCatalogContext, ILogger<Ethnicity> logger)
        {
            _triggerCatalogContext = triggerCatalogContext;
            _logger = logger;
        }

        public virtual async Task<CustomJsonData> GetAllEthnicityAsync()
        {
            try
            {
                var ethnicities = await Task.FromResult(_triggerCatalogContext.EthnicityRepository.SelectAll());
                return JsonSettings.UserCustomDataWithStatusMessage(ethnicities, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
