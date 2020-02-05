using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Country
{
    public class Country
    {
        private readonly ILogger<Country> _logger;
        private readonly TriggerCatalogContext  _catalogDbContext;

        public Country(TriggerCatalogContext catalogDbContext, ILogger<Country> logger)
        {
	        _catalogDbContext = catalogDbContext;
            _logger = logger;
        }

        public virtual async Task<CustomJsonData> GetAllCountryAsync()
        {
            try
            {
                var countries = await Task.FromResult(_catalogDbContext.CountryRepository.SelectAll());                
                return JsonSettings.UserCustomDataWithStatusMessage(countries, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
