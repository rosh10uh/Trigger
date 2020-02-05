using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.Shared;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.Middleware
{
    /// <summary>
    /// Common middleware to hit before any api hit
    /// </summary>
    public class LandingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor for Landing Middleware
        /// </summary>
        /// <param name="next"></param>
        public LandingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invoikes next action to execute base on Login response as user data
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="connection"></param>
        /// <param name="claim"></param>
        /// <param name="landingMiddlewareManager"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext, Connection connection, IClaims claim, ILandingMiddlewareManager landingMiddlewareManager)
        {
            if (claim["Key"] != null && claim["email"] != null)
            {
                var connectionString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], claim["Key"].Value);
                var userLogin = landingMiddlewareManager.CheckUserLogin(connectionString, claim["email"].Value);
                if (userLogin.Message == string.Empty)
                {
                    await _next(httpContext);
                }
                else
                {
                    var jsonObject = JsonConvert.SerializeObject(JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_402.GetHashCode(), userLogin.Message));

                    await httpContext.Response.WriteAsync(jsonObject);
                }
            }
            else
            {
                if (httpContext.Request.Path.Value == "/api/EmployeeSparkSms" && httpContext.Request.Method == "POST")
                {
                    await _next(httpContext);
                }
                else if (httpContext.Request.Path.Value.Contains("api/employee/editProfile"))
                {
                    var jsonObject = JsonConvert.SerializeObject(JsonSettings.UserCustomDataWithStatusMessage(null,BLL.Shared.Enums.StatusCodes.status_401.GetHashCode(), ""));
                    await httpContext.Response.WriteAsync(jsonObject);
                }
                else
                {
                    var jsonObject = JsonConvert.SerializeObject(JsonSettings.UserDataWithStatusMessage(null, BLL.Shared.Enums.StatusCodes.status_401.GetHashCode(), ""));
                    await httpContext.Response.WriteAsync(jsonObject);
                }

               // ((Microsoft.AspNetCore.Http.Internal.DefaultHttpRequest)((Microsoft.AspNetCore.Http.DefaultHttpContext)httpContext).Request).Path.Value.Contains("")
            }
        }
    }
}
