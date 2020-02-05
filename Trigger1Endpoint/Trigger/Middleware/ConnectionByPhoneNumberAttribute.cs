using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.Shared;
using Trigger.DTO.Spark;

namespace Trigger.Middleware
{
    /// <summary>
    /// Class Name   :   SparkSms
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   29 Aug 2019
    /// Purpose      :   Attribute to set context connection using registered phone number of user
    /// Revision     :
    /// </summary>
    public class ConnectionByPhoneNumberAttribute: ActionFilterAttribute
    {
        /// <summary>
        /// Connection attribute to set connecton by finding company of user by phone number 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var connection = context.HttpContext.RequestServices.GetService<DTO.Connection>();
            var catalogDbContext = context.HttpContext.RequestServices.GetService<TriggerCatalogContext>();
            string phoneNumber = context.HttpContext.Request.Form[Messages.From];
            phoneNumber= phoneNumber.Insert(phoneNumber.Length - 10, " ");
            List<AspnetUserDetails> aspnetUserDetails = catalogDbContext.EmployeeSparkRepository.GetAspnetUserByPhoneNumber(new EmployeeSparkModel { PhoneNumber = phoneNumber });
            
            if (aspnetUserDetails?.Count > 0)
            {
                var tenantName = aspnetUserDetails?.FirstOrDefault(x=>x.ClaimType== Enums.ClaimType.Key.ToString()).ClaimValue;
                var connString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], tenantName);
                connection.ConnectionString = connString;
            }

        }
    }
}
