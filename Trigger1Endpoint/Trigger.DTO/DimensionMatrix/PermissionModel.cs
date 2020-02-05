using System.ComponentModel.DataAnnotations;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   PermissionModel
    /// Author       :   Bhumika Bhavsar
    /// Creation Date:   12 June 2019
    /// Purpose      :   DTO class for getting permission of logged in user
    /// Revision     : 
    /// </summary>
    public class PermissionModel
    {
        public int managerId { get; set; }
    }

}
