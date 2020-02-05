namespace Trigger.DTO
{
    public class EmployeeBasicModel
    {
        public int EmpId { get; set; }
        public string EmployeeId { get; set; }
        public int Companyid { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public int ManagerId { get; set; }
        public bool EmpStatus { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public string EmpIdList { get; set; }
    }
}