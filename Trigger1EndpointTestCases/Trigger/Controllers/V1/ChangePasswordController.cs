using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.ChangePassword;
using Trigger.DTO;
using Trigger.Middleware;


namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Contains API's for Change Password Module
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChangePasswordController : Controller
    {
        private readonly ChangePassword _changePassword;

        /// <summary>
        /// Constructor for Change Password
        /// </summary>
        /// <param name="changePassword"></param>
        public ChangePasswordController(ChangePassword changePassword)
        {
            _changePassword = changePassword;
        }

        /// <summary>
        /// For changing existing password
        /// </summary>
        /// <param name="userChangePassword"></param>
        /// <returns></returns>
        //POST api/values
        [HttpPost]
        [Authorize]
        [ParameterValidation]
        [DynamicConnection]
        public async Task<JsonData> PostAsync([FromBody] UserChangePassword userChangePassword)
        {
            return await Task.FromResult(_changePassword.InvokeChangePassword(userChangePassword));
        }

    }
}