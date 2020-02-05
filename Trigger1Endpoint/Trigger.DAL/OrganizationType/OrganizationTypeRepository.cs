using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO.Organization;

namespace Trigger.DAL.OrganizationType
{
    [QueryPath("Trigger.DAL.Query.OrganizationType.OrganizationType")]
    public class OrganizationTypeRepository: DaoRepository<OrganizationTypeModel>
    {
    }
}
