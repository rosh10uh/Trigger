using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.BLL.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   DimensionElements
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   10 June 2019
    /// Purpose      :   Business Logic for Dimension Elements(Values)
    /// Revision     : 
    /// </summary>
    public class DimensionElements
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<DimensionElements> _logger;

        /// <summary>
        /// Constructor for Dimension Elements
        /// </summary>
        /// <param name="connectionContext">Tenant Conection Context</param>
        /// <param name="logger">Logged Error</param>
        public DimensionElements(IConnectionContext connectionContext, ILogger<DimensionElements> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        /// <summary>
        /// Get list of all Dimension Elements (Values)
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetDimensionElements()
        {
            try
            {
                List<DimensionElementsModel> dimensionwiseElements = await Task.FromResult(_connectionContext.TriggerContext.DimensionElementsRepository.SelectAll());
                List<int> dimensions = dimensionwiseElements.Select(x => x.dimensionId).Distinct().ToList();

                List<DimensionElementsListModel> dimensionElementsListModel = new List<DimensionElementsListModel>();
                foreach (int dimensionId in dimensions)
                {
                    dimensionElementsListModel.Add(
                    new DimensionElementsListModel
                    {
                        dimensionId = dimensionId,
                        dimensionType = dimensionwiseElements.FirstOrDefault(x => x.dimensionId == dimensionId).dimensionType,
                        dimensionValues = dimensionwiseElements.Where(x => x.dimensionId == dimensionId && x.id != 0).ToList()
                    });
                }

                return JsonSettings.UserCustomDataWithStatusMessage(dimensionElementsListModel, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to add new Dimensions Elements e.g.Values like Manager,Executive,Admin for Role Dimension
        /// </summary>
        /// <param name="dimensionElementsModel"></param>
        /// <returns></returns>
        public async Task<JsonData> AddDimensionElements(DimensionElementsModel dimensionElementsModel)
        {
            try
            {
                var result = await Task.FromResult(_connectionContext.TriggerContext.DimensionElementsRepository.Insert(dimensionElementsModel));

                if (result.result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.addDimensionValues);
                }
                else if (result.result == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.dimensionValueExists);
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
        /// Method to update existing Dimension Elements Details e.g. Dimension Values
        /// </summary>
        /// <param name="dimensionElementsModel"></param>
        /// <returns></returns>
        public async Task<JsonData> UpdateDimensionElements(DimensionElementsModel dimensionElementsModel)
        {
            try
            {
                var result = await Task.FromResult(_connectionContext.TriggerContext.DimensionElementsRepository.Update(dimensionElementsModel));

                switch (result.result)
                {
                    case 0:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.dimensionValueExists);
                    case 1:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.updateDimensionValues);
                    case -1:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedAcces);
                    case -2:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.invalidDimensionElementId);
                    default:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Delete method for deleting dimension element
        /// </summary>
        /// <param name="dimensionElementsModel"></param>
        /// <returns></returns>
        public async Task<JsonData> DeleteDimensionElements(int dimensionId, int dimensionValueId,int userId)
        {
            try
            {
                var result = await Task.FromResult(_connectionContext.TriggerContext.DimensionElementsRepository.Delete(new DimensionElementsModel {dimensionId=dimensionId,dimensionValueid=dimensionValueId,updatedBy= userId}));

                switch (result.result)
                {
                    case 0:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.dimensionValueAssign);
                    case 1:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.deleteDimensionElement);
                    case -1:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.DimensionElementAlreadyDeleted);
                    case -2:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_208.GetHashCode(), Messages.invalidDimensionElementId);
                    default:
                        return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }
    }
}

