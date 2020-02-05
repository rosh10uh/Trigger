using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.DimensionMatrix;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// Class Name   :   ActionPermissionController
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   11 June 2019
    /// Purpose      :   Controller for Dimension and Actionwise Permission Configurations
    /// Revision     :  
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ActionPermissionController : ControllerBase
    {
        private readonly ActionwisePermission _actionwisePermission;

        /// <summary>
        /// Constructor for Dimension Elements Controller
        /// </summary>
        /// <param name="actionwisePermission"></param>
        public ActionPermissionController(ActionwisePermission actionwisePermission)
        {
            _actionwisePermission = actionwisePermission;
        }

        /// <summary>
        /// Get Configuration Details of Dimension and Actionwise permission
        /// </summary>
        /// <returns></returns>
        [HttpGet("{companyid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get()
        {
            return await _actionwisePermission.GetActionwisePermission();
        }

        /// <summary>
        /// Configuration of Dimensionwise Action Permission
        /// </summary>
        /// <param name="actionwisePermissionModel"></param>
        /// <returns></returns>
        [HttpPost("{companyid}")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidationAttribute]
        public async Task<JsonData> PostAsync([FromBody] List<ActionwisePermissionModel> actionwisePermissionModel)
        {
            return await _actionwisePermission.AddActionwisePermission(actionwisePermissionModel);
        }

        /// <summary>
        /// Get list of actions from Action Master
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [Authorize]
        public async Task<CustomJsonData> GetAllActionsAsync()
        {
            return await _actionwisePermission.GetAllActionsAsync();
        }

        /// <summary>
        /// Api to check permission with existing permission
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="existingPermissionModel"></param>
        /// <returns></returns>
        [HttpPost("checkpermission/{empId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> CheckWithExistingActionPermission(int empId, [FromBody] List<ActionList> existingPermissionModel)
        {
            return await _actionwisePermission.CheckWithExistingActionPermission(empId, existingPermissionModel);
        }
    }
}