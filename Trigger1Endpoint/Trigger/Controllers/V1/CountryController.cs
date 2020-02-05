using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Country;
using Trigger.DTO;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for Country Master
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly Country _country;

        /// <summary>
        /// Country Constructor
        /// </summary>
        /// <param name="country"></param>
        public CountryController(Country country)
        {
            _country = country;
        }

        /// <summary>
        /// Get List of Countries from Master
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<CustomJsonData> Get()
        {
            return await _country.GetAllCountryAsync();
        }
    }
}