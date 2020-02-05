using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.IndustryType;
using Trigger.DTO;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API for Industry Type
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IndustryTypeController : ControllerBase
    {
        private readonly IndustryType _industryType;

        /// <summary>
        /// Constructor for IndustryType
        /// </summary>
        /// <param name="industryType"></param>
	    public IndustryTypeController(IndustryType industryType)
        {
            _industryType = industryType;
        }

        /// <summary>
        /// Get list of Industry Types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<CustomJsonData>> Get()
        {
            return await _industryType.SelectAllAsync();
        }
    }
}
