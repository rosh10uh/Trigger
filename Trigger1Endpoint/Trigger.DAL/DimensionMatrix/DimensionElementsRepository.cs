using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO.DimensionMatrix;

namespace Trigger.DAL.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   DimensionElementsRepository
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   10 June 2019
    /// Purpose      :   Repository for CRUD Operation for Dimensionwise Valus
    /// Revision     : 
    /// </summary>
    [QueryPath("Trigger.DAL.Query.DimensionMatrix.DimensionElements")]
    public class DimensionElementsRepository : DaoRepository<DimensionElementsModel>
    {
        
    }
}
