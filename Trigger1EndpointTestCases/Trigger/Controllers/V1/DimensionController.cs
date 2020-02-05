using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.DimensionMatrix;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Class Name   :   DimensionController
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   07 June 2019
    /// Purpose      :   Controller for Dimension
    /// Revision     :  
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class DimensionController : ControllerBase
    {
        private readonly Dimension _dimension;

        /// <summary>
        /// Constructor for Dimension Controller
        /// </summary>
        /// <param name="dimension"></param>
        public DimensionController(Dimension dimension)
        {
            _dimension = dimension;
        }

        /// <summary>
        /// Get List of all Dimension Types
        /// </summary>
        /// <returns>returns list of Dimensions</returns>
        [HttpGet()]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get()
        {
            return await _dimension.GetAllDimension();
        }

        /// <summary>
        /// Add new Dimensions
        /// </summary>
        /// <param name="dimensionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> PostAsync([FromBody] DimensionModel dimensionModel)
        {
            return await _dimension.AddDimension(dimensionModel);
        }

        /// <summary>
        /// Update Existing Dimensions
        /// </summary>
        /// <param name="dimensionModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> PutAsync([FromBody] DimensionModel dimensionModel)
        {
            return await _dimension.UpdateDimension(dimensionModel);
        }
    }
}