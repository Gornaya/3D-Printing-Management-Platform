using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrintingPlatform.Data;
using PrintingPlatform.Models.AdminUsers;
using PrintingPlatform.Shared;

namespace PrintingPlatform.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class AdminUsersController : Controller
    {
        private readonly PrintingPlatformContext _context;

        public AdminUsersController(PrintingPlatformContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Users
                .Include(u => u.Roles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalized = search.Trim().ToLower();

                query = query.Where(u =>
                    ((u.FirstName ?? "") + " " + (u.LastName ?? "")).ToLower().Contains(normalized) ||
                    (u.Email ?? "").ToLower().Contains(normalized));
            }

            var users = await query
                .OrderBy(user => user.FirstName)
                .ThenBy(user => user.LastName)
                .Select(user => new AdminUserListItemViewModel
                {
                    Id = user.Id,
                    FullName = ((user.FirstName ?? "") + " " + (user.LastName ?? "")).Trim(),
                    Email = user.Email ?? "",
                    Role = user.Roles.Select(r => r.Name).FirstOrDefault() ?? AppRoles.User,
                    IsBlocked = user.IsBlocked
                })
                .ToListAsync();

            var vm = new AdminUsersIndexViewModel
            {
                Search = search,
                Users = users
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int userId, string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole))
            {
                TempData["AdminUsersError"] = "Role was not selected.";
                return RedirectToAction(nameof(Index));
            }

            var allowedRoles = new[] { AppRoles.Admin, AppRoles.Manager, AppRoles.User };
            if (!allowedRoles.Contains(newRole))
            {
                TempData["AdminUsersError"] = "Invalid role.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                TempData["AdminUsersError"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == newRole);
            if (role == null)
            {
                TempData["AdminUsersError"] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }

            user.Roles.Clear();
            user.Roles.Add(role);

            await _context.SaveChangesAsync();

            TempData["AdminUsersSuccess"] = "User role updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlock(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                TempData["AdminUsersError"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var isAdmin = user.Roles.Any(r => r.Name == AppRoles.Admin);
            if (isAdmin)
            {
                TempData["AdminUsersError"] = "Admin user cannot be blocked.";
                return RedirectToAction(nameof(Index));
            }

            user.IsBlocked = !user.IsBlocked;
            await _context.SaveChangesAsync();

            TempData["AdminUsersSuccess"] = user.IsBlocked
                ? "User has been blocked."
                : "User has been unblocked.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                TempData["AdminUsersError"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var isAdmin = user.Roles.Any(r => r.Name == AppRoles.Admin);
            if (isAdmin)
            {
                TempData["AdminUsersError"] = "Admin user cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["AdminUsersSuccess"] = "User deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}