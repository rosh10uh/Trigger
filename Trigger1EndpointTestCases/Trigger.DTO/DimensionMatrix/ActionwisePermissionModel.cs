using System.Collections.Generic;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   ActionwisePermissionModel
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   10 June 2019
    /// Purpose      :   DTO class for Dimension & Action Permission
    /// Revision     : 
    /// </summary>
    public class ActionwisePermissionModel
    {
        public int Id { get; set; }//Dimensionwise Action permission Id

        [RequiredIntAttribute(ValidationMessage.DimensionwiseActionIdRequired)]
        public int ActionId { get; set; }

        public string Actions { get; set; }

        [RequiredIntAttribute(ValidationMessage.DimensionIdRequired)]
        public int DimensionId { get; set; }//Dimension Id

        public string DimensionType { get; set; }

        [RequiredIntAttribute(ValidationMessage.DimensionValueIdRequired)]
        public int DimensionValueid { get; set; }

        public string DimensionValues { get; set; }

        public bool CanView { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public int CreatedBy { get; set; }

        public int Result { get; set; }

        public int ManagerId { get; set; }

       // public List<DepartmentPermissionModel> DepartmentDimension { get; set; }

        //public ActionwisePermissionModel()
        //{
          //  DepartmentDimension = new List<DepartmentPermissionModel>();
        //}
    }

    public class DepartmentPermissionModel
    {
        public int Id { get; set; }//Dimensionwise Action permission Id

        public int ActionId { get; set; }

        public string Actions { get; set; }

        public int DimensionId { get; set; }//Dimension Id

        public string DimensionType { get; set; }

        public int DimensionValueid { get; set; }

        public string DimensionValues { get; set; }

        public bool CanView { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public int CreatedBy { get; set; }

        public int Result { get; set; }

        public int ManagerId { get; set; }

    }

    public class DimensionValuesWisePermision
    {
        public int DimensionValueId { get; set; }

        public string DimensionValues { get; set; }

        public List<ActionwisePermissionModel> ActionwisePermissionModel { get; set; }
    }

    public class DimensionElementwisePermission
    {
        public int DimensionId { get; set; }

        public string DimensionType { get; set; }

        public List<DimensionValuesWisePermision> DimensionValuesWisePermision { get; set; }
    }

    public class ActionList
    {
        public int ActionId { get; set; }

        public string Actions { get; set; }

        public List<ActionwisePermissionModel> ActionPermissions { get; set; }
    }

}
