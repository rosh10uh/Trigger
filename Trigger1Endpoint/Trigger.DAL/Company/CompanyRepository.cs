using OneRPP.Restful.DataAnnotations;
using OneRPP.Restful.DAO;
using Trigger.DTO;

namespace Trigger.DAL.Company
{
	[QueryPath("Trigger.DAL.Query.Company.Company")]
	public class CompanyRepository : DaoRepository<CompanyDetailsModel>
	{
		
	}
}
