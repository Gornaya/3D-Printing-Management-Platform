using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Models.Account;
using Microsoft.EntityFrameworkCore;
using PrintingPlatform.Data;
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
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user = await _context.Users
                .Include(userRecord => userRecord.Roles)
                .FirstOrDefaultAsync(userRecord => userRecord.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            PasswordVerificationResult passwordVerificationResult = _passwordHasher
                .VerifyHashedPassword(user, user.Password, model.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError(string.Empty, "Your account is blocked.");
                return View(model);
            }

            if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(user, model.Password);
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            string roleName = user.Roles
                .Select(roleRecord => roleRecord.Name)
                .FirstOrDefault() ?? AppRoles.User;

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
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
            if (!ModelState.IsValid)
            {
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
                Roles = new List<Role> { userRole }
            };

            newUser.Password = _passwordHasher.HashPassword(newUser, model.Password);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}