using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Spark;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Class Name   :   ClassificationController
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   08 Aug 2019
    /// Purpose      :   Controller for Classification
    /// Revision     :  
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassificationController : ControllerBase
    {
        private readonly Classification _classification;

        /// <summary>
        /// Constructor to initialized BLL object for classification
        /// </summary>
        /// <param name="classification"></param>
        public ClassificationController(Classification classification)
        {
            _classification = classification;
        }

        /// <summary>
        /// API to get list of classifications from master table
        /// </summary>
        /// <returns></returns>
        [HttpGet("{companyId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get()
        {
            return await _classification.GetAllClassifications();
        }
    }
}