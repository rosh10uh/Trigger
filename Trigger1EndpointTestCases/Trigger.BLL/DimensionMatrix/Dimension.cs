using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.BLL.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   Dimension
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   07 June 2019
    /// Purpose      :   Business Logic for Dimension Type
    /// Revision     :   Modified By Vivek Bhavsar on 12-06-2019 : Change connection to Catalog from Tenant.
    /// </summary>
    public class Dimension
    {
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly ILogger<Dimension> _logger;

        /// <summary>
        /// Constructor for Dimension Type
        /// </summary>
        /// <param name="catalogDbContext">Catalog DB Conection Context</param>
        /// <param name="logger">Logged Error</param>
        public Dimension(TriggerCatalogContext catalogDbContext,ILogger<Dimension> logger)
        {
            _catalogDbContext = catalogDbContext;
            _logger = logger;
        }

        /// <summary>
        /// Get list of all Dimensions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetAllDimension()
        {
            try
            {
                var dimensions = await Task.FromResult(_catalogDbContext.DimensionRepository.SelectAll());

                return JsonSettings.UserCustomDataWithStatusMessage(dimensions, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to add new Dimensions like Role,Relation,Department,Team etc.
        /// </summary>
        /// <param name="dimensionModel"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> AddDimension(DimensionModel dimensionModel)
        {
            try
            {
                var result = await Task.FromResult(_catalogDbContext.DimensionRepository.Insert(dimensionModel));

                if (result.result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.addDimension);
                }
                else if (result.result == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.dimensionExists);
                }

                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to update existing Dimension details e.g. Dimension Type
        /// </summary>
        /// <param name="dimensionModel"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> UpdateDimension(DimensionModel dimensionModel)
        {
            try
            {
                var result = await Task.FromResult(_catalogDbContext.DimensionRepository.Update(dimensionModel));

                if (result.result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.updateDimension);
                }
                else if (result.result == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.dimensionExists);
                }

                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }
    }
}
