using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Trigger.DTO
{
    public class CsvEmployeesModel
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
        //  public string recordtype { get; set; }
        [JsonProperty("CSVManagerId")]
        public string CSVManagerId { get; set; }
        public string phonenumber { get; set; }
    }

       public class CsvEmp
    {
        public int companyId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string suffix { get; set; }
        public string email { get; set; }
        public string jobTitle { get; set; }
        public DateTime joiningDate { get; set; }
        public string workCity { get; set; }
        public string workState { get; set; }
        public string workZipcode { get; set; }
        public int departmentId { get; set; }
        public string department { get; set; }
        public int managerId { get; set; }
        public string managerName { get; set; }
        public string managerLName { get; set; }
        public bool empStatus { get; set; }
        public int roleId { get; set; }
        public DateTime dateOfBirth { get; set; }
        public int raceorethanicityId { get; set; }
        public string gender { get; set; }
        public int jobCategoryId { get; set; }
        public int jobCodeId { get; set; }
        public int jobGroupId { get; set; }
        public DateTime lastPromodate { get; set; }
        public decimal currentSalary { get; set; }
        public DateTime lastIncDate { get; set; }
        public string empLocation { get; set; }
        public int countryId { get; set; }
        public int regionId { get; set; }
        public string empImgPath { get; set; }
        public bool bactive { get; set; }
        public int createdBy { get; set; }
        public DateTime createddtstamp { get; set; }
        public int updatedBy { get; set; }
        public DateTime updateddtstamp { get; set; }


    }

    public class CountRecordModel
    {
	    public string companyId { get; set; }
		public int newlyInserted { get; set; }
        public int mismatchRecord { get; set; }
		public List<CsvEmployeesModel> lstNewCsvUpload { get; set; }
        public List<CsvEmployeesModel> lstMisMatchCsvUpload { get; set; }
        public List<CsvEmployeesModel> lstExistPhoneCsvUpload { get; set; }

        public CountRecordModel()
        {
            lstNewCsvUpload = new List<CsvEmployeesModel>();
            lstMisMatchCsvUpload = new List<CsvEmployeesModel>();
            lstExistPhoneCsvUpload = new List<CsvEmployeesModel>();
        }
    }
    public class NewlyInserted
    {
        public List<CsvEmployeesModel> lstNewCsvUpload { get; set; }
    }
    public class MismatchData
    {
        public List<CsvEmployeesModel> lstMismatchUpload { get; set; }
    }

    public class CsvData
    {
        public int NewlyInserted { get; set; }
        public int MismatchRecord { get; set; }
        public string Source { get; set; }
        public int empid { get; set; }
        public string employeeid { get; set; }
        public string companyId { get; set; }
        public string firstname { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string suffix { get; set; }
        public string jobTitle { get; set; }
        public string joiningdate { get; set; }
        public string email { get; set; }
        public string workCity { get; set; }
        public string workState { get; set; }
        public string workZipcode { get; set; }
        public int departmentId { get; set; }
        public string department { get; set; }
        public string managerId { get; set; }
        public string managerName { get; set; }
        public string managerLName { get; set; }
        public int empStatus { get; set; }
        public int roleId { get; set; }
        public string dateOfBirth { get; set; }
        public int raceorethanicityId { get; set; }
        public string gender { get; set; }
        public string jobCategory { get; set; }
        public int jobCategoryId { get; set; }
        public int jobCodeId { get; set; }
        public string jobCode { get; set; }
        public int jobGroupId { get; set; }
        public string jobGroup { get; set; }
        public string lastPromodate { get; set; }
        public decimal currentSalary { get; set; }
        public string lastIncDate { get; set; }
        public string empLocation { get; set; }
        public int countryId { get; set; }
        public int regionId { get; set; }
        public string empImgPath { get; set; }
        public string CSVManagerId { get; set; }
        public int ord { get; set; }
        public string phonenumber { get; set; }
    }

    public class EmployeeCsvModel
    {
        public DataTable tblEmployee { get; set; }        
        public int result { get; set; }
    }

    public class UserCsvModel
	{
        public DataTable tblUserLogin { get; set; }
    }

    public class AuthUserCsvModel
    {
        public DataTable tblAspNetUsers { get; set; }
    }

    public class AuthUserClaimCsvModel
    {
        public DataTable tblAuthClaims { get; set; }
    }

    public class Manager
    {
        public int ManagerEmpId { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
    }

    public class CSVMasterData
    {
        public List<DTO.KeyValueClass> lstCountry { get; set; }
        public List<DTO.CountryRegion> lstRegions { get; set; }
        public List<DTO.KeyValueClass> lstJobCategory { get; set; }
        public List<DTO.KeyValueClass> lstJobGroup { get; set; }
        public List<DTO.KeyValueClass> lstJobCode { get; set; }
        public List<DTO.KeyValueClass> lstRaceorethnicity { get; set; }
        public List<DTO.DepartmentModel> lstDepartment { get; set; }
        public List<DTO.KeyValueClass> lstRole { get; set; }
        public List<DTO.Manager> lstManager { get; set; }

        public CSVMasterData()
        {
            lstCountry = new List<KeyValueClass>();
            lstRegions = new List<CountryRegion>();
            lstJobCategory = new List<KeyValueClass>();
            lstJobGroup = new List<KeyValueClass>();
            lstJobCode = new List<KeyValueClass>();
            lstRaceorethnicity = new List<KeyValueClass>();
            lstDepartment = new List<DepartmentModel>();
            lstRole = new List<KeyValueClass>();
            lstManager = new List<Manager>();
        }

        public class GetEmployeeCSV
        {
            public DataTable tblCountry{ get; set; }
            public DataTable tblRegion{ get; set; }
        }        
    }
}
