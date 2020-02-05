using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Role;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API for Role
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly Role _role;

        /// <summary>
        /// Constructor for Role Master
        /// </summary>
        /// <param name="role"></param>
        public RoleController(Role role)
        {
            _role = role;
        }

        /// <summary>
        /// Get List of Trigger Roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("{companyId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get([FromRoute] int companyId)
        {
            return await _role.GetAllRoleAsync();
        }
    }
}