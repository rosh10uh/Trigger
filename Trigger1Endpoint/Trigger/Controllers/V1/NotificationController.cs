using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Notification;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for Notification
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly Notification _notification;

        /// <summary>
        /// Constructor for Notificaiton
        /// </summary>
        /// <param name="notification"></param>
        public NotificationController(Notification notification)
        {
            _notification = notification;
        }

        /// <summary>
        /// Get list of all Notifications for manager
        /// </summary>
        /// <param name="managerId"></param>
        /// <returns></returns>
        [HttpGet("{managerId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get(int managerId)
        {
            return await _notification.GetAllNotification(managerId);
        }

        /// <summary>
        /// Update Notification Status as Read on user's action on notification
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPut("{ids}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> PutAsync(string ids)
        {
            return await _notification.UpdateNotificationMarkAsReadAsync(ids);
        }
    }
}