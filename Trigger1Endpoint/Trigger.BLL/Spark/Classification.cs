using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Utility;

namespace Trigger.BLL.Spark
{
    /// <summary>
    /// Class Name   :   Classification
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   08 Aug 2019
    /// Purpose      :   Logic to access Classification master
    /// Revision     : 
    /// </summary>
    public class Classification
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<Classification> _logger;

        /// <summary>
        /// Constructor for Classification
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        public Classification(IConnectionContext connectionContext, ILogger<Classification> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        /// <summary>
        /// Method to get list of classifications
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllClassifications()
        {
            try
            {
                List<ClassificationModel> classificationModel = await Task.FromResult(_connectionContext.TriggerContext.ClassificationRepository.SelectAll());

                return JsonSettings.UserCustomDataWithStatusMessage(classificationModel, Enums.StatusCodes.status_200.GetHashCode(), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }
    }
}
