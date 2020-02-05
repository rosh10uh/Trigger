using OneRPP.Restful.Contracts.Enum;
using OneRPP.Restful.Contracts.Services;
using System.Linq;
using Trigger.DAL.Shared;

namespace Trigger.BLL.Shared
{
    public static class ConnectionExtention
    {
        public static void UseSqlConnection(this IContext context)
        {
            var claim = context.reqobj.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Enums.ClaimType.Key.ToString());
            string connString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], claim.Value);
            context.data.AddConnection(Messages.tenantConnectionString, connString, DbEngine.MsSql);
        }

        public static string GetConnectionString(string tenantName)
        {
            string connectionString = string.Format(Messages.conntectionstring, Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.DBServerName.ToString()], tenantName, Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.DBUserName.ToString()], Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.DBPassword.ToString()]);
            return connectionString;
        }
    }
}
