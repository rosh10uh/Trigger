using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Widget
{
    public class Widget
    {
        private readonly ILogger<Widget> _logger;
        private readonly IConnectionContext _connectionContext;

        public Widget(IConnectionContext connectionContext, ILogger<Widget> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        public async Task<CustomJsonData> GetUserwiseWidgetAsync(int widgetType, int userId)
        {
            try
            {
                var widgetLibrary = new WidgetLibrary { widgetType = widgetType, userId = userId };
                var widgetLibraries = await Task.FromResult(_connectionContext.TriggerContext.WidgetLibraryRepository.GetUserwiseWidget(widgetLibrary));
                return JsonSettings.UserCustomDataWithStatusMessage(widgetLibraries, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        public async Task<JsonData> AddWidgetPositionAsync(List<WidgetLibrary> widgetLibrary)
        {
            try
            {
                foreach (var ls in widgetLibrary)
                {
                    ls.createdBy = ls.userId;
                }
                var insert = await Task.FromResult(_connectionContext.TriggerContext.WidgetPositionRepository.Insert(widgetLibrary));
                if (insert?.Count > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.positionsaved);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }

        }
    }
}
