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
    /// Class Name   :   DimensionElementsController
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   10 June 2019
    /// Purpose      :   Controller for Dimension Elements(Values)
    /// Revision     :  
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class DimensionElementsController : ControllerBase
    {
        private readonly DimensionElements _dimensionElements;

        /// <summary>
        /// Constructor for Dimension Elements Controller
        /// </summary>
        /// <param name="dimensionElements"></param>
        public DimensionElementsController(DimensionElements dimensionElements)
        {
            _dimensionElements = dimensionElements;
        }

        /// <summary>
        /// Get List of values for all Dimension Types
        /// </summary>
        /// <returns>returns list of values for all Dimension</returns>
        [HttpGet("{companyid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get()
        {
            return await _dimensionElements.GetDimensionElements();
        }

        /// <summary>
        /// Add new Dimension Element Values
        /// </summary>
        /// <param name="dimensionElementsModel"></param>
        /// <returns></returns>
        [HttpPost("{companyid}")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> PostAsync([FromBody] DimensionElementsModel dimensionElementsModel)
        {
            return await _dimensionElements.AddDimensionElements(dimensionElementsModel);
        }

        /// <summary>
        /// Update Existing Dimension Element Values
        /// </summary>
        /// <param name="dimensionElementsModel"></param>
        /// <returns></returns>
        [HttpPut("{companyid}")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> PutAsync([FromBody] DimensionElementsModel dimensionElementsModel)
        {
            return await _dimensionElements.UpdateDimensionElements(dimensionElementsModel);
        }

        /// <summary>
        /// Delete Existing Dimension Element
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dimensionId"></param>
        /// <param name="dimensionValueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{companyid}/{dimensionId}/{dimensionValueId}/{userId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> DeleteAsync([FromRoute]int companyId,int dimensionId,int dimensionValueId,int userId)
        {
            return await _dimensionElements.DeleteDimensionElements(dimensionId,dimensionValueId,userId);
        }
    }
}