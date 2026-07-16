using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Kirana.WebApi.Data;
using Kirana.WebApi.Dtos;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        // POST /api/auth/register-customer  -> creates a shopper account.
        [HttpPost, Route("register-customer")]
        public IHttpActionResult RegisterCustomer(RegisterCustomerRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Name, email and password are required.");

            var email = req.Email.Trim().ToLowerInvariant();
            using (var db = new KiranaDbContext())
            {
                if (db.Users.Any(u => u.Email == email))
                    return Content(HttpStatusCode.Conflict, new { error = "An account with this email already exists." });

                db.Users.Add(new User
                {
                    Email = email,
                    FullName = (req.FullName ?? "").Trim(),
                    PasswordHash = PasswordHasher.Hash(req.Password),
                    Role = UserRole.Customer,
                    IsActive = true
                });
                db.SaveChanges();
                return Ok(new { email = email });
            }
        }

        // POST /api/auth/login  -> returns a session token for SuperAdmin, store staff, or customer.
        [HttpPost, Route("login")]
        public IHttpActionResult Login(LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Email and password are required.");

            var email = req.Email.Trim().ToLowerInvariant();

            using (var db = new KiranaDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user == null || !PasswordHasher.Verify(user.PasswordHash, req.Password))
                    return Content(HttpStatusCode.Unauthorized, new { error = "Invalid email or password." });

                if (!user.IsActive)
                    return Content(HttpStatusCode.Unauthorized, new { error = "This account is disabled." });

                // Store staff can only sign in once their store is Active (approved).
                if (user.Role != UserRole.SuperAdmin && user.StoreId.HasValue)
                {
                    var store = db.Stores.Find(user.StoreId.Value);
                    if (store == null || store.Status != StoreStatus.Active)
                        return Content(HttpStatusCode.Unauthorized,
                            new { error = "Your store is not active yet. Please wait for admin approval." });
                }

                user.SessionToken = Guid.NewGuid().ToString("N");
                db.SaveChanges();

                return Ok(new LoginResponse
                {
                    Token = user.SessionToken,
                    Role = user.Role.ToString(),
                    FullName = user.FullName,
                    Email = user.Email,
                    StoreId = user.StoreId
                });
            }
        }
    }
}
