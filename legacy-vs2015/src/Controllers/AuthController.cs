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

        // GET /api/auth/me  -> resolve the current user from the X-Auth-Token header.
        // Used server-side by the adaptive login page to re-validate a token
        // obtained via Google / OTP before establishing the ASP.NET session.
        [HttpGet, Route("me")]
        public IHttpActionResult Me()
        {
            var user = AuthUtil.GetCurrentUser(Request);
            if (user == null)
                return Content(HttpStatusCode.Unauthorized, new { error = "Not signed in." });

            return Ok(new LoginResponse
            {
                Token = user.SessionToken,
                Role = user.Role.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                StoreId = user.StoreId
            });
        }

        // POST /api/auth/google  -> store-owner sign-in with a Google ID token.
        // The browser runs Google Sign-In and posts the resulting id_token here.
        [HttpPost, Route("google")]
        public IHttpActionResult Google(GoogleLoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.IdToken))
                return BadRequest("Google token is required.");

            var g = GoogleTokenValidator.Validate(req.IdToken);
            if (g == null || !g.EmailVerified)
                return Content(HttpStatusCode.Unauthorized, new { error = "Google sign-in could not be verified." });

            using (var db = new KiranaDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == g.Email);
                if (user == null)
                    return Content(HttpStatusCode.Unauthorized, new
                    {
                        error = "No account is registered with this Google email. Please register your store first."
                    });

                if (!user.IsActive)
                    return Content(HttpStatusCode.Unauthorized, new { error = "This account is disabled." });

                // Store staff must belong to an approved (Active) store.
                if (user.Role != UserRole.SuperAdmin && user.StoreId.HasValue)
                {
                    var store = db.Stores.Find(user.StoreId.Value);
                    if (store == null || store.Status != StoreStatus.Active)
                        return Content(HttpStatusCode.Unauthorized,
                            new { error = "Your store is not active yet. Please wait for admin approval." });
                }

                user.Provider = AuthProvider.Google;
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

        // POST /api/auth/request-otp  -> issue a one-time code for a consumer phone.
        [HttpPost, Route("request-otp")]
        public IHttpActionResult RequestOtp(RequestOtpRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Phone))
                return BadRequest("Phone number is required.");

            var phone = OtpService.Normalize(req.Phone);
            if (phone.Length < 8)
                return BadRequest("Please enter a valid phone number.");

            using (var db = new KiranaDbContext())
            {
                var code = OtpService.Issue(db, phone);

                // In the course/dev build there is no SMS gateway, so the code is
                // returned here to allow end-to-end testing. Disable via web.config
                // appSetting "OtpDevMode" = false for production.
                if (OtpService.DevMode)
                    return Ok(new { sent = true, devMode = true, code = code });

                return Ok(new { sent = true, devMode = false });
            }
        }

        // POST /api/auth/verify-otp  -> verify the code, find-or-create the customer, log in.
        [HttpPost, Route("verify-otp")]
        public IHttpActionResult VerifyOtp(VerifyOtpRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Phone) || string.IsNullOrWhiteSpace(req.Code))
                return BadRequest("Phone and code are required.");

            var phone = OtpService.Normalize(req.Phone);

            using (var db = new KiranaDbContext())
            {
                if (!OtpService.Verify(db, phone, req.Code))
                    return Content(HttpStatusCode.Unauthorized, new { error = "Invalid or expired code." });

                var user = db.Users.FirstOrDefault(u => u.Phone == phone && u.Role == UserRole.Customer);
                if (user == null)
                {
                    user = new User
                    {
                        // OTP accounts have no email/password; use a stable placeholder email.
                        Email = "otp+" + phone + "@kirana.local",
                        Phone = phone,
                        FullName = (req.FullName ?? "").Trim(),
                        Role = UserRole.Customer,
                        Provider = AuthProvider.Otp,
                        IsActive = true
                    };
                    db.Users.Add(user);
                }
                else if (!user.IsActive)
                {
                    return Content(HttpStatusCode.Unauthorized, new { error = "This account is disabled." });
                }

                user.Provider = AuthProvider.Otp;
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
