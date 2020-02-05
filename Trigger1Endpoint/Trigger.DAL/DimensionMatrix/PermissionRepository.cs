using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO.DimensionMatrix;

namespace Trigger.DAL.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   DimensionRepository
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   07 June 2019
    /// Purpose      :   Repository for CRUD Operation for Dimension
    /// Revision     : 
    /// </summary>
    [QueryPath("Trigger.DAL.Query.DimensionMatrix.Permission")]
    public class PermissionRepository : DaoRepository<PermissionModel>
    {

        private const string invokeGetPermissions = "GetActionWisePermission";

        public List<PermissionModel> GetPermissions(PermissionModel permissionModel)
        {
            return ExecuteQuery<List<PermissionModel>>(permissionModel, invokeGetPermissions);
        }
    }
}
