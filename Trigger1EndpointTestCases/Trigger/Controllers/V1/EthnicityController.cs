using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trigger.BLL.Ethnicity;
using Trigger.DTO;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for Ethnicity
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class EthnicityController : ControllerBase
    {
        private readonly Ethnicity _ethnicity;

        /// <summary>
        /// Constructor for Ethnicity
        /// </summary>
        /// <param name="ethnicity"></param>
        public EthnicityController(Ethnicity ethnicity)
        {
            _ethnicity = ethnicity;
        }

        /// <summary>
        /// Get list of Ethnicity from Master
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<CustomJsonData> Get()
        {
            return await _ethnicity.GetAllEthnicityAsync();
        }
    }
}