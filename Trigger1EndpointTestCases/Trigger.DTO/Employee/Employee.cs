using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO
{
    public class EmployeeModel
    {
        public int EmpId { get; set; }
        [RegularExpression(RegxExpression.EmployeeId, ErrorMessage = ValidationMessage.EmployeeIdValid)]
        public string EmployeeId { get; set; }

        [JsonProperty("companyid")]
        [RequiredInt(ValidationMessage.CompanyRequired)]
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        [Required(ErrorMessage = ValidationMessage.FirstNameRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.FirstNameMaxLength)]
<<<<<<< HEAD
        [RegularExpression(RegxExpression.EmployeeFName, ErrorMessage = ValidationMessage.FirstNameValid)]
        public string firstName { get; set; }
=======
        [RegularExpression(RegxExpression.EmployeeName, ErrorMessage = ValidationMessage.FirstNameValid)]
        public string FirstName { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        [MaxLength(25, ErrorMessage = ValidationMessage.MiddleNameMaxLength)]
        [RegularExpression(RegxExpression.EmployeeName, ErrorMessage = ValidationMessage.MiddleNameValid)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = ValidationMessage.LastNameRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.LastNameMaxLength)]
<<<<<<< HEAD
        [RegularExpression(RegxExpression.EmployeeLName, ErrorMessage = ValidationMessage.LastNamevalid)]
        public string lastName { get; set; }

        public string suffix { get; set; }
=======
        [RegularExpression(RegxExpression.EmployeeName, ErrorMessage = ValidationMessage.LastNamevalid)]
        public string LastName { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public string Suffix { get; set; }

        [MaxLength(60, ErrorMessage = ValidationMessage.EmailMaxLength)]
        public string Email { get; set; }

        [Required(ErrorMessage = ValidationMessage.PositionRequired)]
        [MaxLength(50, ErrorMessage = ValidationMessage.PositionMaxLenght)]
        [RegularExpression(RegxExpression.Position, ErrorMessage = ValidationMessage.PositionValid)]
        public string JobTitle { get; set; }

        [RequiredDate(ValidationMessage.DateOfHireRequired)]
        [DateCompareValidation(nameof(DateOfBirth), enumOprator.greaterThan, ValidationMessage.BirthDateHireDateValidation)]
        [DateCompareValidation(nameof(LastIncDate), enumOprator.lessThan, ValidationMessage.HireDateIncDateValidation)]
        [DateCompareValidation(nameof(LastPromoDate), enumOprator.lessThan, ValidationMessage.HireDatePromDateValidation)]
        [DateValidationOnCurentDate(enumOprator.lessThanOrEqual, nameof(EmpId), ValidationMessage.DateOfHireValidation)]
        public DateTime JoiningDate { get; set; }

        [Required(ErrorMessage = ValidationMessage.CityRequired)]
        [MaxLength(30, ErrorMessage = ValidationMessage.EmployeeCityMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.CityNameValid)]
        public string WorkCity { get; set; }

        [Required(ErrorMessage = ValidationMessage.StateRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.StateMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.StateNameValid)]
        public string WorkState { get; set; }

        [JsonProperty("workZipcode")]
        [Required(ErrorMessage = ValidationMessage.ZipCodeRequired)]
        [RegularExpression(RegxExpression.ZipCode, ErrorMessage = ValidationMessage.ZipCodeValid)] //need to check
        [MinLength(5, ErrorMessage = ValidationMessage.ZipCodeMinLength)]
        [MaxLength(15, ErrorMessage = ValidationMessage.ZipCodeMaxLength)]
        public string WorkZipCode { get; set; }

        [RequiredInt(ValidationMessage.DepartmentRequired)]
        public int DepartmentId { get; set; }

        public string Department { get; set; }
        public int ManagerId { get; set; }
        public string ManagerFName { get; set; }
        public string ManagerLName { get; set; }

        [Required(ErrorMessage = ValidationMessage.EmployeeStateRequired)]
        public bool EmpStatus { get; set; }

        [RequiredInt(ValidationMessage.RoleRequired)]
        //[Range(1, 6, ErrorMessage = ValidationMessage.RoleRequired)]
        public int RoleId { get; set; }
        public string Role { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = RegxExpression.DateFormat)]
        [DateCompareValidation(nameof(LastPromoDate), enumOprator.lessThan, ValidationMessage.BirthDatePromDateValidation)]
        [DateCompareValidation(nameof(LastIncDate), enumOprator.lessThan, ValidationMessage.BirthDateIncDateValidation)]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("raceorethanicityId")]
        public int RaceOrEthaniCityId { get; set; }

        [JsonProperty("raceorethanicity")]
        public string RaceOrEthaniCity { get; set; }
        public string Gender { get; set; }
        public int JobCategoryId { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.JobcategoryMaxLength)]
        public string JobCategory { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.LocationNameMaxLength)]
        [RegularExpression(RegxExpression.AlphabaticWithSpace, ErrorMessage = ValidationMessage.LocationNameValid)]
        public string EmpLocation { get; set; }

        public int JobCodeId { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.JobCodeMaxLength)]
        public string JobCode { get; set; }

        public int JobGroupId { get; set; }

        [MaxLength(25, ErrorMessage = ValidationMessage.JobGroupMaxLength)]
        public string JobGroup { get; set; }

        [JsonProperty("lastPromodate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = RegxExpression.DateFormat)]
        [DateCompareValidation(nameof(LastIncDate), enumOprator.lessThanOrEqual, ValidationMessage.IncDatePromDateValidation)]
        public DateTime LastPromoDate { get; set; }

        [RegularExpression(RegxExpression.Salary, ErrorMessage = ValidationMessage.CurrentSalaryMaxValue)]
        public decimal CurrentSalary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = RegxExpression.DateFormat)]
        [DateCompareValidation(nameof(DateOfBirth), enumOprator.greaterThan, ValidationMessage.BirthDateIncDateValidation)]
        public DateTime LastIncDate { get; set; }

        public string Country { get; set; }
        public int CountryId { get; set; }

        [RegionValidation(nameof(CountryId))]
        public int RegionId { get; set; }

        public string Region { get; set; }
        public string EmpImgPath { get; set; }
        public string EmpFolderPath { get; set; }

        [JsonProperty("bactive")]
        public bool BActive { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string EmpImage { get; set; }
        public string LastAssessedDate { get; set; }
        public string LastAssessmentDate { get; set; }
        public int AvgTriggerScore { get; set; }
        public string AvgScoreRank { get; set; }
        public string LastScoreRank { get; set; }
        public string RatingCompleted { get; set; }
        public string CompanyLogoPath { get; set; }
        public bool IsMailSent { get; set; }

        [RegularExpression(RegxExpression.PhoneNumber, ErrorMessage = ValidationMessage.PhoneNumberValid)]
<<<<<<< HEAD
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
=======
        public string PhoneNumber { get; set; }

        public string EmpIdList { get; set; }
        public DateTime CompanyContractStartDate { get; set; }
        public DateTime CompanyContractEndDate { get; set; }

        public int ResultId { set; get; }
        public int Result { set; get; }
        public string Password { get; set; }
        public int TotalRowCount { get; set; }
        public int YearId { get; set; }

        [JsonProperty("pagenumber")]
        public int PageNumber { get; set; }

        [JsonProperty("pagesize")]
        public int PageSize { get; set; }

        [JsonProperty("searchstring")]
        public string SearchString { get; set; }

        [JsonProperty("departmentlist")]
        public string DepartmentList { get; set; }
        public string Grade { get; set; }
        public string Month { get; set; }
        public int UserId { get; set; }
        public int EmpRelation { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

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
        public string Country { get; set; }
        public int CountryId { get; set; }
        public int RegionId { get; set; }
        public string Region { get; set; }
    }

    public class EmployeeProfilePicture : EmployeeImage
    {
        public int EmpId { get; set; }
        public int CompanyId { get; set; }
        public int UpdatedBy { get; set; }
        public int Result { set; get; }
    }

    public class EmployeeSalary
    {
        public int EmpId { get; set; }
        public int CompanyId { get; set; }
        public decimal CurrentSalary { get; set; }
        public int UpdatedBy { get; set; }
        public int Result { set; get; }
    }
    public class EmployeeProfileModel
    {
        [RequiredInt(ValidationMessage.EmpIdRequired)]
        public int EmpId { get; set; }

        public string EmployeeId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = ValidationMessage.CityRequired)]
        [MaxLength(30, ErrorMessage = ValidationMessage.EmployeeCityMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.CityNameValid)]
        public string WorkCity { get; set; }

        [Required(ErrorMessage = ValidationMessage.StateRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.StateMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.StateNameValid)]
        public string WorkState { get; set; }

        [Required(ErrorMessage = ValidationMessage.ZipCodeRequired)]
        [RegularExpression(RegxExpression.ZipCode, ErrorMessage = ValidationMessage.ZipCodeValid)]
        [MinLength(5, ErrorMessage = ValidationMessage.ZipCodeMinLength)]
        [MaxLength(15, ErrorMessage = ValidationMessage.ZipCodeMaxLength)]
        public string WorkZipcode { get; set; }
        public string EmpImgPath { get; set; }

        [RegularExpression(RegxExpression.PhoneNumber, ErrorMessage = ValidationMessage.PhoneNumberValid)]
        public string PhoneNumber { get; set; }
        public bool PhoneConfirmed { get; set; }
        public bool OptForSms { get; set; }
        public int UpdatedBy { get; set; }
        public int Result { set; get; }
    }

    public class EmployeeSmsNotification
    {
        public int EmpId { get; set; }
        public bool OptForSms { get; set; }
        public int UpdatedBy { get; set; }
        public int Result { set; get; }
    }
    public class EmployeeImage
    {
        public string EmpImgPath { get; set; }
        public string EmpFolderPath { get; set; }
        public string EmpImage { get; set; }
    }

    public class ExcelEmployeeDetailsModel
    {
        public int CompanyId { get; set; }
        public int ManagerEmpId { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
    }

    public class EmployeeListModel
    {
<<<<<<< HEAD
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
        public string ratingCompleted { get; set; }
        
        public int empRelation { get; set; }
        public int TeamType { get; set; }

=======
        public int EmpId { get; set; }
        public string EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public string department { get; set; }
        public int ManagerId { get; set; }
        public bool EmpStatus { get; set; }
        public int RoleId { get; set; }
        public string LastAssessedDate { get; set; }
        public string RatingCompleted { get; set; }
        public int EmpRelation { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
    }
       
       
}