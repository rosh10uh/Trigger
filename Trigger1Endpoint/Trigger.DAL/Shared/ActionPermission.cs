using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.DAL.Shared
{
    /// <summary>
    /// Class to check action permission for particular action
    /// </summary>
    public class ActionPermission : IActionPermission
    {
        private readonly IConnectionContext _connectionContext;
        private readonly IClaims _iClaims;

        public List<ActionList> GetPermissionAsPerApiVersion(string apiVersion, int empId)
        {
            if (apiVersion == Messages.DefaultApiVersion)
            {
                return GetPermissions(empId);
            }
            else if (apiVersion == Messages.ApiVersionV2)
            {
                return GetPermissionsV2(empId);
            }
            else
            {
                return GetPermissionsV2_1(empId);
            }
        }

        
        public ActionPermission(IConnectionContext connectionContext, IClaims claims)
        {
            _connectionContext = connectionContext;
            _iClaims = claims;
        }

        public bool CheckActionPermission(CheckPermission checkPermission)
        {
            if (checkPermission.RoleId == Enums.DimensionElements.CompanyAdmin.GetHashCode())
                return true;

            var permissions = GetPermissionsV2(checkPermission.EmpId).Where(x => x.ActionId == checkPermission.ActionId).ToList();

            if (permissions.Count > 0)
            {
                BindingFlags bindingFlags = BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

                var actionPermission = permissions.FirstOrDefault().ActionPermissions.ToList();

                return actionPermission.Any(x => Convert.ToBoolean(x.GetType().GetProperty(checkPermission.PermissionType, bindingFlags).GetValue(x)));
                
            }

            return false;
        }

        public bool CheckActionPermissionV2_1(CheckPermission checkPermission)
        {
            if (checkPermission.RoleId == Enums.DimensionElements.CompanyAdmin.GetHashCode())
                return true;

            var permissions = GetPermissionsV2_1(checkPermission.EmpId).Where(x => x.ActionId == checkPermission.ActionId).ToList();

            if (permissions.Count > 0)
            {
                BindingFlags bindingFlags = BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

                var actionPermission = permissions.FirstOrDefault().ActionPermissions.ToList();

                return actionPermission.Any(x => Convert.ToBoolean(x.GetType().GetProperty(checkPermission.PermissionType, bindingFlags).GetValue(x)));

            }

            return false;
        }

        #region old implementation checking of permission with Department Dimension
        /// <summary>
        /// Version 2 : Generate final array for actionwise permission for login employee with department dimension matrix
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        //public List<ActionList> GetPermissionsV2(int empId)
        //{
        //    List<ActionwisePermissionModel> allPermissions = _connectionContext.TriggerContext.ActionwisePermissionRepository.GetPermissions(new ActionwisePermissionModel { ManagerId = empId });
        //    List<int> actionIds = allPermissions.Select(x => x.ActionId).Distinct().ToList();
        //    List<ActionwisePermissionModel> permission;
        //    List<ActionList> finalPermission = new List<ActionList>();
        //    ActionList actionList;

        //    DataTable dimensionValues;

        //    foreach (int actionId in actionIds)
        //    {
        //        permission = allPermissions.Where(x => x.ActionId == actionId).ToList();

        //        dimensionValues = ConvertToDataTable.ToDataTable(permission);
        //        dimensionValues = dimensionValues.DefaultView.ToTable(true, "dimensionId", "dimensionType", "dimensionValueid", "dimensionValues");

        //        actionList = new ActionList
        //        {
        //            ActionId = actionId,
        //            Actions = permission.FirstOrDefault().Actions
        //        };
        //        actionList.ActionPermissions = new List<ActionwisePermissionModel>();

        //        string criteria = string.Empty;
        //        if (permission.Any(x => x.DimensionId > Enums.DimensionType.Role.GetHashCode()))
        //        {
        //            if (permission.Any(x => x.DimensionId == Enums.DimensionType.Relation.GetHashCode()))
        //            {
        //                criteria = string.Format("dimensionid not in ({0}, {1})", Enums.DimensionType.Role.GetHashCode(), Enums.DimensionType.Department.GetHashCode());
        //            }
        //            else
        //            {
        //                criteria = string.Format("dimensionid <> {0}", Enums.DimensionType.Role.GetHashCode());
        //            }

        //        }

        //        foreach (DataRow dr in dimensionValues.Select(criteria))
        //        {
        //            actionList.ActionPermissions.Add(SetActionwisePermissionModelV2(dr, permission));
        //        }
        //        finalPermission.Add(actionList);
        //    }

        //    return finalPermission;
        //}

        ///// <summary>
        ///// Set response for action permission for particular action 
        ///// </summary>
        ///// <param name="dr"></param>
        ///// <param name="permission"></param>
        ///// <returns></returns>
        //private ActionwisePermissionModel SetActionwisePermissionModelV2(DataRow dr, List<ActionwisePermissionModel> permission)
        //{
        //    int dimensionId = Convert.ToInt32(dr["dimensionid"]);
        //    int dimensionValueId = Convert.ToInt32(dr["dimensionValueId"]);

        //    ActionwisePermissionModel actionwisePermission = new ActionwisePermissionModel();

        //    actionwisePermission.DimensionId = dimensionId;
        //    actionwisePermission.DimensionType = Convert.ToString(dr["dimensionType"]);

        //    actionwisePermission.DimensionValueid = dimensionValueId;
        //    actionwisePermission.DimensionValues = Convert.ToString(dr["dimensionValues"]);

        //    List<ActionwisePermissionModel> dimensionwiseActPermission = new List<ActionwisePermissionModel>();

        //    if (permission.Any(x => x.DimensionId == Enums.DimensionType.Role.GetHashCode()))
        //    {
        //        actionwisePermission.CanView = permission.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueId).CanView;
        //        actionwisePermission.CanAdd = permission.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueId).CanAdd;
        //        actionwisePermission.CanEdit = permission.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueId).CanEdit;
        //        actionwisePermission.CanDelete = permission.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueId).CanDelete;

        //        dimensionwiseActPermission.Add(actionwisePermission);
        //    }
        //    dimensionwiseActPermission.AddRange(permission.Where(x => x.DimensionId != dimensionId && x.DimensionId < Enums.DimensionType.Relation.GetHashCode()));
        //    var departmentValues = permission.Where(x => x.DimensionId == Enums.DimensionType.Department.GetHashCode()).ToList();

        //    if (departmentValues.Count > 0 && dimensionId != Enums.DimensionType.Department.GetHashCode())
        //    {
        //        foreach (var dp in departmentValues.ToList())
        //        {

        //            ActionwisePermissionModel finalPermission = new ActionwisePermissionModel();

        //            dimensionwiseActPermission.AddRange(permission.Where(x => x.DimensionId == dp.DimensionId && x.DimensionValueid == dp.DimensionValueid));

        //            finalPermission.CanView = dimensionwiseActPermission.Select(x => x.CanView).Min();
        //            finalPermission.CanAdd = dimensionwiseActPermission.Select(x => x.CanAdd).Min();
        //            finalPermission.CanEdit = dimensionwiseActPermission.Select(x => x.CanEdit).Min();
        //            finalPermission.CanDelete = dimensionwiseActPermission.Select(x => x.CanDelete).Min();

        //            DepartmentPermissionModel departmentPermission = new DepartmentPermissionModel();

        //            departmentPermission.DimensionId = dp.DimensionId;
        //            departmentPermission.DimensionType = dp.DimensionType;
        //            departmentPermission.DimensionValueid = dp.DimensionValueid;
        //            departmentPermission.DimensionValues = dp.DimensionValues;

        //            departmentPermission.CanView = finalPermission.CanView;
        //            departmentPermission.CanAdd = finalPermission.CanAdd;
        //            departmentPermission.CanEdit = finalPermission.CanEdit;
        //            departmentPermission.CanDelete = finalPermission.CanDelete;

        //           // actionwisePermission.DepartmentDimension.Add(departmentPermission);

        //            dimensionwiseActPermission.RemoveRange(2, 1);

        //        }
        //    }

        //    else
        //    {
        //        actionwisePermission.CanView = dimensionwiseActPermission.Select(x => x.CanView).Min();
        //        actionwisePermission.CanAdd = dimensionwiseActPermission.Select(x => x.CanAdd).Min();
        //        actionwisePermission.CanEdit = dimensionwiseActPermission.Select(x => x.CanEdit).Min();
        //        actionwisePermission.CanDelete = dimensionwiseActPermission.Select(x => x.CanDelete).Min();
        //    }

        //    return actionwisePermission;
        //}
        #endregion

        /// <summary>
        /// Generate final array for actionwise permission for login employee only with Role & Relation Dimension
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public List<ActionList> GetPermissions(int empId)
        {
            List<ActionwisePermissionModel> allPermissions = _connectionContext.TriggerContext.ActionwisePermissionRepository.GetPermissions(new ActionwisePermissionModel { ManagerId = empId }).Where(x => x.DimensionId < Enums.DimensionType.Department.GetHashCode()).ToList();
            List<int> actionIds = allPermissions.Select(x => x.ActionId).Distinct().ToList();
            List<ActionwisePermissionModel> permission;
            List<ActionList> finalPermission = new List<ActionList>();
            ActionList actionList;

            DataTable dimensionValues;

            foreach (int actionId in actionIds)
            {
                permission = allPermissions.Where(x => x.ActionId == actionId).ToList();

                dimensionValues = ConvertToDataTable.ToDataTable(permission);
                dimensionValues = dimensionValues.DefaultView.ToTable(true, "dimensionId", "dimensionType", "dimensionValueid", "dimensionValues");

                actionList = new ActionList
                {
                    ActionId = actionId,
                    Actions = permission.FirstOrDefault().Actions
                };
                actionList.ActionPermissions = new List<ActionwisePermissionModel>();

                string criteria = string.Empty;
                if (permission.Any(x => x.DimensionId > Enums.DimensionType.Role.GetHashCode()))
                {
                    criteria = string.Format("dimensionid<> {0}", Enums.DimensionType.Role.GetHashCode());
                }

                foreach (DataRow dr in dimensionValues.Select(criteria))
                {
                    actionList.ActionPermissions.Add(SetActionwisePermissionModel(dr, permission));
                }
                finalPermission.Add(actionList);
            }

            return finalPermission;
        }

        /// <summary>
        /// Set response for action permission for particular action
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private ActionwisePermissionModel SetActionwisePermissionModel(DataRow dr, List<ActionwisePermissionModel> permission)
        {
            int dimensionId = Convert.ToInt32(dr["dimensionid"]);
            int dimensionValueId = Convert.ToInt32(dr["dimensionValueId"]);

            ActionwisePermissionModel actionwisePermission = new ActionwisePermissionModel();

            actionwisePermission.DimensionId = dimensionId;
            actionwisePermission.DimensionType = Convert.ToString(dr["dimensionType"]);

            actionwisePermission.DimensionValueid = dimensionValueId;
            actionwisePermission.DimensionValues = Convert.ToString(dr["dimensionValues"]);

            if (permission.Any(x => x.DimensionId == Enums.DimensionType.Role.GetHashCode()))
            {
                actionwisePermission.CanView = permission.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueId).CanView;
                actionwisePermission.CanAdd = permission.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueId).CanAdd;
                actionwisePermission.CanEdit = permission.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueId).CanEdit;
                actionwisePermission.CanDelete = permission.FirstOrDefault(x => x.DimensionId == dimensionId && x.DimensionValueid == dimensionValueId).CanDelete;

                List<ActionwisePermissionModel> dimensionwiseActPermission = new List<ActionwisePermissionModel>();
                dimensionwiseActPermission.Add(actionwisePermission);
                dimensionwiseActPermission.AddRange(permission.Where(x => x.DimensionId != dimensionId));


                actionwisePermission.CanView = dimensionwiseActPermission.Select(x => x.CanView).Min();
                actionwisePermission.CanAdd = dimensionwiseActPermission.Select(x => x.CanAdd).Min();
                actionwisePermission.CanEdit = dimensionwiseActPermission.Select(x => x.CanEdit).Min();
                actionwisePermission.CanDelete = dimensionwiseActPermission.Select(x => x.CanDelete).Min();
            }
            return actionwisePermission;
        }

        /// <summary>
        /// Generate final array for actionwise permission for login employee only with Role, Relation & department Dimension
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public List<ActionList> GetPermissionsV2(int empId)
        {
            List<ActionwisePermissionModel> allPermissions = _connectionContext.TriggerContext.ActionwisePermissionRepository.GetPermissions(new ActionwisePermissionModel { ManagerId = empId }).Where(x => x.DimensionId < Enums.DimensionType.Team.GetHashCode()).ToList();
            List<int> actionIds = allPermissions.Select(x => x.ActionId).Distinct().ToList();
            List<ActionwisePermissionModel> permission;
            List<ActionList> finalPermission = new List<ActionList>();
            ActionList actionList;

            foreach (int actionId in actionIds)
            {
                permission = allPermissions.Where(x => x.ActionId == actionId).ToList();

                List<ActionwisePermissionModel> dimensionValues = permission.Select(o => new ActionwisePermissionModel() { DimensionId = o.DimensionId, DimensionType = o.DimensionType, DimensionValueid = o.DimensionValueid, DimensionValues = o.DimensionValues, CanView = o.CanView, CanAdd = o.CanAdd, CanEdit = o.CanEdit, CanDelete = o.CanDelete }).ToList(); 

                actionList = new ActionList
                {
                    ActionId = actionId,
                    Actions = permission.FirstOrDefault().Actions
                };
                actionList.ActionPermissions = new List<ActionwisePermissionModel>();
                               
                if (permission.Any(x => x.DimensionId > Enums.DimensionType.Role.GetHashCode()))
                {
                    dimensionValues = dimensionValues.Where(x => x.DimensionId != Enums.DimensionType.Role.GetHashCode()).ToList();
                }

                foreach (ActionwisePermissionModel permissionModel in dimensionValues.ToList())
                {
                    actionList.ActionPermissions.Add(SetPermissionModel(permissionModel, permission));
                }
                finalPermission.Add(actionList);
            }

            return finalPermission;
        }

        /// <summary>
        /// Generate final array for actionwise permission for login employee with all dimensions (Role, Relation, Department & Team)
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public List<ActionList> GetPermissionsV2_1(int empId)
        {
            List<ActionwisePermissionModel> allPermissions = _connectionContext.TriggerContext.ActionwisePermissionRepository.GetPermissions(new ActionwisePermissionModel { ManagerId = empId }).ToList();
            List<int> actionIds = allPermissions.Select(x => x.ActionId).Distinct().ToList();
            List<ActionwisePermissionModel> permission;
            List<ActionList> finalPermission = new List<ActionList>();
            ActionList actionList;

            foreach (int actionId in actionIds)
            {
                permission = allPermissions.Where(x => x.ActionId == actionId).ToList();

                List<ActionwisePermissionModel> dimensionValues = permission.Select(o => new ActionwisePermissionModel() { DimensionId = o.DimensionId, DimensionType = o.DimensionType, DimensionValueid = o.DimensionValueid, DimensionValues = o.DimensionValues, CanView = o.CanView, CanAdd = o.CanAdd, CanEdit = o.CanEdit, CanDelete = o.CanDelete }).ToList();

                actionList = new ActionList
                {
                    ActionId = actionId,
                    Actions = permission.FirstOrDefault().Actions
                };
                actionList.ActionPermissions = new List<ActionwisePermissionModel>();

                if (permission.Any(x => x.DimensionId > Enums.DimensionType.Role.GetHashCode()))
                {
                    dimensionValues = dimensionValues.Where(x => x.DimensionId != Enums.DimensionType.Role.GetHashCode()).ToList();
                }

                foreach (ActionwisePermissionModel permissionModel in dimensionValues.ToList())
                {
                    actionList.ActionPermissions.Add(SetPermissionModel(permissionModel, permission));
                }
                finalPermission.Add(actionList);
            }

            return finalPermission;
        }

        /// <summary>
        /// Set response for action permission for particular action with intersection
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private ActionwisePermissionModel SetPermissionModel(ActionwisePermissionModel permissionModel, List<ActionwisePermissionModel> permission)
        {

            ActionwisePermissionModel rolePermission = permission.Find(x => x.DimensionId == Enums.DimensionType.Role.GetHashCode());
            if(rolePermission== null)
            {
                rolePermission = new ActionwisePermissionModel();
            }
                      
            ActionwisePermissionModel actionwisePermission = new ActionwisePermissionModel();

            actionwisePermission.DimensionId = Convert.ToInt32(permissionModel.DimensionId);
            actionwisePermission.DimensionType = Convert.ToString(permissionModel.DimensionType);
            actionwisePermission.DimensionValueid = Convert.ToInt32(permissionModel.DimensionValueid);
            actionwisePermission.DimensionValues = Convert.ToString(permissionModel.DimensionValues);
            actionwisePermission.CanAdd = Convert.ToBoolean(permissionModel.CanAdd);
            actionwisePermission.CanEdit = Convert.ToBoolean(permissionModel.CanEdit);
            actionwisePermission.CanDelete = Convert.ToBoolean(permissionModel.CanDelete);
            actionwisePermission.CanView = Convert.ToBoolean(permissionModel.CanView);

            List<ActionwisePermissionModel> dimensionwiseActPermission = new List<ActionwisePermissionModel>
            {
                rolePermission,
                actionwisePermission
            };

            actionwisePermission.CanView = dimensionwiseActPermission.Select(x => x.CanView).Min();
            actionwisePermission.CanAdd = dimensionwiseActPermission.Select(x => x.CanAdd).Min();
            actionwisePermission.CanEdit = dimensionwiseActPermission.Select(x => x.CanEdit).Min();
            actionwisePermission.CanDelete = dimensionwiseActPermission.Select(x => x.CanDelete).Min();

            return actionwisePermission;
        }

        /// <summary>
        /// method to build permission parameter object
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        public CheckPermission GetPermissionParameters(Enums.Actions actions, Enums.PermissionType permissionType)
        {
            return new CheckPermission
            {
                RoleId = Convert.ToInt32(_iClaims["RoleId"].Value),
                EmpId = Convert.ToInt32(_iClaims["EmpId"].Value),
                ActionId = actions.GetHashCode(),
                PermissionType = permissionType.ToString()
            };
        }

    }
}

