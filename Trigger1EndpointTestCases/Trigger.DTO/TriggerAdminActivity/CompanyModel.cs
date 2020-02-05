using System;
using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO
{
    public class CompanyDetailsModel
    {
        public int compId { get; set; }

        public string companyId { get; set; }

        public int OrganizationTypeId { get; set; }

        [RequiredInt(ValidationMessage.CompanyTypeRequired)]
        public int industryTypeId { get; set; }

        public string industryType { get; set; }

        [Required(ErrorMessage = ValidationMessage.CompanyNameRequired)]
        [MaxLength(100, ErrorMessage = ValidationMessage.CompanyNameMaxLength)]
        [RegularExpression(RegxExpression.CompanyName, ErrorMessage = ValidationMessage.CompanyNameValid)]
        public string companyName { get; set; }

        [Required(ErrorMessage = ValidationMessage.Address1Required)]
        [MaxLength(100, ErrorMessage = ValidationMessage.Address1MaxLength)]
        [RegularExpression(RegxExpression.Address, ErrorMessage = ValidationMessage.Address1Valid)]
        public string address1 { get; set; }

        [MaxLength(100, ErrorMessage = ValidationMessage.Address2MaxLength)]
        [RegularExpression(RegxExpression.Address, ErrorMessage = ValidationMessage.Address2Valid)]
        public string address2 { get; set; }

        [Required(ErrorMessage = ValidationMessage.CityRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.CityMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.CityNameValid)]
        public string city { get; set; }

        [Required(ErrorMessage = ValidationMessage.StateRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.StateMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.StateNameValid)]
        public string state { get; set; }

        [Required(ErrorMessage = ValidationMessage.ZipCodeRequired)]
        [RegularExpression(RegxExpression.ZipCode, ErrorMessage = ValidationMessage.ZipCodeValid)] //need to check
        [MinLength(5, ErrorMessage = ValidationMessage.ZipCodeMinLength)]
        [MaxLength(15, ErrorMessage = ValidationMessage.ZipCodeMaxLength)]
        public string zipcode { get; set; }

        [Required(ErrorMessage = ValidationMessage.CountryRequired)]
        [MaxLength(30, ErrorMessage = ValidationMessage.CountryMaxLength)]
        [RegularExpression(RegxExpression.CityStateCountry, ErrorMessage = ValidationMessage.CountryValid)]
        public string country { get; set; }

        [Required(ErrorMessage = ValidationMessage.PhoneRequired)]
        [RegularExpression(RegxExpression.PhoneNumberClient, ErrorMessage = ValidationMessage.PhoneNumberValid)]
        public Int64 phoneNo1 { get; set; }

        public Int64 phoneNo2 { get; set; }

        [MaxLength(50, ErrorMessage = ValidationMessage.WebsiteMaxLength)]
        //[RegularExpression(RegxExpression.Website, ErrorMessage = ValidationMessage.WebsiteValid)]
        public string website { get; set; }

        public string keyEmpName { get; set; }

        public string keyEmpEmail { get; set; }

        [MaxLength(300, ErrorMessage = ValidationMessage.RemarkMaxLength)]
        public string remarks { get; set; }

        [Range(0, 99999, ErrorMessage = ValidationMessage.CostPerEmpMaxValue)]
        public decimal costPerEmp { get; set; }

        [Range(0, 99999, ErrorMessage = ValidationMessage.FixedAmtPerMonMaxValue)]
        public decimal fixedAmtPerMon { get; set; }

        [MaxLength(300, ErrorMessage = ValidationMessage.DealsDetailsMaxLength)]
        public string dealsRemarks { get; set; }

        public string compImgPath { get; set; }

        public string compFolderPath { get; set; }

        public string compImage { get; set; }

        [RequiredDate(ValidationMessage.ContractStartDateRequired)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = RegxExpression.DateFormat)]
        [DateCompareValidation(ValidationResource.contractEndDate, enumOprator.lessThan, ValidationMessage.ContractStartDateLessThanEndDate)]
        [DateValidationOnCurentDate(enumOprator.greaterThanOrEqual, ValidationResource.compId, ValidationMessage.ContractStartDateNotPastDate)]
        public DateTime contractStartDate { get; set; }

        [RequiredDate(ValidationMessage.ContractEndDateRequired)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = RegxExpression.DateFormat)]
        public DateTime contractEndDate { get; set; }

        //[RequiredInt(ValidationMessage.GracePeriodRequired)]
        [Range(0, 365, ErrorMessage = ValidationMessage.GracePeriodMaxValue)]
        public int gracePeriod { get; set; }
        public int inActivityDays { get; set; }
        public int reminderDays { get; set; }
        public bool isActive { get; set; }

        public int createdBy { get; set; }

        public int updatedBy { get; set; }

        public int result { set; get; }
    }

    public class CompLogoModel
    {
        public int companyId { get; set; }
        public string compImgPath { get; set; }
        public string compFolderPath { get; set; }
        public string compImage { get; set; }
        public int updatedBy { get; set; }
    }

    public class CompanyConfigModel
    {
        public int companyId { get; set; }
        public string companyDomain { get; set; }
        public string serverName { get; set; }
        public string dbName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public int createdBy { get; set; }
        public int updatedBy { get; set; }
        public int result { set; get; }
    }
}
