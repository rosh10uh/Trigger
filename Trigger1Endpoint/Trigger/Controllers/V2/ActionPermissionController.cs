using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.DimensionMatrix;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Middleware;

namespace Trigger.Controllers.V2
{
    /// <summary>
    /// Class Name   :   ActionPermissionController
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   11 June 2019
    /// Purpose      :   Controller for Dimension and Actionwise Permission Configurations
    /// Revision     :  
    /// </summary>
    [ApiVersion("2.0")]
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
        /// Version 2 : Api to check permission with existing permission
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="existingPermissionModel"></param>
        /// <returns></returns>
        [HttpPost("checkpermission/{empId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> CheckWithExistingActionPermission(int empId, [FromBody] List<ActionList> existingPermissionModel)
        {
            return await _actionwisePermission.CheckWithExistingActionPermissionV2(empId, existingPermissionModel);
        }
    }
}