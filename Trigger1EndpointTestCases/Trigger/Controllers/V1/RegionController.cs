using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Region;
using Trigger.DTO;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API for Region
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly Region _region;

        /// <summary>
        /// Constructor for Region
        /// </summary>
        /// <param name="region"></param>
        public RegionController(Region region)
        {
            _region = region;
        }

        /// <summary>
        /// Get list of Regions from Master
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<CustomJsonData> Get()
        {
            return await _region.GetAllRegionAsync();
        }
    }
}