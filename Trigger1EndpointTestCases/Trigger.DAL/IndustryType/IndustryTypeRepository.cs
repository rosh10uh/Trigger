using OneRPP.Restful.DataAnnotations;
using OneRPP.Restful.DAO;
using  Trigger.DTO;

namespace Trigger.DAL.IndustryType
{
	[QueryPath("Trigger.DAL.Query.IndustryType.IndustryType")]
	public class IndustryTypeRepository : DaoRepository<IndustryModel>
	{
	}
}
