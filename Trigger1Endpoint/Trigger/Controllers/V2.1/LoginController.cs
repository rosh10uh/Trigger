using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Trigger.BLL.Login;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V2_1
{
    /// <summary>
    /// API's for Login Module
    /// </summary>
    [ApiVersion("2.1")]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly Login _login;

        /// <summary>
        /// Constructor for Login
        /// </summary>
        /// <param name="login"></param>
        public LoginController(Login login)
        {
            _login = login;
        }

        //POST api/values
        /// <summary>
        /// Check for User and Response with user details
        /// </summary>
        /// <param name="userLoginModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [DynamicConnection]
        public JsonData PostAsync([FromBody] UserLoginModel userLoginModel)
        {
            StringValues version;
            HttpContext.Request.Headers.TryGetValue("api-version", out version);
            return _login.InvokeLogin(userLoginModel, version.ToString());
        }
    }
}