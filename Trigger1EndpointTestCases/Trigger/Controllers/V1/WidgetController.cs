using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.Widget;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for Widget Master for Dashboards
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetController : ControllerBase
    {
        private readonly Widget _widget;

        /// <summary>
        /// Constructor for Widget
        /// </summary>
        /// <param name="widget"></param>
        public WidgetController(Widget widget)
        {
            _widget = widget;
        }

        // GET: api/widget  
        /// <summary>
        /// Get Widget Details by UserId
        /// </summary>
        /// <param name="widgetType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{widgetType}/{userId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get(int widgetType, int userId)
        {
            return await _widget.GetUserwiseWidgetAsync(widgetType, userId);
        }

        /// <summary>
        /// Save Widget Positions for Dashboard (Userwise)
        /// </summary>
        /// <param name="widgetLibrary"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> Post([FromBody] List<WidgetLibrary> widgetLibrary)
        {
            return await _widget.AddWidgetPositionAsync(widgetLibrary);
        }
    }
}