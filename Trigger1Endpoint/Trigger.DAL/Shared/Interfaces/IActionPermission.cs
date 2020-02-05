using System.Collections.Generic;
using Trigger.BLL.Shared;
using Trigger.DTO.DimensionMatrix;

namespace Trigger.DAL.Shared.Interfaces
{
   public interface IActionPermission
   {
        List<ActionList> GetPermissionAsPerApiVersion(string apiVersion, int empId);
        bool CheckActionPermission(CheckPermission checkPermission);
        bool CheckActionPermissionV2_1(CheckPermission checkPermission);
        List<ActionList> GetPermissions(int empId);
        List<ActionList> GetPermissionsV2(int empId);
        List<ActionList> GetPermissionsV2_1(int empId);
        CheckPermission GetPermissionParameters(Enums.Actions actions, Enums.PermissionType permissionType);
    }
}
