using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   DimensionModel
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   07 June 2019
    /// Purpose      :   DTO class for Dimension Master
    /// Revision     : 
    /// </summary>
    public class DimensionModel
    {
        public int id { get; set; }//Dimension Id

        [RegularExpression(RegxExpression.DimensionRegularExpression, ErrorMessage = ValidationMessage.DimensionElementsRegExpression)]
        [Required(ErrorMessage = ValidationMessage.DimensionTypeRequired)]
        [MaxLength(50, ErrorMessage = ValidationMessage.DimensionTypeMaxLength)] 
        public string dimensionType { get; set; }

        public bool bActive { get; set; }

        public int createdBy { get; set; }

        public int updatedBy { get; set; }

        public int result { get; set; }
    }
}
