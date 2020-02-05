using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trigger.DTO.DimensionMatrix;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO
{
    public class UserLoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string deviceType { get; set; }
        public string deviceID { get; set; }
        public int userId { get; set; }
        public int result { set; get; }
    }

    public class UserDataModel
    {
        public int userId { get; set; }
        public int empId { get; set; }
        public int roleId { get; set; }
        public int companyid { get; set; }
        public string userName { get; set; }
        public string companyname { get; set; }
        public string role { get; set; }
        public string empEmailId { get; set; }
        public string Message { get; set; }
        public string dbConnection { get; set; }
        public EmployeeModel employee { get; set; }
        public List<ActionList> permission { get; set; }

        public UserDataModel()
        {
            employee = new EmployeeModel();
            permission = new List<ActionList>();
        }

        public string key { get; set; }
        public string result { set; get; }
        public string Error { set; get; }

    }

    public class UserChangePassword
    {
        [RequiredInt(ValidationMessage.UserIdRequire)]
        public int userId { get; set; }
        public string userName { get; set; }
        [Required(ErrorMessage = ValidationMessage.CurrentPasswordRequired)]
        public string oldPassword { get; set; }

        [Required(ErrorMessage = ValidationMessage.NewPasswordRequired)]
        [MinLength(10, ErrorMessage = ValidationMessage.PasswordMinLength)]
        [MaxLength(15, ErrorMessage = ValidationMessage.PasswordMaxLength)]
        [RegularExpression(RegxExpression.Password, ErrorMessage = ValidationMessage.NewPasswordValid)]
        public string newPassword { get; set; }

        public int updatedBy { get; set; }
        public int result { get; set; }
    }

    public class CompanyDB
    {
        public string serverName { get; set; }
        public string dbName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
    }

    public class ResetPassword
    {
        public int userId { get; set; }
        public string password { get; set; }
        public int updatedBy { get; set; }
        public bool result { get; set; }
    }
}
