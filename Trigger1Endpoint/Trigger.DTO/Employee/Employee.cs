using System;
using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO
{
    public class EmployeeModel
    {
        public int dimensionType { get; set; }

        public int dimensionValues { get; set; }

        public int empId { get; set; }
        [RegularExpression(RegxExpression.EmployeeId, ErrorMessage = ValidationMessage.EmployeeIdValid)]
        public string employeeId { get; set; }

        [RequiredInt(ValidationMessage.CompanyRequired)]
        public int companyid { get; set; }

        public string companyName { get; set; }

        [Required(ErrorMessage = ValidationMessage.FirstNameRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.FirstNameMaxLength)]
        [RegularExpression(RegxExpression.EmployeeFName, ErrorMessage = ValidationMessage.FirstNameValid)]
        public string firstName { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.MiddleNameMaxLength)]
        [RegularExpression(RegxExpression.EmployeeName, ErrorMessage = ValidationMessage.MiddleNameValid)]
        public string middleName { get; set; }

        [Required(ErrorMessage = ValidationMessage.LastNameRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.LastNameMaxLength)]
        [RegularExpression(RegxExpression.EmployeeLName, ErrorMessage = ValidationMessage.LastNamevalid)]
        public string lastName { get; set; }

        public string suffix { get; set; }


        [MaxLength(60, ErrorMessage = ValidationMessage.EmailMaxLength)]
        public string email { get; set; }

        [Required(ErrorMessage = ValidationMessage.PositionRequired)]
        [MaxLength(50, ErrorMessage = ValidationMessage.PositionMaxLenght)]
        [RegularExpression(RegxExpression.Position, ErrorMessage = ValidationMessage.PositionValid)]
        public string jobTitle { get; set; }

        [RequiredDate(ValidationMessage.DateOfHireRequired)]
        [DateCompareValidation(ValidationResource.dateOfBirth, enumOprator.greaterThan, ValidationMessage.BirthDateHireDateValidation)]
        [DateCompareValidation(ValidationResource.lastIncDate, enumOprator.lessThan, ValidationMessage.HireDateIncDateValidation)]
        [DateCompareValidation(ValidationResource.lastPromodate, enumOprator.lessThan, ValidationMessage.HireDatePromDateValidation)]
        [DateValidationOnCurentDate(enumOprator.lessThanOrEqual, ValidationResource.empId, ValidationMessage.DateOfHireValidation)]
        public DateTime joiningDate { get; set; }

        [Required(ErrorMessage = ValidationMessage.CityRequired)]
        [MaxLength(30, ErrorMessage = ValidationMessage.EmployeeCityMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.CityNameValid)]
        public string workCity { get; set; }

        [Required(ErrorMessage = ValidationMessage.StateRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.StateMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.StateNameValid)]
        public string workState { get; set; }

        [Required(ErrorMessage = ValidationMessage.ZipCodeRequired)]
        [RegularExpression(RegxExpression.ZipCode, ErrorMessage = ValidationMessage.ZipCodeValid)] //need to check
        [MinLength(5, ErrorMessage = ValidationMessage.ZipCodeMinLength)]
        [MaxLength(15, ErrorMessage = ValidationMessage.ZipCodeMaxLength)]
        public string workZipcode { get; set; }

        [RequiredInt(ValidationMessage.DepartmentRequired)]
        public int departmentId { get; set; }

        public string department { get; set; }
        public int managerId { get; set; }
        public string managerFName { get; set; }
        public string managerLName { get; set; }

        [Required(ErrorMessage = ValidationMessage.EmployeeStateRequired)]
        public bool empStatus { get; set; }

        [RequiredInt(ValidationMessage.RoleRequired)]
        //[Range(1, 6, ErrorMessage = ValidationMessage.RoleRequired)]
        public int roleId { get; set; }
        public string role { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = RegxExpression.DateFormat)]
        [DateCompareValidation(ValidationResource.lastPromodate, enumOprator.lessThan, ValidationMessage.BirthDatePromDateValidation)]
        [DateCompareValidation(ValidationResource.lastIncDate, enumOprator.lessThan, ValidationMessage.BirthDateIncDateValidation)]
        public DateTime dateOfBirth { get; set; }

        public int raceorethanicityId { get; set; }
        public string raceorethanicity { get; set; }
        public string gender { get; set; }
        public int jobCategoryId { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.JobcategoryMaxLength)]
        public string jobCategory { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.LocationNameMaxLength)]
        [RegularExpression(RegxExpression.AlphabaticWithSpace, ErrorMessage = ValidationMessage.LocationNameValid)]
        public string empLocation { get; set; }

        public int jobCodeId { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.JobCodeMaxLength)]
        public string jobCode { get; set; }

        public int jobGroupId { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.JobGroupMaxLength)]
        public string jobGroup { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = RegxExpression.DateFormat)]
        [DateCompareValidation(ValidationResource.lastIncDate, enumOprator.lessThanOrEqual, ValidationMessage.IncDatePromDateValidation)]
        public DateTime lastPromodate { get; set; }

        [RegularExpression(RegxExpression.Salary, ErrorMessage = ValidationMessage.CurrentSalaryMaxValue)]
        public decimal currentSalary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = RegxExpression.DateFormat)]
        [DateCompareValidation(ValidationResource.dateOfBirth, enumOprator.greaterThan, ValidationMessage.BirthDateIncDateValidation)]
        public DateTime lastIncDate { get; set; }

        public string country { get; set; }
        public int countryId { get; set; }

        [RegionValidation]
        public int regionId { get; set; }

        public string region { get; set; }
        public string empImgPath { get; set; }
        public string empFolderPath { get; set; }
        public bool bactive { get; set; }
        public int createdBy { get; set; }
        public int updatedBy { get; set; }
        public string empImage { get; set; }
        public string lastAssessedDate { get; set; }
        public string ManagerLastAssessedDate { get; set; }
        public string lastAssessmentDate { get; set; }
        public int avgTriggerScore { get; set; }
        public string avgScoreRank { get; set; }
        public string lastScoreRank { get; set; }
        public string ratingCompleted { get; set; }
        public string companyLogoPath { get; set; }
        public bool isMailSent { get; set; }

        [RegularExpression(RegxExpression.PhoneNumber, ErrorMessage = ValidationMessage.PhoneNumberValid)]
        public string phoneNumber { get; set; }

        public string empIdList { get; set; }
        public DateTime companyContractStartDate { get; set; }
        public DateTime companyContractEndDate { get; set; }

        public int resultId { set; get; }
        public int result { set; get; }
        public string password { get; set; }
        public int totalRowCount { get; set; }
        public int yearId { get; set; }

        public int pagenumber { get; set; }
        public int pagesize { get; set; }
        public string searchstring { get; set; }
        public string departmentlist { get; set; }
        public string grade { get; set; }
        public string month { get; set; }
        public int userId { get; set; }
        public int empRelation { get; set; }
        public int TeamType { get; set; }

        public enum Emprole
        {
            TriggerAdmin = 1,
            Admin = 2,
            Manager = 3,
            Executive = 4,
            NonManager = 5
        }
    }

    public class CountryRegion
    {
        public string country { get; set; }
        public int countryId { get; set; }
        public int regionId { get; set; }
        public string region { get; set; }
    }

    public class EmpProfile
    {
        public int empId { get; set; }
        public int companyId { get; set; }
        public string empImgPath { get; set; }
        public string empFolderPath { get; set; }
        public string empImage { get; set; }
        public int updatedBy { get; set; }
        public int result { set; get; }
    }

    public class EmployeeSalary
    {
        public int empId { get; set; }
        public int companyId { get; set; }
        public decimal currentSalary { get; set; }
        public int updatedBy { get; set; }
        public int result { set; get; }
    }
    public class EmployeeProfile
    {
        [RequiredInt(ValidationMessage.EmpIdRequired)]
        public int empId { get; set; }

        public string employeeId { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }

        [Required(ErrorMessage = ValidationMessage.CityRequired)]
        [MaxLength(30, ErrorMessage = ValidationMessage.EmployeeCityMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.CityNameValid)]
        public string workCity { get; set; }

        [Required(ErrorMessage = ValidationMessage.StateRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.StateMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.StateNameValid)]
        public string workState { get; set; }

        [Required(ErrorMessage = ValidationMessage.ZipCodeRequired)]
        [RegularExpression(RegxExpression.ZipCode, ErrorMessage = ValidationMessage.ZipCodeValid)]
        [MinLength(5, ErrorMessage = ValidationMessage.ZipCodeMinLength)]
        [MaxLength(15, ErrorMessage = ValidationMessage.ZipCodeMaxLength)]
        public string workZipcode { get; set; }
        public string empImgPath { get; set; }

        [RegularExpression(RegxExpression.PhoneNumber, ErrorMessage = ValidationMessage.PhoneNumberValid)]
        public string phoneNumber { get; set; }
        public bool phoneConfirmed { get; set; }
        public bool optForSms { get; set; }
        public int updatedBy { get; set; }
        public int result { set; get; }
    }

    public class EmployeeSMSNotification
    {
        public int empId { get; set; }
        public bool optForSms { get; set; }
        public int updatedBy { get; set; }
        public int result { set; get; }
    }
    public class EmpProfilePic
    {
        public string empImgPath { get; set; }
        public string empFolderPath { get; set; }
        public string empImage { get; set; }
    }

    public class CSVEmployeeDetailsModel
    {
        public int companyId { get; set; }
        public int managerEmpId { get; set; }
        public string managerId { get; set; }
        public string managerName { get; set; }
    }

    public class EmployeeListModel 
    {
        public int empId { get; set; }
        [RegularExpression(RegxExpression.EmployeeId, ErrorMessage = ValidationMessage.EmployeeIdValid)]
        public string employeeId { get; set; }

        [RequiredInt(ValidationMessage.CompanyRequired)]
        public int companyId { get; set; }

        public string companyName { get; set; }

        [Required(ErrorMessage = ValidationMessage.FirstNameRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.FirstNameMaxLength)]
        [RegularExpression(RegxExpression.EmployeeFName, ErrorMessage = ValidationMessage.FirstNameValid)]
        public string firstName { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.MiddleNameMaxLength)]
        [RegularExpression(RegxExpression.EmployeeName, ErrorMessage = ValidationMessage.MiddleNameValid)]
        public string middleName { get; set; }

        [Required(ErrorMessage = ValidationMessage.LastNameRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.LastNameMaxLength)]
        [RegularExpression(RegxExpression.EmployeeLName, ErrorMessage = ValidationMessage.LastNamevalid)]
        public string lastName { get; set; }

        [MaxLength(60, ErrorMessage = ValidationMessage.EmailMaxLength)]
        public string email { get; set; }

        [RequiredInt(ValidationMessage.DepartmentRequired)]
        public int departmentId { get; set; }

        public string department { get; set; }
        public int managerId { get; set; }

        [Required(ErrorMessage = ValidationMessage.EmployeeStateRequired)]
        public bool empStatus { get; set; }

        [RequiredInt(ValidationMessage.RoleRequired)]
        [Range(1, 6, ErrorMessage = ValidationMessage.RoleRequired)]
        public int roleId { get; set; }
        public string lastAssessedDate { get; set; }
        public string ManagerLastAssessedDate { get; set; }
        public string ratingCompleted { get; set; }
        
        public int empRelation { get; set; }
        public int TeamType { get; set; }

    }
       
       
}