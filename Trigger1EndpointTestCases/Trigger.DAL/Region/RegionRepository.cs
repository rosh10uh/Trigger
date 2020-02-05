using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.Region
{
    [QueryPath("Trigger.DAL.Query.Region.Region")]
    public class RegionRepository : DaoRepository<RegionModel>
    {
    }
}
