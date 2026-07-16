using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Kirana.WebApi.Data;
using Kirana.WebApi.Dtos;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Controllers
{
    [RoutePrefix("api/stores")]
    public class StoresController : ApiController
    {
        // Returns 401 (as IHttpActionResult) if the caller is not a logged-in SuperAdmin.
        private IHttpActionResult RequireSuperAdmin()
        {
            var user = AuthUtil.GetCurrentUser(Request);
            if (user == null || user.Role != UserRole.SuperAdmin)
                return Content(HttpStatusCode.Unauthorized, new { error = "Super admin login required." });
            return null;
        }

        // POST /api/stores/register
        [HttpPost, Route("register")]
        public IHttpActionResult Register(RegisterStoreRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Email and password are required.");

            var email = req.Email.Trim().ToLowerInvariant();

            using (var db = new KiranaDbContext())
            {
                if (db.Users.Any(u => u.Email == email))
                    return Content(System.Net.HttpStatusCode.Conflict, new { error = "An account with this email already exists." });

                var store = new Store
                {
                    Name = (req.StoreName ?? "").Trim(),
                    OwnerName = (req.OwnerName ?? "").Trim(),
                    Email = email,
                    Phone = (req.Phone ?? "").Trim(),
                    AddressLine = req.AddressLine,
                    City = req.City,
                    Gstin = req.Gstin,
                    Pan = req.Pan
                };

                var owner = new User
                {
                    Email = email,
                    FullName = store.OwnerName,
                    PasswordHash = PasswordHasher.Hash(req.Password),
                    Role = UserRole.Owner,
                    StoreId = store.Id
                };

                db.Stores.Add(store);
                db.Users.Add(owner);
                db.SaveChanges();

                return Ok(ToDto(store));
            }
        }

        // GET /api/stores/pending  (SuperAdmin only)
        [HttpGet, Route("pending")]
        public IHttpActionResult Pending()
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var stores = db.Stores
                    .Where(s => s.Status == StoreStatus.Pending)
                    .OrderBy(s => s.CreatedAt)
                    .ToList();
                return Ok(stores.Select(ToDto));
            }
        }

        // GET /api/stores  (SuperAdmin only)
        [HttpGet, Route("")]
        public IHttpActionResult All()
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var stores = db.Stores.OrderByDescending(s => s.CreatedAt).ToList();
                return Ok(stores.Select(ToDto));
            }
        }

        // POST /api/stores/{id}/approve
        [HttpPost, Route("{id:guid}/approve")]
        public IHttpActionResult Approve(Guid id)
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var store = db.Stores.Find(id);
                if (store == null) return NotFound();
                if (store.Status != StoreStatus.Pending)
                    return BadRequest("Only pending stores can be approved.");

                store.Status = StoreStatus.Active;
                store.ApprovedAt = DateTime.UtcNow;
                store.RejectionReason = null;
                db.SaveChanges();
                return Ok(ToDto(store));
            }
        }

        // POST /api/stores/{id}/reject
        [HttpPost, Route("{id:guid}/reject")]
        public IHttpActionResult Reject(Guid id, RejectRequest req)
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;

            if (req == null || string.IsNullOrWhiteSpace(req.Reason))
                return BadRequest("A rejection reason is required.");

            using (var db = new KiranaDbContext())
            {
                var store = db.Stores.Find(id);
                if (store == null) return NotFound();
                if (store.Status != StoreStatus.Pending)
                    return BadRequest("Only pending stores can be rejected.");

                store.Status = StoreStatus.Rejected;
                store.RejectionReason = req.Reason.Trim();
                db.SaveChanges();
                return Ok(ToDto(store));
            }
        }

        private static StoreDto ToDto(Store s)
        {
            return new StoreDto
            {
                Id = s.Id,
                Name = s.Name,
                OwnerName = s.OwnerName,
                Email = s.Email,
                Phone = s.Phone,
                City = s.City,
                Gstin = s.Gstin,
                Pan = s.Pan,
                Status = s.Status.ToString(),
                CreatedAt = s.CreatedAt,
                ApprovedAt = s.ApprovedAt,
                RejectionReason = s.RejectionReason
            };
        }
    }
}
