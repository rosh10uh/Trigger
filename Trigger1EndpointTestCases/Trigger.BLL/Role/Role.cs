using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Role
{
    /// <summary>
    /// Role : Fetch role from Tenant - Dimension Elements,
    /// Modified by Vivek Bhavsar on 25-06-2019 change connection from catalog to tenant
    /// </summary>
    public class Role
    {
        private readonly ILogger<Role> _logger;
        private readonly IConnectionContext _connectionContext;

        public Role(IConnectionContext connectionContext, ILogger<Role> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        /// <summary>
        /// Get all roles from Dimension Elements : Modified by Vivek Bhavsar on 25-06-2019
        /// </summary>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetAllRoleAsync()
        {
            try
            {
                var roles = await Task.FromResult(_connectionContext.TriggerContext.RoleRepository.SelectAll());
                return JsonSettings.UserCustomDataWithStatusMessage(roles, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
