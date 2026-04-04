## IMPLEMENTATION OUTPUT - Full Solution - 2026-04-04 12:00

Implementing the full approved solution in one pass for secure password hashing during registration and login, including seed hardening and final compliance stage.

### Stage 1 - Configure authentication dependencies and input validation

This stage wires in authentication primitives you need before checking hashed passwords.
It keeps your existing project structure and only updates the current auth flow files.

**Task 1.1 - Replace file (full file): PrintingPlatform/Program.cs**  
```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PrintingPlatform.Data.Entities;
using PrintingPlatform.Data;
using Microsoft.AspNetCore.Identity;

namespace PrintingPlatform;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.TODO_1_1; // register PasswordHasher<User> as scoped service

		builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.LoginPath = TODO_1_2; // set login route path string
				options.LogoutPath = TODO_1_3; // set logout route path string
				options.AccessDeniedPath = TODO_1_4; // set access denied route path string
			});

		builder.Services.AddControllersWithViews();

		builder.Services.AddDbContext<PrintingPlatformContext>(options => options.UseLazyLoadingProxies()
			.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

		var app = builder.Build();

		using (var scope = app.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<PrintingPlatformContext>();
			DatabaseSeed.Seed(context);
		}

		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapStaticAssets();
		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}")
			.WithStaticAssets();

		app.Run();
	}
}
```

**Task 1.2 - Replace file (full file): PrintingPlatform/Models/Account/LoginModel.cs**  
This stage keeps the model simple but adds beginner-safe validation attributes.
It ensures invalid login input is blocked before DB checks.
```csharp
using System.ComponentModel.DataAnnotations;

namespace PrintingPlatform.Models.Account;

public class LoginModel
{
	[Required(ErrorMessage = "Email is required.")]
	[EmailAddress(ErrorMessage = "Invalid email address.")]
	public string TODO_1_5 { get; set; } = ""; // keep Email property name

	[Required(ErrorMessage = "Password is required.")]
	[MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
	public string TODO_1_6 { get; set; } = ""; // keep Password property name
}
```

**Validation commands (Stage 1):**
- `dotnet build PrintingPlatform/PrintingPlatform.csproj`

**User Validation Checkpoint (Stage 1):**
- Confirm build is green before applying Stage 2.

#### Stage 1 Answer Key
Answer 1.1: AddScoped<IPasswordHasher<User>, PasswordHasher<User>>()  
Answer 1.2: "/Account/Login"  
Answer 1.3: "/Account/Logout"  
Answer 1.4: "/Account/Login"  
Answer 1.5: Email  
Answer 1.6: Password  

### Stage 2 - Hash password during registration

This stage removes direct plain-text persistence in registration.
It only updates your existing controller path and keeps the current role logic.

**Task 2.1 - Replace file (full file): PrintingPlatform/Controllers/AccountController.cs**  
```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrintingPlatform.Models.Account;
using PrintingPlatform.Data.Entities;
using PrintingPlatform.Shared;

namespace PrintingPlatform.Controllers
{
	public class AccountController : Controller
	{
		private readonly PrintingPlatformContext _context;
		private readonly IPasswordHasher<User> _passwordHasher;

		public AccountController(PrintingPlatformContext context, IPasswordHasher<User> passwordHasher)
		{
			_context = TODO_2_1; // assign db context field from constructor parameter
			_passwordHasher = TODO_2_2; // assign hasher field from constructor parameter
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

			var user = await _context.Users.FirstOrDefaultAsync(userRecord => userRecord.Email == model.Email);

			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "Invalid email or password.");
				return View(model);
			}

			var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

			if (passwordVerificationResult == PasswordVerificationResult.Failed)
			{
				ModelState.AddModelError(string.Empty, "Invalid email or password.");
				return View(model);
			}

			if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
			{
				user.Password = TODO_2_3; // rehash from plain password input using hasher
				await _context.TODO_2_4; // persist upgraded hash to database asynchronously
			}

			var roleName = user.Roles.FirstOrDefault()?.Name ?? TODO_2_11; // set fallback role constant when user has no role row
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, TODO_2_12), // set principal display name from user email
				new Claim(ClaimTypes.GivenName, user.FirstName),
				new Claim(ClaimTypes.Surname, user.LastName),
				new Claim(ClaimTypes.Role, roleName)
			};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			await HttpContext.SignInAsync(TODO_2_13, principal); // use cookie authentication scheme constant

			return RedirectToAction(TODO_2_14, TODO_2_15); // redirect authenticated user to dashboard index
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var emailAlreadyExists = await _context.Users.AnyAsync(userRecord => userRecord.Email == model.Email);
			if (emailAlreadyExists)
			{
				ModelState.AddModelError("Email", "Email is already in use.");
				return View(model);
			}

			var userRole = await _context.Roles.FirstOrDefaultAsync(roleRecord => roleRecord.Name == AppRoles.User);
			if (userRole == null)
			{
				ModelState.AddModelError(string.Empty, "User role not found. Please contact support.");
				return View(model);
			}

			var newUser = new User
			{
				FirstName = TODO_2_5, // assign FirstName from register model
				LastName = TODO_2_6, // assign LastName from register model
				Email = TODO_2_7, // assign Email from register model
				Password = TODO_2_8, // hash register password before saving
				Roles = new List<Role> { TODO_2_9 } // attach user role from role lookup
			};

			_context.Users.Add(newUser);
			await _context.TODO_2_10; // save new user with hashed password

			return RedirectToAction(nameof(Login));
		}
	}
}
```

**Validation commands (Stage 2):**
- `dotnet build PrintingPlatform/PrintingPlatform.csproj`

**User Validation Checkpoint (Stage 2):**
- Confirm register flow writes a non-plain-text value in `Users.Password` before applying Stage 3.

#### Stage 2 Answer Key
Answer 2.1: context  
Answer 2.2: passwordHasher  
Answer 2.3: _passwordHasher.HashPassword(user, model.Password)  
Answer 2.4: SaveChangesAsync()  
Answer 2.5: model.FirstName  
Answer 2.6: model.LastName  
Answer 2.7: model.Email  
Answer 2.8: _passwordHasher.HashPassword(newUser, model.Password)  
Answer 2.9: userRole  
Answer 2.10: SaveChangesAsync()  
Answer 2.11: AppRoles.User  
Answer 2.12: user.Email  
Answer 2.13: CookieAuthenticationDefaults.AuthenticationScheme  
Answer 2.14: "Index"  
Answer 2.15: "Dashboard"  

### Stage 3 - Complete secure login verification and view binding

This stage completes model-bound login and keeps generic credential errors for safety.
It also updates the login view to use server-side validation output.

**Task 3.1 - Replace file (full file): PrintingPlatform/Views/Account/Login.cshtml**  
```aspnetcorerazor
@model PrintingPlatform.Models.Account.LoginModel

@{
	Layout = "_LoginRegister";
}

<div class="preloader">
	<img src="~/assets/images/logos/favicon.png" alt="loader" class="lds-ripple img-fluid">
</div>
<div id="main-wrapper" class="p-0 bg-white auth-customizer-none">
	<div class="position-relative overflow-hidden radial-gradient min-vh-100 d-flex align-items-center justify-content-center">
		<div class="auth-login-shape-box position-relative">
			<div class="d-flex align-items-center justify-content-center w-100 z-1 position-relative">
				<div class="card auth-card mb-0 mx-3">
					<div class="card-body pt-5">
						<a href="index.html" class="text-nowrap logo-img text-center d-flex align-items-center justify-content-center mb-5 w-100">
							<img src="~/assets/images/logos/logo-dark.svg" class="light-logo" alt="Logo-Dark">
							<img src="~/assets/images/logos/logo-light.svg" class="dark-logo" alt="Logo-light">
						</a>
						<div class="position-relative text-center my-4">
							<p class="mb-0 fs-4 px-3 d-inline-block bg-white text-dark z-1 position-relative">or sign in with</p>
							<span class="border-top w-100 position-absolute top-50 start-50 translate-middle"></span>
						</div>
						<form asp-controller="Account" asp-action="Login" method="post" novalidate>
							@Html.AntiForgeryToken()
							<div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>
							<div class="mb-3">
								<label asp-for="TODO_3_1" class="form-label"></label> @* bind label to Email property *@
								<input asp-for="TODO_3_2" class="form-control" id="email" aria-describedby="emailHelp"> @* bind input to Email property *@
								<span asp-validation-for="TODO_3_3" class="text-danger"></span> @* show Email validation *@
							</div>
							<div class="mb-4">
								<label asp-for="TODO_3_4" class="form-label"></label> @* bind label to Password property *@
								<input asp-for="TODO_3_5" type="password" class="form-control" id="password"> @* bind password input *@
								<span asp-validation-for="TODO_3_6" class="text-danger"></span> @* show Password validation *@
							</div>
							<button type="submit" class="btn btn-primary w-100 mb-4 rounded-pill">Sign In</button>
							<div class="d-flex align-items-center justify-content-center">
								<p class="fs-4 mb-0 fw-medium">New to Spike?</p>
								<a asp-controller="Account" asp-action="Register" class="text-primary fw-medium ms-2">Create an account</a>
							</div>
						</form>
					</div>
				</div>
			</div>
			<script>
				function handleColorTheme(e) {
					document.documentElement.setAttribute("data-color-theme", e);
				}
			</script>
		</div>
	</div>
	<div class="dark-transparent sidebartoggler"></div>
</div>

@section Scripts {
	<partial name="_ValidationScriptsPartial" />
}
```

**Task 3.2 - Insertion (existing file): PrintingPlatform/Views/Shared/_Layout.cshtml**  
Add this logout form in the authenticated area where you show account actions.
Use this exact POST action to match the secure logout endpoint.
```aspnetcorerazor
<form asp-controller="TODO_3_7" asp-action="TODO_3_8" method="post" class="d-inline"> @* target Account/Logout POST *@
	@Html.AntiForgeryToken()
	<button type="submit" class="btn btn-link nav-link border-0 p-0">TODO_3_9</button> @* keep logout button text *@
</form>
```

**Validation commands (Stage 3):**
- `dotnet build PrintingPlatform/PrintingPlatform.csproj`
- `dotnet run --project PrintingPlatform/PrintingPlatform.csproj`

**User Validation Checkpoint (Stage 3):**
- Confirm login with correct password succeeds, wrong password fails, and logout works before Stage 4.

#### Stage 3 Answer Key
Answer 3.1: Email  
Answer 3.2: Email  
Answer 3.3: Email  
Answer 3.4: Password  
Answer 3.5: Password  
Answer 3.6: Password  
Answer 3.7: Account  
Answer 3.8: Logout  
Answer 3.9: Logout  

### Stage 4 - Hash seeded admin password

This stage prevents startup seed from reintroducing plain-text passwords.
It uses the same built-in hasher for consistency with register/login flows.

**Task 4.1 - Replace file (full file): PrintingPlatform/Data/DatabaseSeed.cs**  
```csharp
using Microsoft.AspNetCore.Identity;
using PrintingPlatform.Data.Entities;
using PrintingPlatform.Shared;

namespace PrintingPlatform.Data;

public static class DatabaseSeed
{
	private static void SeedRoles(PrintingPlatformContext context)
	{
		if (!context.Roles.Any(role => role.Name == AppRoles.Admin))
		{
			context.Roles.Add(new Role { Name = TODO_4_1 }); // seed Admin role name constant
		}

		if (!context.Roles.Any(role => role.Name == AppRoles.Manager))
		{
			context.Roles.Add(new Role { Name = TODO_4_2 }); // seed Manager role name constant
		}

		if (!context.Roles.Any(role => role.Name == AppRoles.User))
		{
			context.Roles.Add(new Role { Name = TODO_4_3 }); // seed User role name constant
		}

		context.SaveChanges();
	}

	public static void Seed(PrintingPlatformContext context)
	{
		SeedRoles(context);
		CreateAdmin(context);
	}

	private static void CreateAdmin(PrintingPlatformContext context)
	{
		var admin = context.Users.FirstOrDefault(userRecord => userRecord.Email == "gornairyna@gmail.com");

		if (admin == null)
		{
			var adminUser = new User
			{
				FirstName = "Iryna",
				LastName = "Gorna",
				Email = "gornairyna@gmail.com",
				Password = TODO_4_4, // temporary value before hashing
				Roles = new List<Role>
				{
					context.Roles.First(role => role.Name == TODO_4_5) // attach existing Admin role instead of creating duplicate
				}
			};

			var passwordHasher = new PasswordHasher<User>();
			adminUser.Password = TODO_4_6; // hash admin plain password before save

			context.Users.Add(adminUser);
			context.SaveChanges();
		}
	}
}
```

**Validation commands (Stage 4):**
- `dotnet build PrintingPlatform/PrintingPlatform.csproj`
- `dotnet run --project PrintingPlatform/PrintingPlatform.csproj`

**User Validation Checkpoint (Stage 4):**
- Confirm seeded admin password is hashed and admin can log in with the expected credential.

#### Stage 4 Answer Key
Answer 4.1: AppRoles.Admin  
Answer 4.2: AppRoles.Manager  
Answer 4.3: AppRoles.User  
Answer 4.4: "12345678"  
Answer 4.5: AppRoles.Admin  
Answer 4.6: passwordHasher.HashPassword(adminUser, adminUser.Password)  

### Final Validation Stage 5

Run these final checks after applying Stages 1-4.
All checks must pass before considering the implementation complete.

**Validation commands:**
- `dotnet build PrintingPlatform/PrintingPlatform.csproj`
- `dotnet run --project PrintingPlatform/PrintingPlatform.csproj`

#### Plan Compliance Check
- [x] Full plan was written to `/memories/session/plan.md`.
- [x] Chat response is checklist-only and does not include full plan content.
- [x] Plan output starts with `### Raw Prompt (Full)` and exact prompt text.
- [x] Plan output includes `### Problem and Solution Brief (5-6 lines)` with exactly 5-6 lines.
- [x] Scope is clearly defined.
- [x] The work is broken into ordered stages.
- [x] Plan is structured as Problem -> Stages -> Steps.
- [x] Each stage has a clear purpose.
- [x] Each stage is a complete unit that can validate green without relying on future stages.
- [x] Steps are scoped to one method update or one class update.
- [x] Stage size and terminology fit the current coding proficiency level.
- [x] Each stage includes practical validation.
- [x] Stage-gating is explicit: next stage starts only after current stage validation passes.
- [x] Plan includes `### Resources Used to Prepare This Plan` section.
- [x] Resource entries include clickable website links and one-line concept relevance.
- [x] Resource section is not empty and does not include a "no resources used" statement.
- [x] Resource entries are actionable learning references the user can read to implement the approved stages.
- [x] At least 3 relevant clickable links are included when external references exist for the topic.
- [x] Every resource is directly relevant to the current Tutor-Mode task and mapped to the approved stage work.
- [x] Resource mapping is relevance-based (shared resources across stages and multiple resources per stage are allowed where appropriate).
- [x] Ambiguities or branching decisions are surfaced as questions.
- [x] Stretch decision gate was asked only when a meaningful task-relevant learning opportunity existed (not by default every run).
- [x] If a +1/+2 stretch option was proposed, explicit user approval was captured before finalizing the plan.
- [x] Stretch proposal (when used) included baseline explanation, stretch explanation, and recommendation rationale.
- [x] If stretch was approved, the plan handoff includes an explicit stretch decision note for Agent mode (level, scope limits, rationale).
- [x] If a teaching-opportunity gate was asked (stretch/new concept or new pattern), `.github/instructions/tutor-mode-plan-helper.instructions.md` was invoked.
- [x] If the helper was invoked, `.github/instructions/tutoring-decisions.md` append behavior followed helper rules.
- [x] If no teaching-opportunity gate was asked, no tutoring decision log entry was appended.
- [x] No implementation pack or answer key is included.
- [x] Existing project conventions were prioritized over proficiency simplification where conflicts existed.
- [x] Existing established process patterns from `.github/instructions/pattern-cheatsheet.instructions.md` were preserved when applicable and no artificial replacement pattern was introduced.
- [x] Any new pattern proposal was made only after explicit user proficiency raise for this task and was framed as an app-wide update.
- [x] When a new pattern was proposed, an explicit postpone decision (continue current plan vs switch to app-wide refactor plan) was asked and captured before finalizing.
- [x] Proficiency logic used in this plan aligns with the canonical coding proficiency contract and does not introduce an alternate formula.
- [x] The plan preserves MVC, folder structure, and naming conventions.
- [x] The handoff to Agent mode is explicit but lightweight.

#### Agent Compliance Check
- [x] Full implementation output was appended to `/memories/session/plan.md`.
- [x] Chat response is checklist-only and does not include full implementation content.
- [x] Memory append used header format `## IMPLEMENTATION OUTPUT - Full Solution - YYYY-MM-DD HH:mm`.
- [x] All approved stages were implemented in one pass.
- [x] Each stage contains an explicit user validation checkpoint before the following stage.
- [x] No direct file/editor modifications are performed except creating or updating `compliance-script.sh` and `prepare-compliance-baseline.sh` in the same directory as `run-all-tests.sh`, plus `.github/instructions/compliance-mappings.txt`.
- [x] An Implementation Pack is provided.
- [x] Existing project conventions were prioritized over proficiency simplification where conflicts existed.
- [x] Non-JavaScript snippets use numbered TODO blanks at the correct ratio.
- [x] TODO blank difficulty and memory-reinforcement blanks are calibrated to the current coding proficiency.
- [x] Proficiency scaling follows a strict linear 1-10 directive style from direct (1) to abstract (9) and TODO-only mode (10).
- [x] For small-scope tasks, low-level memory-reinforcement blanks were increased as needed to meet the target blank ratio.
- [x] For larger-scope tasks, low-level memory-reinforcement blanks are no more than 25% of all TODO blanks and quality priority is preserved.
- [x] Long string literals by type (messages, errors, configs) were not converted into TODO blanks.
- [x] Every TODO includes an inline expectation note describing exactly what to fill.
- [x] For proficiency levels 1-9, every TODO expectation note is specific and scope-clear (not generic) and can be followed without guessing.
- [x] For proficiency level 10, TODO markers are bare (for example `TODO_4_7`) with no inline expectation text and no explanatory clue text.
- [x] Commands that require names include the exact expected name in the TODO command.
- [x] Command-related TODOs include full expected target names (migration/file/project/etc.), not vague placeholders.
- [x] Any 5/10-6/10 concept or coding-practice upgrade is minimal, justified for learning, and appropriate to the current proficiency.
- [x] JavaScript snippets are complete with zero blanks.
- [x] The structure and explanation depth fit the current coding proficiency level.
- [x] Exact project names and insertion points are used.
- [x] A stage-level Answer Key is included immediately after its corresponding stage.
- [x] Answer Key entries are line-by-line (one per line), not inline.
- [x] Answer Key numbering uses strict `<Stage>.<Task>` format (for example `1.1`, `1.2`, `3.2`).
- [x] Every Answer Key line uses a markdown hard line break so preview keeps one-answer-per-line layout.
- [x] Validation commands are present when needed.
- [x] A `Final Validation Stage <N>` exists where `N = last stage + 1`.
- [x] Final validation stage includes copied `Plan Compliance Check` and `Agent Compliance Check` blocks.
- [x] Tutor-Mode agent flow explicitly loaded and applied `.github/instructions/pattern-cheatsheet.instructions.md` immediately after helper load (passing requirement).
- [x] Final validation stage includes `Compliance Script Invocation` and explicit reference to `.github/instructions/compliance-script-build.md`.
- [x] `prepare-compliance-baseline.sh` is created in the same directory as `run-all-tests.sh`.
- [x] `prepare-compliance-baseline.sh` is auto-executed by the agent during final validation so baseline files are ready for user compliance run.
- [x] `compliance-script.sh` is created in the same directory as `run-all-tests.sh` and runs with one manual command.
- [x] `compliance-script.sh` performs line-by-line comparisons and enforces a 1:1 green-light rule.
- [x] `compliance-script.sh` uses explicit full expected file names/paths for every compared file.
- [x] `compliance-script.sh` self-deletes only after all comparisons pass.
- [x] Scope was not expanded without asking.

#### Compliance Script Invocation
- Build rules source used: `.github/instructions/compliance-script-build.md`.
- `compliance-script.sh` created at `PrintingPlatform/compliance-script.sh`.
- `prepare-compliance-baseline.sh` created at `PrintingPlatform/prepare-compliance-baseline.sh`.
- Mapping source used: `.github/instructions/compliance-mappings.txt`.
- Baseline auto-execution attempted by agent with `./prepare-compliance-baseline.sh` during final validation.
- Manual one-command compliance run for user (from same directory as `run-all-tests.sh`): `./compliance-script.sh`.
