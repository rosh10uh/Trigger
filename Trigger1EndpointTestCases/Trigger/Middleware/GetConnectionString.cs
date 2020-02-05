using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL;
using Trigger.DAL.Shared;

namespace Trigger.Middleware
{
    /// <summary>
    /// Role base dynamic connection of Catalog or Tenant database
    /// </summary>
	public class DynamicConnectionAttribute : ActionFilterAttribute
    {
        private const string CompanyId = "CompanyId";

        /// <summary>
        /// Generate role base dynamic connection string
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var connection = context.HttpContext.RequestServices.GetService<DTO.Connection>();
            var catalogDbContext = context.HttpContext.RequestServices.GetService<TriggerCatalogContext>();
            var claim = context.HttpContext.RequestServices.GetService<IClaims>();

            if (claim["RoleId"] != null)
            {
                if (claim["RoleId"].Value == Enums.DimensionElements.TriggerAdmin.GetHashCode().ToString())
                {
                    var companyId = Convert.ToInt32(context.RouteData.Values[CompanyId] ?? GetCompanyIdFromRequest(context.ActionArguments));
                    if(companyId == 0)
                    {
                        companyId = Convert.ToInt32(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Enums.ClaimType.CompId.ToString()).Value);
                    }

                    var tenantName = catalogDbContext.CompanyDbConfig.Select<string>(new DTO.CompanyDbConfig { CompanyId = companyId });
                    var connString = ConnectionExtention.GetConnectionString(tenantName);
                    connection.ConnectionString = connString;
                }
                else
                {
                    var claimType = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Enums.ClaimType.Key.ToString());
                    var connString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], claimType.Value);
                    connection.ConnectionString = connString;
                }
            }
        }

        /// <summary>
        /// Get Companyid from request body
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        private int GetCompanyIdFromRequest(IDictionary<string, object> @object)
        {
            var companyId = 0;
            foreach (var item in @object)
            {
                var property = item.Value.GetType().GetProperty(CompanyId, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (property != null)
                {
                    companyId = Convert.ToInt32(property.GetValue(item.Value));
                    break;
                }
            }

            return companyId;
        }
    }
}