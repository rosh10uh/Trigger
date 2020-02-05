using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.OrganizationType;
using Trigger.DTO;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API for Organization Type
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizationTypeController: ControllerBase
    {
        private readonly OrganizationType _organizationType;

        /// <summary>
        ///  Constructor for OrganizationType
        /// </summary>
        /// <param name="organizationType"></param>
        public OrganizationTypeController(OrganizationType organizationType)
        {
            _organizationType = organizationType;
        }

        /// <summary>
        /// Get list of Organization Types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<CustomJsonData>> Get()
        {
            return await _organizationType.SelectAllAsync();
        }
    }
}
