using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.ExcelUpload
{
    /// <summary>
    /// Class for add employee auth claims details
    /// </summary>
    [QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
    public class AuthUserClaimExcelRepository : DaoRepository<AuthUserClaimExcelModel>
    {
        /// <summary>
        /// Add employee claim details 
        /// </summary>
        /// <param name="authUserClaimExcelModel"></param>
        /// <returns>AuthUserClaimExcelModel</returns>
        public virtual AuthUserClaimExcelModel AddAuthClaims(AuthUserClaimExcelModel authUserClaimExcelModel)
        {
            return ExecuteQuery<AuthUserClaimExcelModel>(authUserClaimExcelModel, SPFileName.AddAuthClaimsFromExcel);
        }
    }
}
