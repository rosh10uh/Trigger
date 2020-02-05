using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.Country
{
    [QueryPath("Trigger.DAL.Query.Country.Country")]
    public class CountryRepository : DaoRepository<CountryModel>
    {
    }
}
