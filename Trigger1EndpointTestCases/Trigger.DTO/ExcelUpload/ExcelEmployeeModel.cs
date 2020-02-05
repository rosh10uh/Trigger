using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Data;

namespace Trigger.DTO
{

    public class ExcelEmployeesModel 
    {
        public int empId { get; set; }
        public string companyId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string suffix { get; set; }
        public string email { get; set; }
        public string jobTitle { get; set; }
        public string joiningDate { get; set; }
        public string workCity { get; set; }
        public string workState { get; set; }
        public string workZipcode { get; set; }
        public string departmentId { get; set; }
        public string department { get; set; }
        public string managerId { get; set; }
        public string managerName { get; set; }
        public string managerLName { get; set; }
        public bool empStatus { get; set; }
        public string roleId { get; set; }
        public string dateOfBirth { get; set; }
        public string raceorethanicityId { get; set; }
        public string gender { get; set; }
        public string jobCategory { get; set; }
        public string jobCode { get; set; }
        public string jobGroup { get; set; }
        public string lastPromodate { get; set; }
        public decimal currentSalary { get; set; }
        public string lastIncDate { get; set; }
        public string empLocation { get; set; }
        public string countryId { get; set; }
        public string regionId { get; set; }
        public string empImgPath { get; set; }
        public bool bactive { get; set; }
        public int createdBy { get; set; }
        public string createddtstamp { get; set; }
        public int updatedBy { get; set; }
        public string updateddtstamp { get; set; }
        public string employeeId { get; set; }
        [JsonProperty("CSVManagerId")]
        public string CSVManagerId { get; set; }
        public string phonenumber { get; set; }
    }
        
    public class CountRecordModel
    {
        public string CompanyId { get; set; }
        public int NewlyInserted { get; set; }
        public int MismatchRecord { get; set; }
        [JsonProperty("lstNewCsvUpload")]
        public List<ExcelEmployeesModel> LstNewExcelUpload { get; set; }
        [JsonProperty("lstMisMatchCsvUpload")]
        public List<ExcelEmployeesModel> LstMisMatchExcelUpload { get; set; }
        [JsonProperty("lstExistPhoneCsvUpload")]
        public List<ExcelEmployeesModel> LstExistPhoneExcelUpload { get; set; }

        public CountRecordModel()
        {
            LstNewExcelUpload = new List<ExcelEmployeesModel>();
            LstMisMatchExcelUpload = new List<ExcelEmployeesModel>();
            LstExistPhoneExcelUpload = new List<ExcelEmployeesModel>();
        }
    }
    //public class NewlyInserted
    //{
    //    public List<CsvEmployeesModel> lstNewCsvUpload { get; set; }
    //}
    //public class MismatchData
    //{
    //    public List<CsvEmployeesModel> lstMismatchUpload { get; set; }
    //}

    public class ExcelData
    {
        public int NewlyInserted { get; set; }
        public int MismatchRecord { get; set; }
        public string Source { get; set; }
        public int EmpId { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyId { get; set; }
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string JobTitle { get; set; }
        public string JoiningDate { get; set; }
        public string Email { get; set; }
        public string WorkCity { get; set; }
        public string WorkState { get; set; }
        public string WorkZipCode { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string ManagerLName { get; set; }
        public int EmpStatus { get; set; }
        public int RoleId { get; set; }
        public string DateOfBirth { get; set; }
        public int RaceOrEthanicityId { get; set; }
        public string Gender { get; set; }
        public string JobCategory { get; set; }
        public int JobCategoryId { get; set; }
        public int JobCodeId { get; set; }
        public string JobCode { get; set; }
        public int JobGroupId { get; set; }
        public string JobGroup { get; set; }
        public string LastPromodate { get; set; }
        public decimal CurrentSalary { get; set; }
        public string LastIncDate { get; set; }
        public string EmpLocation { get; set; }
        public int CountryId { get; set; }
        public int RegionId { get; set; }
        public string EmpImgPath { get; set; }
        [JsonProperty("CSVManagerId")]
        public string CSVManagerId { get; set; }
        public int Ord { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class EmployeeExcelModel
    {
        public DataTable TblEmployee { get; set; }
        public int Result { get; set; }
    }

    public class UserExcelModel
    {
        public DataTable TblUserLogin { get; set; }
    }

    public class AuthUserExcelModel
    {
        public DataTable TblAspNetUsers { get; set; }
    }

    public class AuthUserClaimExcelModel
    {
        public DataTable TblAuthClaims { get; set; }
    }
   
}
