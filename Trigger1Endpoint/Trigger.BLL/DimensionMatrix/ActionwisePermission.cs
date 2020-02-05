using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.BLL.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   ActionwisePermission
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   11 June 2019
    /// Purpose      :   Business Logic for Dimension & Actionwise Permission
    /// Revision     : 
    /// </summary>
    public class ActionwisePermission
    {
        private readonly IConnectionContext _connectionContext;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly ILogger<ActionwisePermission> _logger;

        private readonly IActionPermission _actionPermission;

        /// <summary>
        /// Constructor for Dimension & Actionwise Permission
        /// </summary>
        /// <param name="connectionContext">Tenant Conection Context</param>
        /// <param name="logger">Logged Error</param>
        public ActionwisePermission(IConnectionContext connectionContext, TriggerCatalogContext catalogDbContext, ILogger<ActionwisePermission> logger, IActionPermission actionPermission)
        {
            _connectionContext = connectionContext;
            _catalogDbContext = catalogDbContext;
            _logger = logger;
            _actionPermission = actionPermission;
        }

        /// <summary>
        /// Get list of all Dimension & Actionwise Permission
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetActionwisePermission()
        {
            try
            {
                List<ActionwisePermissionModel> actionwisePermissions = await Task.FromResult(_connectionContext.TriggerContext.ActionwisePermissionRepository.SelectAll());
                List<int> dimensions = actionwisePermissions.Select(x => x.DimensionId).Distinct().ToList();

                List<DimensionElementwisePermission> dimensionElementwisePermission = (from int dimensionId in dimensions
                                                                                       select new DimensionElementwisePermission
                                                                                       {
                                                                                           DimensionId = dimensionId,
                                                                                           DimensionType = actionwisePermissions.FirstOrDefault(x => x.DimensionId == dimensionId).DimensionType,
                                                                                           DimensionValuesWisePermision = GetValuesWiseActionPermision(dimensionId, actionwisePermissions)
                                                                                       }).ToList();

                return JsonSettings.UserCustomDataWithStatusMessage(dimensionElementwisePermission, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Generates list of actionwise permission for dimension
        /// </summary>
        /// <param name="dimensionId"></param>
        /// <param name="actionwisePermissions"></param>
        /// <returns></returns>
        private List<DimensionValuesWisePermision> GetValuesWiseActionPermision(int dimensionId, List<ActionwisePermissionModel> actionwisePermissions)
        {
            return (from int dimensionValueid in actionwisePermissions.Where(x => x.DimensionId == dimensionId).Select(x => x.DimensionValueid).Distinct()
                    select new DimensionValuesWisePermision
                    {
                        DimensionValueId = dimensionValueid,
                        DimensionValues = actionwisePermissions.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueid).DimensionValues,
                        ActionwisePermissionModel = actionwisePermissions.Where(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueid).ToList()
                    }
                 ).ToList();
        }

        /// <summary>
        /// Method to save Dimension & Actionwise Permission
        /// </summary>
        /// <param name="actionwisePermissionModel"></param>
        /// <returns></returns>
        public async Task<JsonData> AddActionwisePermission(List<ActionwisePermissionModel> actionwisePermissionModel)
        {
            try
            {

                if (actionwisePermissionModel.Any(x => x.CanEdit && !x.CanView))
                {
                    return await Task.FromResult(JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_412.GetHashCode(), Messages.validatePermission));
                }

                _connectionContext.TriggerContext.BeginTransaction();

                foreach (ActionwisePermissionModel permissionModel in actionwisePermissionModel)
                {
                    var result = _connectionContext.TriggerContext.ActionwisePermissionRepository.Insert(permissionModel);
                    if (result.Result == -1)
                    {
                        _connectionContext.TriggerContext.RollBack();
                        return await Task.FromResult(JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_204.GetHashCode(), Messages.unauthorizedAcces));
                    }
                }

                _connectionContext.TriggerContext.Commit();
                return await Task.FromResult(JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.addActionwisePermission));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _connectionContext.TriggerContext.RollBack();
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to get list of actions from database
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllActionsAsync()
        {
            try
            {
                var actions = await Task.FromResult(_catalogDbContext.ActionwisePermissionRepository.GetAllActions());

                List<ActionList> actionList = (from ActionwisePermissionModel action in actions
                                               select new ActionList
                                               {
                                                   ActionId = action.ActionId,
                                                   Actions = action.Actions
                                               }).ToList();

                return JsonSettings.UserCustomDataWithStatusMessage(actionList, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   CheckWithExistingActionPermission
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   18 July 2019
        /// Purpose      :  Check given permission with current permission for all actions
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="actionwisePermissionModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> CheckWithExistingActionPermission(int empId, List<ActionList> actionwisePermissionModel)
        {
            try
            {
                List<ActionList> permission = _actionPermission.GetPermissions(empId);
                              
                if (ComparePermission(permission, actionwisePermissionModel))
                {
                   return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), string.Empty));
                }
                else
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// method to compare login permission with latest permission
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="actionwisePermissionModel"></param>
        /// <returns></returns>
        private bool ComparePermission(List<ActionList> permission, List<ActionList> actionwisePermissionModel)
        {
            bool result = false;
            try
            {
                if (permission.Count == 0 && actionwisePermissionModel.Count == 0)
                {
                    result = true;
                }

                foreach (ActionList subPermission in permission)
                {
                    List<ActionList> actionPermissions = actionwisePermissionModel.Where(x => x.ActionId == subPermission.ActionId).ToList();
                    if (actionPermissions.Count > 0)
                    {
                        if (actionPermissions.FirstOrDefault().ActionPermissions.Count == subPermission.ActionPermissions.Count)
                        {
                            foreach (var values in subPermission.ActionPermissions)
                            {
                                if (actionPermissions.FirstOrDefault().ActionPermissions.Any(x => x.DimensionId == values.DimensionId &&
                                                                                             x.DimensionValueid == values.DimensionValueid &&
                                                                                             x.CanView == values.CanView &&
                                                                                             x.CanAdd == values.CanAdd &&
                                                                                             x.CanEdit == values.CanEdit &&
                                                                                             x.CanDelete == values.CanDelete
                                                                                            ))
                                
                                {
                                    result = true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Method Name  :   CheckWithExistingActionPermissionV2
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   18 July 2019
        /// Purpose      :   Version 2 :Check given permission with current permission for all actions 
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="actionwisePermissionModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> CheckWithExistingActionPermissionV2(int empId, List<ActionList> actionwisePermissionModel)
        {
            try
            {
                List<ActionList> permission = _actionPermission.GetPermissionsV2(empId);

                if (CompareObjects.CompareObject(permission, actionwisePermissionModel))
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), string.Empty));
                }
                else
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   CheckWithExistingActionPermissionV2_1
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   10 September 2019
        /// Purpose      :   Version 2.1 :Check given permission with current permission for all actions 
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="actionwisePermissionModel"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> CheckWithExistingActionPermissionV2_1(int empId, List<ActionList> actionwisePermissionModel)
        {
            try
            {
                List<ActionList> permission = _actionPermission.GetPermissionsV2_1(empId);

                if (CompareObjects.CompareObject(permission, actionwisePermissionModel))
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), string.Empty));
                }
                else
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

    }
}
