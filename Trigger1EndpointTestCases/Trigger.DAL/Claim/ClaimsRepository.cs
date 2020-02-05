using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;

namespace Trigger.DAL.ClaimRepo
{
    [QueryPath("Trigger.DAL.Query.Claim.Claim")]
    public class ClaimsRepository : DaoRepository<List<Claims>>
    {
    }

    [QueryPath("Trigger.DAL.Query.Claim.Claim")]
    public class ClaimsCommonRepository : DaoRepository<Claims>
    {
    }
}
