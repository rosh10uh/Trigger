using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.IndustryType
{
	/// <summary>
	/// Contains method to manage industry.
	/// </summary>
	public class IndustryType
	{
		/// <summary>
		/// Use to get service of OneAuthorityPolicyContext
		/// </summary>
		private readonly TriggerCatalogContext _catalogDbContext;

		/// <summary>
		/// Use to get service of ILogger for IndustryType
		/// </summary>
		private readonly ILogger<IndustryType> _iLogger;

		public IndustryType(TriggerCatalogContext catalogDbContext, ILogger<IndustryType> iLogger)
		{
			_catalogDbContext = catalogDbContext;
			_iLogger = iLogger;
		}

		/// <summary>
		///  This async method is responsible to get list of all Industry
		/// </summary>
		/// <returns></returns>
		public virtual async Task<CustomJsonData> SelectAllAsync()
		{
			try
			{
				var industryType = await Task.FromResult(_catalogDbContext.IndustryTypeRepository.SelectAll());
				return JsonSettings.UserCustomDataWithStatusMessage(industryType, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
			}
			catch (Exception ex)
			{
				_iLogger.LogError(ex.Message);
				return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
			}
		}
	}
}
