using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.BLL.Shared;
using Trigger.DTO.DimensionMatrix;

namespace Trigger.DAL.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   ActionwisePermissionRepository
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   10 June 2019
    /// Purpose      :   Repository for Configuation of Permission for Dimensionwise Actions
    /// Revision     :   Modified By Vivek Bhavsar on 21 June 2019 : Add method to get all actions
    /// </summary>
    [QueryPath("Trigger.DAL.Query.DimensionMatrix.ActionwisePermission")]
    public class ActionwisePermissionRepository : DaoRepository<ActionwisePermissionModel>
    {
        private const string invokeGetPermissions = "GetActionWisePermission";

        public List<ActionwisePermissionModel> GetPermissions(ActionwisePermissionModel actionwisePermissionModel)
        {
            return ExecuteQuery<List<ActionwisePermissionModel>>(actionwisePermissionModel, invokeGetPermissions);
        }

        /// <summary>
        /// to get list of actions from Catalog database
        /// </summary>
        /// <returns></returns>
        public List<ActionwisePermissionModel> GetAllActions()
        {
            return ExecuteQuery<List<ActionwisePermissionModel>>(new ActionwisePermissionModel(), Messages.InvokeGetAllActions);
        }
    }
}
