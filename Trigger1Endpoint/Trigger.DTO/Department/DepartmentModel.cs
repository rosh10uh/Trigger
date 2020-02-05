using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO
{
    public class DepartmentModel
    {
        [RequiredInt(ValidationMessage.CompanyRequired)]
        public int companyId { get; set; }
        public int departmentId { get; set; }

        [Required(ErrorMessage = ValidationMessage.DepartmentNameRequired)]
        [MaxLength(25, ErrorMessage = ValidationMessage.DepartmentNameMaxLength)]
        [RegularExpression(RegxExpression.AlphabaticWithSpace, ErrorMessage = ValidationMessage.DepartmentNameValid)]
        public string department { get; set; }
        public bool isActive { get; set; }
        public int createdBy { get; set; }
        public string updatedBy { get; set; }
        public int result { get; set; }
        public string yearId { get; set; }
    }
    public class CompanyWiseDepartmentModel
    {
        public int companyId { get; set; }
        public int departmentId { get; set; }
        public int mstDepartmentId { get; set; }
        public int isSelected { get; set; }
        public string department { get; set; }
        public int createdBy { get; set; }
        public string updatedBy { get; set; }
        public string yearID { get; set; }
    }
    public class CompWiseDepartmentModel
    {
        public int? companyId { get; set; }
        public int? departmentId { get; set; }
        public string department { get; set; }
        public int? createdBy { get; set; }
        public int? result { get; set; }
    }
}
