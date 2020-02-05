using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.OrganizationType
{
    public class OrganizationType
    {
        /// <summary>
		/// Use to get service of OneAuthorityPolicyContext
		/// </summary>
		private readonly TriggerCatalogContext _catalogDbContext;

        /// <summary>
		/// Use to get service of ILogger for OrganizationType
		/// </summary>
		private readonly ILogger<OrganizationType> _iLogger;

        public OrganizationType(TriggerCatalogContext catalogDbContext, ILogger<OrganizationType> iLogger)
        {
            _catalogDbContext = catalogDbContext;
            _iLogger = iLogger;
        }

        /// <summary>
		///  This async method is responsible to get list of all Organization
		/// </summary>
		/// <returns></returns>
		public virtual async Task<CustomJsonData> SelectAllAsync()
        {
            try
            {
                var organizationType = await Task.FromResult(_catalogDbContext.OrganizationTypeRepository.SelectAll());
                return JsonSettings.UserCustomDataWithStatusMessage(organizationType, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
