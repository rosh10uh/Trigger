using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.Ethnicity
{
    [QueryPath("Trigger.DAL.Query.Ethnicity.Ethnicity")]
    public class EthnicityRepository : DaoRepository<RaceOrEthnicityModel>
    {
    }
}
