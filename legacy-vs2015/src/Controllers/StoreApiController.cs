using System;
using System.Net;
using System.Web.Http;
using Kirana.WebApi.Data;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Controllers
{
    // Base for store-owner endpoints: resolves the logged-in owner/manager and
    // exposes their StoreId. Call EnsureStoreUser() at the top of each action.
    public abstract class StoreApiController : ApiController
    {
        protected Guid CurrentStoreId;

        protected IHttpActionResult EnsureStoreUser()
        {
            var user = AuthUtil.GetCurrentUser(Request);
            if (user == null || (user.Role != UserRole.Owner && user.Role != UserRole.Manager))
                return Content(HttpStatusCode.Unauthorized, new { error = "Store owner login required." });
            if (!user.StoreId.HasValue)
                return Content(HttpStatusCode.BadRequest, new { error = "This account is not linked to a store." });

            CurrentStoreId = user.StoreId.Value;
            return null;
        }
    }
}
