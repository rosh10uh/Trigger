using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Trigger.Utility;
using static Trigger.BLL.Shared.Enums;

namespace Trigger.Middleware
{
    /// <summary>
    /// Parameter Validation Attribute
    /// </summary>
    public class ParameterValidationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// validate parametrs from context
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.status_412.GetHashCode();
                var errors = context.ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                context.Result = new JsonResult(JsonSettings.UserDataWithStatusMessage(null, StatusCodes.status_412.GetHashCode(), errors[0][0].ErrorMessage));
            }
        }
    }
}
