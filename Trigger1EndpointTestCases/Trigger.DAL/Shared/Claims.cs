using System.Linq;
using System.Security.Claims;
using OneRPP.Restful.Contracts.Services;
using Trigger.BLL.Shared.Interfaces;

namespace Trigger.DAL.Shared
{
    public class Claims : IClaims
    {
        private readonly IContext _ctx;

        public Claims(IContext context)
        {
            this._ctx = context;
        }

        /// <summary>
        /// Get claim from request object
        /// </summary>
        /// <param name="claimName">Claim Name</param>
        /// <returns>Claim from request object</returns>
        public Claim this[string claimName]
        {
            get
            {
                return _ctx.reqobj.HttpContext.User.Claims.FirstOrDefault(x => x.Type == claimName);
            }
        }

        /// <summary>
        /// Property Name   : UserId
        /// Author          : Divyesh Saijwani
        /// Creation Date   : 02-07-2018
        /// Purpose         : To return User id from claims
        /// </summary>
        /// <returns>User Id</returns>
        public string UserId
        {
            get
            {
                return this["sub"].Value;
            }
        }

        /// <summary>
        /// Property Name   : ClientId
        /// Author          : Divyesh Saijwani
        /// Creation Date   : 02-07-2018
        /// Purpose         : To return ClientId from claims
        /// </summary>
        /// <returns>Client Id</returns>
        public string ClientId
        {
            get
            {
                return this["client_id"].Value;
            }
        }

        public string Role
        {
            get
            {
                return this["role"].Value;
            }
        }
    }
}
