using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trigger.BLL.Login;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API for Logout from Mobile Application
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly Login _login;

        /// <summary>
        /// Logout Constructor
        /// </summary>
        /// <param name="login"></param>
        public LogoutController(Login login)
        {
            _login = login;
        }

        /// <summary>
        /// API to Delete device info on Logout
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpDelete("api/Logout/{deviceId}")]
        [Authorize]
        [DynamicConnection]
        public JsonData DeleteAsync(string deviceId)
        {
            return _login.InvokeDeleteDeviceInfo(deviceId);
        }
    }
}