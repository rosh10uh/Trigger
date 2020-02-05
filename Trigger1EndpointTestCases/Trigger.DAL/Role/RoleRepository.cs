using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.Role
{
    [QueryPath("Trigger.DAL.Query.Role.Role")]
    public class RoleRepository : DaoRepository<RoleModel>
    {
    }
}
