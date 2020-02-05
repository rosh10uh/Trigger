using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Region
{
    public class Region
    {
        private readonly ILogger<Region> _logger;
        private readonly TriggerCatalogContext _triggerCatalogContext;

        public Region(TriggerCatalogContext triggerCatalogContext, ILogger<Region> logger)
        {
            _triggerCatalogContext = triggerCatalogContext;
            _logger = logger;
        }


        public async Task<CustomJsonData> GetAllRegionAsync()
        {
            try
            {
                var regions = await Task.FromResult(_triggerCatalogContext.RegionRepository.SelectAll());                
                return JsonSettings.UserCustomDataWithStatusMessage(regions, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
