using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO
{
    public class ContactDetails
    {

        [Required(ErrorMessage = ValidationMessage.FullNameRequired)]
        public string fullName { get; set; }
        [Required(ErrorMessage = ValidationMessage.EmailRequired)]
        [RegularExpression(RegxExpression.Email,ErrorMessage =ValidationMessage.EmailValid)]
        public string email { get; set; }
        [Required(ErrorMessage = ValidationMessage.SubjectRequired)]
        public string subject { get; set; }
        [Required(ErrorMessage = ValidationMessage.CommentsRequired)]
        public string comments { get; set; }
    }

    public class Subscriber
    {
        public string email { get; set; }
        public int result { get; set; }
    }
}
