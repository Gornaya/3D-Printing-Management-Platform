using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Models.Account;
using Microsoft.EntityFrameworkCore;
using PrintingPlatform.Data.Entities;
using PrintingPlatform.Shared;

namespace PrintingPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly PrintingPlatformContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(PrintingPlatformContext context, 
        IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync
            (userRecord => userRecord.Email == model.Email);
            
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var passwordVerificationResult = _passwordHasher.
            VerifyHashedPassword(user, user.Password, model.Password);

			if (passwordVerificationResult == 
            PasswordVerificationResult.Failed)
			{
				ModelState.AddModelError(string.Empty, "Invalid email or password.");
				return View(model);
			}

			if (passwordVerificationResult == 
            PasswordVerificationResult.SuccessRehashNeeded)
			{
				user.Password = _passwordHasher.HashPassword(user, model.Password);
				_context.Update(user);
				await _context.SaveChangesAsync();
			}

			var roleName = user.Roles.FirstOrDefault()?.Name ?? "User";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var Identity = new ClaimsIdentity(claims, 
            CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(Identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var normalizedRoleName = (roleName ?? AppRoles.User).Trim();
            var isAdmin = string.Equals(normalizedRoleName, AppRoles.Admin, StringComparison.OrdinalIgnoreCase);
            var isManager = string.Equals(normalizedRoleName, AppRoles.Manager, StringComparison.OrdinalIgnoreCase);
            if (isAdmin || isManager)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return RedirectToAction("Index", "Home");
            
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var emailAlreadyExists = await _context.Users.AnyAsync
            (userRecord => userRecord.Email == model.Email);
            if (emailAlreadyExists)
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View(model);
            }

            var userRole = await _context.Roles.FirstOrDefaultAsync
            (roleRecord => roleRecord.Name == AppRoles.User);
             if (userRole == null)
            {
                ModelState.AddModelError(string.Empty, "User role not found. Please contact support.");
                return View(model);
            }

            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Roles = new List<Role> {userRole}
            };

            newUser.Password = _passwordHasher.HashPassword(newUser, model.Password);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}