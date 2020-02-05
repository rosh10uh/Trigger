using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   DimensionElementsModel
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   10 June 2019
    /// Purpose      :   DTO class for Dimension Elements(Values for Dimension Types)
    /// Revision     : 
    /// </summary>
    public class DimensionElementsModel
    {
        public int id { get; set; }//Dimension Elements Id

        [Required(ErrorMessage = ValidationMessage.DimensionTypeRequired)]
        public int dimensionId { get; set; }//Dimension Id

        public string dimensionType { get; set; }

        [RegularExpression(RegxExpression.DimensionRegularExpression, ErrorMessage = ValidationMessage.DimensionElementsRegExpression)]
        [Required(ErrorMessage = ValidationMessage.DimensionValueRequired)]
        [MaxLength(50, ErrorMessage = ValidationMessage.DimensionValueMaxLength)] 
        public string dimensionValues { get; set; }

        public int dimensionValueid { get; set; }//Dimension Elements Value Id

        public bool bActive { get; set; }

        public bool isDefault { get; set; }

        public int createdBy { get; set; }

        public int updatedBy { get; set; }

        public int result { get; set; }
    }

    public class DimensionElementsListModel
    {
        public int dimensionId { get; set; }
        public string dimensionType { get; set; }
        public List<DimensionElementsModel> dimensionValues { get; set; }
    }
}
