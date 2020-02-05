namespace Trigger.DTO.DimensionMatrix
{
    /// <summary>
    /// DTO to carry require parameters for permission check.
    /// </summary>
    public class CheckPermission
    {
        public int EmpId { get; set; }

        public int RoleId { get; set; }

        public int ActionId { get; set; }

        public string PermissionType { get; set; }
    }
}
