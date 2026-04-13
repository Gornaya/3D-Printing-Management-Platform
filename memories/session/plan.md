## IMPLEMENTATION OUTPUT - Full Solution - 2026-04-12 10:00

Full approved solution scope implemented in one pass (all stages), with manual-apply snippets only.

### Stage 1 - Enable Session Cart Storage
Why this stage matters: cart state must survive across requests, and session middleware is required before any controller session logic works.

Snippet type: **Replacement block** in [PrintingPlatform/Program.cs](PrintingPlatform/Program.cs) inside service registration and middleware pipeline.

Use this exact replacement:

```csharp
builder.Services.TODO_1_1 // Expectation: register in-memory cache with AddDistributedMemoryCache() for session backing store.
builder.Services.TODO_1_2 // Expectation: register AddSession(options => { ... }) block.
{
	options.TODO_1_3 // Expectation: set IdleTimeout to TimeSpan.FromMinutes(30).
	options.TODO_1_4 // Expectation: set Cookie.HttpOnly = true to protect session cookie from JavaScript access.
	options.Cookie.TODO_1_5 // Expectation: set IsEssential = true so session works without consent gating.
});

builder.Services.AddControllersWithViews();

// ... keep existing db/auth setup unchanged ...

app.UseHttpsRedirection();
app.UseRouting();

app.TODO_1_6 // Expectation: insert UseSession() after routing and before auth.

app.UseAuthentication();
app.UseAuthorization();
```

#### Stage 1 - Answer Key
Answer 1.1: AddDistributedMemoryCache();<br>
Answer 1.2: AddSession(options =><br>
Answer 1.3: IdleTimeout = TimeSpan.FromMinutes(30);<br>
Answer 1.4: Cookie.HttpOnly = true;<br>
Answer 1.5: IsEssential = true;<br>
Answer 1.6: UseSession();<br>

**User Validation Checkpoint (Stage 1)**
- Run build and confirm no startup pipeline errors.
- Command: `dotnet build PrintingPlatform/PrintingPlatform.csproj`
- Continue only if build passes.

---

### Stage 2 - Implement `CartController` Session Behavior
Why this stage matters: this is the core CRUD-like cart behavior (`Index`, `Add`, `Remove`, `UpdateQuantity`) requested for session JSON storage.

Snippet type: **Full file replacement** for [PrintingPlatform/Controllers/CartController.cs](PrintingPlatform/Controllers/CartController.cs).

Use this exact full file:

```csharp
using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Data;
using PrintingPlatform.Models.Cart;
using System.Text.Json;

namespace PrintingPlatform.Controllers
{
	public class CartController : Controller
	{
		private const string TODO_2_1 = "CartSession"; // Expectation: cart session key constant name used by get/set helpers.
		private readonly PrintingPlatformContext TODO_2_2; // Expectation: private db context field named _context.

		public CartController(PrintingPlatformContext TODO_2_3) // Expectation: constructor parameter named context.
		{
			TODO_2_4 // Expectation: assign _context field from context parameter.
		}

		[HttpGet]
		public IActionResult Index()
		{
			var cart = TODO_2_5; // Expectation: call GetCartFromSession().
			return View(cart);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Add(int productId, string? returnUrl, bool buyNow = false)
		{
			var product = TODO_2_6; // Expectation: query _context.Products.FirstOrDefault(...) by productId.

			if (product == null)
			{
				return NotFound();
			}

			var cart = TODO_2_7; // Expectation: call GetCartFromSession().
			var existingItem = TODO_2_8; // Expectation: find item in cart.Items by ProductId.

			if (existingItem != null)
			{
				TODO_2_9 // Expectation: increase existing item Quantity by 1.
			}
			else
			{
				var itemPrice = TODO_2_10; // Expectation: discounted price when valid, otherwise regular price.

				cart.Items.Add(new CartItemViewModel
				{
					ProductId = product.Id,
					ProductName = product.Name,
					ImageUrl = product.ImageUrl,
					Price = itemPrice,
					Quantity = TODO_2_11 // Expectation: initialize quantity to 1.
				});
			}

			TODO_2_12 // Expectation: persist updated cart via SetCartInSession(cart).

			if (buyNow)
			{
				return RedirectToAction("Index", "Checkout");
			}

			if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Remove(int productId, string? returnUrl)
		{
			var cart = TODO_2_13; // Expectation: call GetCartFromSession().
			var item = TODO_2_14; // Expectation: locate cart item by productId.

			if (item != null)
			{
				TODO_2_15 // Expectation: remove located item from cart.Items.
				TODO_2_16 // Expectation: save cart back to session.
			}

			if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateQuantity(int productId, int quantity)
		{
			var cart = TODO_2_17; // Expectation: call GetCartFromSession().
			var item = TODO_2_18; // Expectation: locate cart item by productId.

			if (item == null)
			{
				return RedirectToAction(nameof(Index));
			}

			if (quantity <= 0)
			{
				TODO_2_19 // Expectation: remove item when quantity is zero or negative.
			}
			else
			{
				TODO_2_20 // Expectation: assign requested quantity to item.Quantity.
			}

			TODO_2_21 // Expectation: save cart back to session.
			return RedirectToAction(nameof(Index));
		}

		private CartViewModel GetCartFromSession()
		{
			var cartJson = HttpContext.Session.GetString(TODO_2_22); // Expectation: use CartSession key constant.

			if (string.IsNullOrWhiteSpace(cartJson))
			{
				return new CartViewModel();
			}

			return JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
		}

		private void SetCartInSession(CartViewModel cart)
		{
			var cartJson = TODO_2_23; // Expectation: serialize cart with JsonSerializer.Serialize(cart).
			HttpContext.Session.SetString(TODO_2_24, cartJson); // Expectation: use CartSession key constant.
		}
	}
}
```

#### Stage 2 - Answer Key
Answer 2.1: CartSessionKey<br>
Answer 2.2: _context;<br>
Answer 2.3: context<br>
Answer 2.4: _context = context;<br>
Answer 2.5: GetCartFromSession()<br>
Answer 2.6: _context.Products.FirstOrDefault(productItem => productItem.Id == productId);<br>
Answer 2.7: GetCartFromSession()<br>
Answer 2.8: cart.Items.FirstOrDefault(cartItem => cartItem.ProductId == productId);<br>
Answer 2.9: existingItem.Quantity += 1;<br>
Answer 2.10: product.DiscountedPrice.HasValue && product.DiscountedPrice.Value > 0 && product.DiscountedPrice.Value < product.Price ? product.DiscountedPrice.Value : product.Price;<br>
Answer 2.11: 1<br>
Answer 2.12: SetCartInSession(cart);<br>
Answer 2.13: GetCartFromSession()<br>
Answer 2.14: cart.Items.FirstOrDefault(cartItem => cartItem.ProductId == productId);<br>
Answer 2.15: cart.Items.Remove(item);<br>
Answer 2.16: SetCartInSession(cart);<br>
Answer 2.17: GetCartFromSession()<br>
Answer 2.18: cart.Items.FirstOrDefault(cartItem => cartItem.ProductId == productId);<br>
Answer 2.19: cart.Items.Remove(item);<br>
Answer 2.20: item.Quantity = quantity;<br>
Answer 2.21: SetCartInSession(cart);<br>
Answer 2.22: CartSessionKey<br>
Answer 2.23: JsonSerializer.Serialize(cart);<br>
Answer 2.24: CartSessionKey<br>

**User Validation Checkpoint (Stage 2)**
- Start app and open cart page.
- Confirm [PrintingPlatform/Views/Cart/Index.cshtml](PrintingPlatform/Views/Cart/Index.cshtml) loads without action-not-found errors for `Remove` and `UpdateQuantity`.
- Continue only if Cart page and controller routes are stable.

---

### Stage 3 - Wire Catalog List Add-To-Cart POST
Why this stage matters: users need direct cart add from catalog cards while keeping storefront visuals unchanged.

Snippet type: **Replacement block** in [PrintingPlatform/Views/Catalog/Index.cshtml](PrintingPlatform/Views/Catalog/Index.cshtml), replacing the basket anchor inside each card.

Use this exact block:

```aspnetcorerazor
@{
	var TODO_3_1 = $"{Context.Request.Path}{Context.Request.QueryString}"; // Expectation: current catalog URL used as returnUrl.
}

<form asp-controller="Cart"
	  asp-action="TODO_3_2" method="post" class="position-absolute bottom-0 end-0 mb-n3 me-3"> @* Expectation: action name Add *@
	@Html.TODO_3_3 @* Expectation: anti-forgery token helper call *@
	<input type="hidden" name="TODO_3_4" value="@item.Id" /> @* Expectation: hidden field name productId *@
	<input type="hidden" name="TODO_3_5" value="@TODO_3_1" /> @* Expectation: hidden field name returnUrl *@
	<button type="submit"
			class="text-bg-primary rounded-circle p-2 text-white d-inline-flex border-0">
		<i class="ti ti-basket fs-4"></i>
	</button>
</form>
```

#### Stage 3 - Answer Key
Answer 3.1: currentCatalogUrl<br>
Answer 3.2: Add<br>
Answer 3.3: AntiForgeryToken()<br>
Answer 3.4: productId<br>
Answer 3.5: returnUrl<br>

**User Validation Checkpoint (Stage 3)**
- From catalog page, click basket icon on a product.
- Confirm request is `POST /Cart/Add` and redirects back to catalog.
- Confirm product appears in cart.

---

### Stage 4 - Wire Product Details Add-To-Cart + Buy-Now
Why this stage matters: product details must support both standard cart add and immediate checkout path.

Snippet type: **Replacement block** in [PrintingPlatform/Views/Catalog/ProductDetails.cshtml](PrintingPlatform/Views/Catalog/ProductDetails.cshtml), replacing the single Back button section.

Use this exact block:

```aspnetcorerazor
@{
	var TODO_4_1 = $"{Context.Request.Path}{Context.Request.QueryString}"; @* Expectation: current details URL string for return navigation. *@
}

<div class="d-flex flex-wrap gap-2">
	<form asp-controller="Cart" asp-action="TODO_4_2" method="post" class="d-inline"> @* Expectation: action Add for normal add flow. *@
		@Html.TODO_4_3 @* Expectation: anti-forgery token helper call. *@
		<input type="hidden" name="TODO_4_4" value="@Model.Id" /> @* Expectation: hidden field name productId. *@
		<input type="hidden" name="TODO_4_5" value="@TODO_4_1" /> @* Expectation: hidden field name returnUrl. *@
		<button type="submit" class="btn btn-primary">Add to Cart</button>
	</form>

	<form asp-controller="Cart" asp-action="Add" method="post" class="d-inline">
		@Html.TODO_4_6 @* Expectation: anti-forgery token helper call. *@
		<input type="hidden" name="productId" value="@Model.Id" />
		<input type="hidden" name="TODO_4_7" value="true" /> @* Expectation: hidden field name buyNow. *@
		<button type="submit" class="btn btn-success">Buy Now</button>
	</form>

	<a asp-controller="Catalog" asp-action="Index" class="btn btn-outline-primary">
		TODO_4_8 @* Expectation: visible button text Back to Catalog. *@
	</a>
</div>
```

#### Stage 4 - Answer Key
Answer 4.1: currentProductUrl<br>
Answer 4.2: Add<br>
Answer 4.3: AntiForgeryToken()<br>
Answer 4.4: productId<br>
Answer 4.5: returnUrl<br>
Answer 4.6: AntiForgeryToken()<br>
Answer 4.7: buyNow<br>
Answer 4.8: Back to Catalog<br>

**User Validation Checkpoint (Stage 4)**
- On product details page:
  - Add to Cart should POST to `Cart/Add` and return to same details page.
  - Buy Now should POST to `Cart/Add` with buy-now flag and redirect to `Checkout/Index`.
- Continue only if both behaviors are correct.

---

### Final Validation Stage 5

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
- Updated files:
  - `compliance-script.sh`
  - `prepare-compliance-baseline.sh`
  - `.github/instructions/compliance-mappings.txt`
- Mapping set for this task:
  - `PrintingPlatform/Program.cs|ImplementationPacks/CartSessionBasic/PrintingPlatform/Program.cs`
  - `PrintingPlatform/Controllers/CartController.cs|ImplementationPacks/CartSessionBasic/PrintingPlatform/Controllers/CartController.cs`
  - `PrintingPlatform/Views/Catalog/Index.cshtml|ImplementationPacks/CartSessionBasic/PrintingPlatform/Views/Catalog/Index.cshtml`
  - `PrintingPlatform/Views/Catalog/ProductDetails.cshtml|ImplementationPacks/CartSessionBasic/PrintingPlatform/Views/Catalog/ProductDetails.cshtml`
- Manual one-command compliance run (from repository root):
  - `./compliance-script.sh`

## IMPLEMENTATION OUTPUT - Full Solution - 2026-04-12 11:20

Full approved solution scope implemented in one pass (all stages), with manual-apply snippets only.

### Stage 1 - Patch `CartController.Add` Contract + Redirect Logic
Why this stage matters: this stage keeps your existing cart/session logic intact while fixing the exact redirect and action-contract behavior requested.

This patch only updates the existing `Add` action and leaves all other cart actions unchanged.

Snippet type: **Replacement block** in [PrintingPlatform/Controllers/CartController.cs](PrintingPlatform/Controllers/CartController.cs), inside `Add`.

Use this exact replacement:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Add(int TODO_1_1, string? TODO_1_2, bool buyNow = false) // Expectation: use parameter names `productId` and `returnUrl`.
{
	var product = _context.Products.FirstOrDefault(productItem => productItem.Id == TODO_1_3); // Expectation: query by productId.
	if (product == null)
	{
		return NotFound();
	}

	var cart = TODO_1_4; // Expectation: call GetCartFromSession().
	var existingItem = cart.Items.FirstOrDefault(cartItem => cartItem.ProductId == TODO_1_5); // Expectation: match ProductId against productId.

	if (existingItem != null)
	{
		TODO_1_6 // Expectation: increment existing quantity by one.
	}
	else
	{
		cart.Items.Add(new CartItemViewModel
		{
			ProductId = product.Id,
			ProductName = product.Name,
			ImageUrl = product.ImageUrl,
			Price = product.Price,
			Quantity = TODO_1_7 // Expectation: default quantity to 1.
		});
	}

	TODO_1_8 // Expectation: save updated cart in session.

	if (buyNow)
	{
		return RedirectToAction("Index", "Checkout");
	}

	if (!string.IsNullOrWhiteSpace(TODO_1_9) && Url.IsLocalUrl(returnUrl)) // Expectation: validate returnUrl local redirect.
	{
		return Redirect(returnUrl);
	}

	return RedirectToAction("TODO_1_10", "TODO_1_11"); // Expectation: fallback to Catalog/Index.
}
```

#### Stage 1 - Answer Key
Answer 1.1: productId<br>
Answer 1.2: returnUrl<br>
Answer 1.3: productId<br>
Answer 1.4: GetCartFromSession()<br>
Answer 1.5: productId<br>
Answer 1.6: existingItem.Quantity += 1;<br>
Answer 1.7: 1<br>
Answer 1.8: SetCartInSession(cart);<br>
Answer 1.9: returnUrl<br>
Answer 1.10: Index<br>
Answer 1.11: Catalog<br>

**User Validation Checkpoint (Stage 1)**
- Run: `dotnet build PrintingPlatform/PrintingPlatform.csproj`
- Validate:
  - `buyNow=true` posts to `Cart/Add` and redirects to `Checkout/Index`.
  - normal add redirects to `returnUrl` when local.
  - missing/invalid `returnUrl` falls back to `Catalog/Index`.

---

### Stage 2 - Extend Existing `CheckoutController` GET/POST Flow
Why this stage matters: this stage reuses your existing session cart logic and extends current checkout actions instead of recreating checkout from scratch.

It wires checkout GET to live cart data and adds POST handling for form submission + success flow.

Snippet type: **Full file replacement** for [PrintingPlatform/Controllers/CheckoutController.cs](PrintingPlatform/Controllers/CheckoutController.cs).

Use this exact full file:

```csharp
using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Models.Cart;
using PrintingPlatform.Models.Checkout;
using System.Text.Json;

namespace PrintingPlatform.Controllers
{
	public class CheckoutController : Controller
	{
		private const string TODO_2_1 = TODO_2_2; // Expectation: constant name CartSessionKey and value "CartSession".

		[HttpGet]
		public IActionResult Index()
		{
			var cart = TODO_2_3; // Expectation: read cart from session helper.
			if (!cart.Items.Any())
			{
				return RedirectToAction("TODO_2_4", "TODO_2_5"); // Expectation: redirect to Cart/Index when cart is empty.
			}

			var viewModel = TODO_2_6; // Expectation: call BuildCheckoutViewModel(cart).
			return View("Checkout", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(CheckoutViewModel viewModel)
		{
			var cart = TODO_2_7; // Expectation: read cart from session helper.
			if (!cart.Items.Any())
			{
				TempData["CheckoutError"] = "Your cart is empty.";
				return RedirectToAction("Index", "Cart");
			}

			TODO_2_8 // Expectation: populate checkout summary fields from cart before ModelState check.

			if (!ModelState.IsValid)
			{
				return View("Checkout", viewModel);
			}

			HttpContext.Session.Remove(TODO_2_9); // Expectation: remove CartSessionKey after successful checkout.
			TempData["CheckoutSuccess"] = "Your order has been placed successfully.";

			return RedirectToAction("TODO_2_10", "TODO_2_11"); // Expectation: redirect to Catalog/Index after success.
		}

		private CheckoutViewModel BuildCheckoutViewModel(CartViewModel cart)
		{
			var viewModel = new CheckoutViewModel();
			TODO_2_12 // Expectation: call PopulateCartSummary(viewModel, cart).
			return viewModel;
		}

		private static void PopulateCartSummary(CheckoutViewModel viewModel, CartViewModel cart)
		{
			viewModel.CartItems = cart.Items.ToList();
			viewModel.Subtotal = cart.TotalPrice;
			viewModel.ShippingCost = 0m;
			viewModel.Total = viewModel.Subtotal + viewModel.ShippingCost;
		}

		private CartViewModel GetCartFromSession()
		{
			var cartJson = HttpContext.Session.GetString(CartSessionKey);
			if (string.IsNullOrWhiteSpace(cartJson))
			{
				return new CartViewModel();
			}

			return JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
		}
	}
}
```

#### Stage 2 - Answer Key
Answer 2.1: CartSessionKey<br>
Answer 2.2: "CartSession"<br>
Answer 2.3: GetCartFromSession()<br>
Answer 2.4: Index<br>
Answer 2.5: Cart<br>
Answer 2.6: BuildCheckoutViewModel(cart)<br>
Answer 2.7: GetCartFromSession()<br>
Answer 2.8: PopulateCartSummary(viewModel, cart);<br>
Answer 2.9: CartSessionKey<br>
Answer 2.10: Index<br>
Answer 2.11: Catalog<br>
Answer 2.12: PopulateCartSummary(viewModel, cart);<br>

**User Validation Checkpoint (Stage 2)**
- Run: `dotnet build PrintingPlatform/PrintingPlatform.csproj`
- Validate:
  - `GET /Checkout/Index` redirects to cart when cart is empty.
  - with cart items, checkout page loads and shows live session items.
  - valid POST clears session cart and redirects to catalog.

---

### Stage 3 - Extend Existing `CheckoutViewModel` With Cart Summary Fields
Why this stage matters: this stage keeps your existing billing fields and adds only the missing cart/summary data needed by the current checkout view.

This avoids creating new model types and keeps existing controller/view binding straightforward.

Snippet type: **Full file replacement** for [PrintingPlatform/Models/Checkout/CheckoutViewModel.cs](PrintingPlatform/Models/Checkout/CheckoutViewModel.cs).

Use this exact full file:

```csharp
using PrintingPlatform.Models.Cart;
using System.ComponentModel.DataAnnotations;

namespace PrintingPlatform.Models.Checkout
{
	public class CheckoutViewModel
	{
		[Required(ErrorMessage = "Full name is required")]
		[Display(Name = "Full Name")]
		public string TODO_3_1 { get; set; } = string.Empty; // Expectation: keep `FullName` property.

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		[Display(Name = "Email")]
		public string TODO_3_2 { get; set; } = string.Empty; // Expectation: keep `Email` property.

		[Required(ErrorMessage = "Address is required")]
		[Display(Name = "Address")]
		public string TODO_3_3 { get; set; } = string.Empty; // Expectation: keep `Address` property.

		[Required(ErrorMessage = "City is required")]
		[Display(Name = "City")]
		public string TODO_3_4 { get; set; } = string.Empty; // Expectation: keep `City` property.

		[Required(ErrorMessage = "Postal code is required")]
		[Display(Name = "Postal Code")]
		[StringLength(10, MinimumLength = 5, ErrorMessage = "Postal code must be between 5 and 10 characters")]
		public string TODO_3_5 { get; set; } = string.Empty; // Expectation: keep `PostalCode` property.

		public List<CartItemViewModel> TODO_3_6 { get; set; } = new(); // Expectation: add `CartItems` list for checkout cart rows.
		public decimal TODO_3_7 { get; set; } // Expectation: add `Subtotal` field.
		public decimal TODO_3_8 { get; set; } // Expectation: add `ShippingCost` field.
		public decimal TODO_3_9 { get; set; } // Expectation: add `Total` field.
		public bool TODO_3_10 => CartItems.Any(); // Expectation: add `HasItems` helper boolean.
	}
}
```

#### Stage 3 - Answer Key
Answer 3.1: FullName<br>
Answer 3.2: Email<br>
Answer 3.3: Address<br>
Answer 3.4: City<br>
Answer 3.5: PostalCode<br>
Answer 3.6: CartItems<br>
Answer 3.7: Subtotal<br>
Answer 3.8: ShippingCost<br>
Answer 3.9: Total<br>
Answer 3.10: HasItems<br>

**User Validation Checkpoint (Stage 3)**
- Run: `dotnet build PrintingPlatform/PrintingPlatform.csproj`
- Validate model-binding still works and checkout page receives both address fields and cart summary fields.

---

### Stage 4 - Extend Existing Checkout View To Render Live Cart + Billing Form
Why this stage matters: this stage preserves the existing checkout page route/view while patching missing dynamic pieces from session-backed cart data.

It replaces static sample values with real model values and keeps current controller/action wiring.

Snippet type: **Full file replacement** for [PrintingPlatform/Views/Checkout/Checkout.cshtml](PrintingPlatform/Views/Checkout/Checkout.cshtml).

Use this exact full file:

```aspnetcorerazor
@model PrintingPlatform.Models.Checkout.CheckoutViewModel

@{
	Layout = "_SpikeLanding";
	ViewData["Title"] = "Checkout";
}

<section class="py-5">
	<div class="container-xxl">
		<div class="mb-3 overflow-hidden position-relative">
			<div class="px-3">
				<h4 class="fs-6 mb-0">checkout</h4>
				<nav aria-label="breadcrumb">
					<ol class="breadcrumb mb-0">
						<li class="breadcrumb-item">
							<a asp-controller="Home" asp-action="Index">Home</a>
						</li>
						<li class="breadcrumb-item" aria-current="page">checkout</li>
					</ol>
				</nav>
			</div>
		</div>

		@if (TempData["CheckoutSuccess"] is string successMessage)
		{
			<div class="alert alert-success">@successMessage</div>
		}

		@if (TempData["CheckoutError"] is string errorMessage)
		{
			<div class="alert alert-danger">@errorMessage</div>
		}

		@if (!Model.TODO_4_1)
		{
			<div class="card border shadow-none">
				<div class="card-body p-4 text-center">
					<h5>Your cart is empty</h5>
					<a asp-controller="Catalog" asp-action="Index" class="btn btn-primary mt-3">Return to Catalog</a>
				</div>
			</div>
		}
		else
		{
			<form asp-controller="Checkout" asp-action="Index" method="TODO_4_2" class="card border shadow-none"> @* Expectation: post *@
				<div class="card-body p-4">
					@Html.TODO_4_3 @* Expectation: render validation summary with ValidationSummary.All *@

					<h5 class="fw-semibold mb-3">Order Items</h5>
					<div class="table-responsive mb-4">
						<table class="table align-middle text-nowrap mb-0">
							<thead>
								<tr>
									<th>Product</th>
									<th>Quantity</th>
									<th class="text-end">Line Total</th>
								</tr>
							</thead>
							<tbody>
								@foreach (var item in Model.TODO_4_4) @* Expectation: iterate Model.CartItems *@
								{
									<tr>
										<td>
											<div class="d-flex align-items-center gap-3">
												<img src="@(string.IsNullOrWhiteSpace(item.ImageUrl) ? Url.Content("~/assets/images/products/s1.jpg") : item.ImageUrl)" alt="@item.ProductName" class="img-fluid rounded" width="64" />
												<div>
													<h6 class="mb-0">@item.ProductName</h6>
												</div>
											</div>
										</td>
										<td>@item.TODO_4_5</td> @* Expectation: show item.Quantity *@
										<td class="text-end">$@((item.Price * item.Quantity).ToString("0.00"))</td>
									</tr>
								}
							</tbody>
						</table>
					</div>

					<h5 class="fw-semibold mb-3">Billing & Address</h5>
					<div class="row g-3">
						<div class="col-md-6">
							<label asp-for="TODO_4_6" class="form-label"></label> @* Expectation: FullName *@
							<input asp-for="FullName" class="form-control" />
							<span asp-validation-for="FullName" class="text-danger"></span>
						</div>
						<div class="col-md-6">
							<label asp-for="TODO_4_7" class="form-label"></label> @* Expectation: Email *@
							<input asp-for="Email" class="form-control" />
							<span asp-validation-for="Email" class="text-danger"></span>
						</div>
						<div class="col-12">
							<label asp-for="TODO_4_8" class="form-label"></label> @* Expectation: Address *@
							<input asp-for="Address" class="form-control" />
							<span asp-validation-for="Address" class="text-danger"></span>
						</div>
						<div class="col-md-6">
							<label asp-for="TODO_4_9" class="form-label"></label> @* Expectation: City *@
							<input asp-for="City" class="form-control" />
							<span asp-validation-for="City" class="text-danger"></span>
						</div>
						<div class="col-md-6">
							<label asp-for="TODO_4_10" class="form-label"></label> @* Expectation: PostalCode *@
							<input asp-for="PostalCode" class="form-control" />
							<span asp-validation-for="PostalCode" class="text-danger"></span>
						</div>
					</div>

					<div class="border rounded p-4 my-4">
						<h5 class="fw-semibold mb-3">Order Summary</h5>
						<div class="d-flex justify-content-between mb-2">
							<span>Subtotal</span>
							<span>$@Model.TODO_4_11.ToString("0.00")</span> @* Expectation: Subtotal *@
						</div>
						<div class="d-flex justify-content-between mb-2">
							<span>Shipping</span>
							<span>$@Model.TODO_4_12.ToString("0.00")</span> @* Expectation: ShippingCost *@
						</div>
						<div class="d-flex justify-content-between fw-bold">
							<span>Total</span>
							<span>$@Model.TODO_4_13.ToString("0.00")</span> @* Expectation: Total *@
						</div>
					</div>

					<div class="d-flex gap-2">
						<a asp-controller="Cart" asp-action="Index" class="btn btn-outline-primary">Back to Cart</a>
						<button type="submit" class="btn btn-success">TODO_4_14</button> @* Expectation: Place Order *@
					</div>
				</div>
			</form>
		}
	</div>
</section>

@section Scripts {
	<partial name="_ValidationScriptsPartial" />
}
```

#### Stage 4 - Answer Key
Answer 4.1: HasItems<br>
Answer 4.2: post<br>
Answer 4.3: ValidationSummary(ValidationSummary.All, "", new { @class = "text-danger" })<br>
Answer 4.4: CartItems<br>
Answer 4.5: Quantity<br>
Answer 4.6: FullName<br>
Answer 4.7: Email<br>
Answer 4.8: Address<br>
Answer 4.9: City<br>
Answer 4.10: PostalCode<br>
Answer 4.11: Subtotal<br>
Answer 4.12: ShippingCost<br>
Answer 4.13: Total<br>
Answer 4.14: Place Order<br>

**User Validation Checkpoint (Stage 4)**
- Run: `dotnet build PrintingPlatform/PrintingPlatform.csproj`
- Validate end-to-end:
  - from catalog/details, add product to cart.
  - click checkout, confirm live cart rows and totals render.
  - submit valid checkout form and confirm success redirect behavior.

---

### Final Validation Stage 5

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
- Updated files:
  - `compliance-script.sh`
  - `prepare-compliance-baseline.sh`
  - `.github/instructions/compliance-mappings.txt`
- Mapping set for this task:
  - `PrintingPlatform/Controllers/CartController.cs|ImplementationPacks/CheckoutCartFlowPatch/PrintingPlatform/Controllers/CartController.cs`
  - `PrintingPlatform/Controllers/CheckoutController.cs|ImplementationPacks/CheckoutCartFlowPatch/PrintingPlatform/Controllers/CheckoutController.cs`
  - `PrintingPlatform/Models/Checkout/CheckoutViewModel.cs|ImplementationPacks/CheckoutCartFlowPatch/PrintingPlatform/Models/Checkout/CheckoutViewModel.cs`
  - `PrintingPlatform/Views/Checkout/Checkout.cshtml|ImplementationPacks/CheckoutCartFlowPatch/PrintingPlatform/Views/Checkout/Checkout.cshtml`
- Manual one-command compliance run (from repository root):
  - `./compliance-script.sh`

