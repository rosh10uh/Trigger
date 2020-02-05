using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO.DimensionMatrix;

namespace Trigger.DAL.DimensionMatrix
{
    /// <summary>
    /// Class Name   :   DimensionRepository
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   07 June 2019
    /// Purpose      :   Repository for CRUD Operation for Dimension
    /// Revision     : 
    /// </summary>
    [QueryPath("Trigger.DAL.Query.DimensionMatrix.Dimension")]
    public class DimensionRepository : DaoRepository<DimensionModel>
    {

    }
}
