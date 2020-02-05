using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO.TeamConfiguration
{
    public class TeamConfigurationModel
    {
        public int TeamId { get; set; }

        [Required(ErrorMessage = ValidationMessage.TeamNameRequired)]
        [RegularExpression(RegxExpression.TeamName, ErrorMessage = ValidationMessage.TeamNameRequired)]
        public string Name { get; set; }

        public string Description { get; set; }

        [RequiredDate(ValidationMessage.TeamStartDateRequired)]
        [DateCompareValidation(ValidationResource.TeamEndDate, enumOprator.lessThan, ValidationMessage.TeamStartDateLessThanEndDate)]
        //[DateValidationOnCurentDate(enumOprator.greaterThanOrEqual, ValidationResource.TeamId, ValidationMessage.TeamStartDateNotPastDate)]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [AllowPositiveNumbersAttribute(ValidationMessage.InValidTriggerActivityDays)]
        public int TriggerActivityDays { get; set; }
        public bool Status { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }

        public string CreatedByFName { get; set; }
        public string CreatedByLName { get; set; }

        public string Managers { get; set; }
        public string ManagerIds { get; set; }
        public int Result { get; set; }

        public List<TeamManagerModel> TeamManagers { get; set; }
        public List<TeamEmployeesModel> TeamEmployees { get; set; }

        public TeamConfigurationModel()
        {
            TeamManagers = new List<TeamManagerModel>();
            TeamEmployees = new List<TeamEmployeesModel>();
        }
    }

    public class TeamManagerModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int ManagerId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int Result { get; set; }
    }

    public class TeamEmployeesModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int EmpId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int Result { get; set; }
    }

    public class TeamStatusInActive
    {
        public int TeamId { get; set; }
        public int CreatedBy { get; set; }
        public int Result { get; set; }
    }

    public class TeamInActivityLogModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string EmailTo { get; set; }
        public string EmailText { get; set; }
        public int TriggerActivitydays { get; set; }
        public int CreatedBy { get; set; }
        public int Result { get; set; }
    }

}
