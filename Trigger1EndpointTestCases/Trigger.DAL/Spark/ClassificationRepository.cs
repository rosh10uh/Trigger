using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO.Spark;

namespace Trigger.DAL.Spark
{
    /// <summary>
    /// Class Name   :   ClassificationRepository
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   08 Aug 2019
    /// Purpose      :   Repository for classification master
    /// Revision     : 
    /// </summary>
    [QueryPath("Trigger.DAL.Query.Spark.Classification")]
    public class ClassificationRepository : DaoRepository<ClassificationModel>
    {

    }
}
