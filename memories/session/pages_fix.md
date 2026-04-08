## IMPLEMENTATION OUTPUT - Full Solution - 2026-04-07 10:40

Implementing the full approved solution for Catalog, Pricing, and Contact page rendering in one pass, while preserving the existing MVC structure.

### Stage 1 - Route and Controller Validation (No structural changes)
Why this stage matters: it confirms the endpoint chain is correct before touching view/layout files.

Task 1.1 - Validation command block (run from repository root)
Type: command block

```bash
dotnet build PrintingPlatform/PrintingPlatform.csproj
```

Task 1.2 - Manual route checks
Type: browser validation checklist
- Open `/Catalog`
- Open `/Pricing`
- Open `/Contact`
- Confirm each action resolves to `Index()` in its controller.

User Validation Checkpoint (Stage 1):
- Confirm build succeeds and all three routes return HTTP 200 before applying Stage 2.

#### Stage 1 Answer Key
Answer 1.1: dotnet build PrintingPlatform/PrintingPlatform.csproj  
Answer 1.2: /Catalog, /Pricing, /Contact each resolve to the corresponding `Index()` action and return 200.  

---

### Stage 2 - Shared Layout and Navigation Hardening
Why this stage matters: shared layout/partial issues break all landing-style pages at once.

Task 2.1 - Update header logo links to MVC tag helpers
Type: replacement block in existing file
File: `PrintingPlatform/Views/Shared/_Header.cshtml`
Replace both logo anchor tags.

```cshtml
<a asp-controller=TODO_2_1 class="text-nowrap logo-img"> @* Expectation: set Home as logo target controller *@
	<img src="~/assets/images/logos/logo-light.svg" class="dark-logo" alt="Logo-Dark">
	<img src="~/assets/images/logos/logo-dark.svg" class="light-logo" alt="Logo-light">
</a>

<a asp-controller=TODO_2_2 class="text-nowrap logo-img"> @* Expectation: set Home as mobile logo target controller *@
	<img src="~/assets/images/logos/logo-light.svg" class="dark-logo" alt="Logo-Dark">
	<img src="~/assets/images/logos/logo-dark.svg" class="light-logo" alt="Logo-light">
</a>
```

Task 2.2 - Keep header partial inside `<body>` and keep `@RenderBody()` in main content area
Type: replacement block in existing file
File: `PrintingPlatform/Views/Shared/_SpikeLanding.cshtml`

```cshtml
<body>
	<partial name=TODO_2_3 /> @* Expectation: use shared header partial name string *@

	<div class="preloader">
		<img src="~/assets/images/logos/favicon.png" alt=TODO_2_4 class="lds-ripple img-fluid"> @* Expectation: keep loader alt text literal "loader" *@
	</div>

	<div class=TODO_2_5> @* Expectation: keep wrapper class string "main-wrapper overflow-hidden" *@
		<!-- existing landing sections stay unchanged -->
		<section>
			@RenderBody()
		</section>
	</div>

	<partial name=TODO_2_6 /> @* Expectation: use shared footer partial name string *@

	<script src=TODO_2_7></script> @* Expectation: keep vendor bundle path under ~/assets/js/vendor.min.js *@
	<script src=TODO_2_8></script> @* Expectation: keep bootstrap bundle under ~/assets/libs/bootstrap/dist/js/bootstrap.bundle.min.js *@
	<script src=TODO_2_9></script> @* Expectation: keep simplebar path under ~/assets/libs/simplebar/dist/simplebar.min.js *@
	<script src=TODO_2_10></script> @* Expectation: set iconify CDN URL used by shared landing layout *@
	<script src=TODO_2_11></script> @* Expectation: keep owl carousel path under ~/assets/libs/owl.carousel/dist/owl.carousel.min.js *@
	<script src=TODO_2_12></script> @* Expectation: keep homepage script path under ~/assets/js/frontend-landingpage/homepage.js *@
</body>
```

User Validation Checkpoint (Stage 2):
- Confirm header renders once and footer renders once on `/Pricing` and `/Contact` before applying Stage 3.

#### Stage 2 Answer Key
Answer 2.1: "Home" asp-action="Index"  
Answer 2.2: "Home" asp-action="Index"  
Answer 2.3: "_Header"  
Answer 2.4: "loader"  
Answer 2.5: "main-wrapper overflow-hidden"  
Answer 2.6: "_Footer"  
Answer 2.7: "~/assets/js/vendor.min.js"  
Answer 2.8: "~/assets/libs/bootstrap/dist/js/bootstrap.bundle.min.js"  
Answer 2.9: "~/assets/libs/simplebar/dist/simplebar.min.js"  
Answer 2.10: "https://cdn.jsdelivr.net/npm/iconify-icon@1.0.8/dist/iconify-icon.min.js"  
Answer 2.11: "~/assets/libs/owl.carousel/dist/owl.carousel.min.js"  
Answer 2.12: "~/assets/js/frontend-landingpage/homepage.js"  

---

### Stage 3 - Pricing View Normalize to Layout-Body Content
Why this stage matters: it removes nested full-document markup that breaks render composition.

Task 3.1 - Replace `Pricing/Index.cshtml` with layout-body-only page content
Type: full file replacement
File: `PrintingPlatform/Views/Pricing/Index.cshtml`

Before (remove): duplicate `<!DOCTYPE html>`, `<html>`, `<head>`, `<body>`, `<partial name="_Header" />`, `<partial name="_Footer" />`, and page-local script imports.

After (apply):
```cshtml
@{
		Layout = TODO_3_1; @* Expectation: set shared landing layout string *_
		ViewData[TODO_3_2] = TODO_3_3; @* Expectation: set page title key and value for Pricing *@
}

<section class=TODO_3_4> @* Expectation: keep pricing banner classes from existing view *@
	<div class="container-fluid">
		<div class="d-flex justify-content-between flex-md-nowrap flex-wrap">
			<h2 class="fs-16 fw-bolder mb-0">Choose Your Plan</h2>
			<div class="d-flex align-items-center gap-6">
				<a asp-controller=TODO_3_5 asp-action=TODO_3_6 class="text-muted fw-bolder link-primary fs-3 text-uppercase"> @* Expectation: route breadcrumb "Spike" to Home/Index *@
					Spike
				</a>
				<iconify-icon icon="solar:alt-arrow-right-outline" class="fs-5 text-muted"></iconify-icon>
				<a href="#" class="text-primary link-primary fw-bolder fs-3 text-uppercase">Pricing Page</a>
			</div>
		</div>
	</div>
</section>

<section class=TODO_3_7> @* Expectation: keep pricing section spacing classes from existing view *@
	<!-- Keep existing pricing cards content exactly as-is from the current file -->
</section>

<section class=TODO_3_8> @* Expectation: keep focus CTA section classes from existing view *@
	<!-- Keep existing focus CTA content exactly as-is from the current file -->
</section>
```

User Validation Checkpoint (Stage 3):
- Open `/Pricing` and verify page renders without duplicate `<html>/<body>` structure and without duplicated header/footer.

#### Stage 3 Answer Key
Answer 3.1: "_SpikeLanding";  
Answer 3.2: "Title"  
Answer 3.3: "Pricing";  
Answer 3.4: "py-5 py-lg-12 bg-primary-subtle"  
Answer 3.5: "Home"  
Answer 3.6: "Index"  
Answer 3.7: "py-5 py-md-14 py-lg-11"  
Answer 3.8: "bg-primary py-lg-11 py-5 position-relative"  

---

### Stage 4 - Contact View Normalize to Layout-Body Content
Why this stage matters: it applies the same valid Razor composition pattern to Contact.

Task 4.1 - Replace `Contact/Index.cshtml` with layout-body-only page content
Type: full file replacement
File: `PrintingPlatform/Views/Contact/Index.cshtml`

Before (remove): duplicate `<!DOCTYPE html>`, `<html>`, `<head>`, `<body>`, `<partial name="_Header" />`, `<partial name="_Footer" />`, and page-local script imports.

After (apply):
```cshtml
@{
		Layout = TODO_4_1; @* Expectation: set shared landing layout string *_
		ViewData[TODO_4_2] = TODO_4_3; @* Expectation: set page title key and value for Contact *@
}

<section class=TODO_4_4> @* Expectation: keep contact banner classes from existing view *@
	<div class="container-fluid">
		<div class="d-flex justify-content-between flex-md-nowrap flex-wrap">
			<h2 class="fs-16 fw-bolder mb-0">We’d love to hear from you</h2>
			<div class="d-flex align-items-center gap-6">
				<a asp-controller=TODO_4_5 asp-action=TODO_4_6 class="text-muted fw-bolder link-primary fs-3 text-uppercase"> @* Expectation: route breadcrumb "Spike" to Home/Index *@
					Spike
				</a>
				<iconify-icon icon="solar:alt-arrow-right-outline" class="fs-5 text-muted"></iconify-icon>
				<a href="#" class="text-primary link-primary fw-bolder fs-3 text-uppercase">Contact Us</a>
			</div>
		</div>
	</div>
</section>

<section class=TODO_4_7> @* Expectation: keep existing contact form section classes *@
	<!-- Keep existing map + contact form content exactly as-is from the current file -->
</section>

<section class=TODO_4_8> @* Expectation: keep focus CTA section classes from existing view *@
	<!-- Keep existing focus CTA content exactly as-is from the current file -->
</section>
```

User Validation Checkpoint (Stage 4):
- Open `/Contact` and verify no nested document tags and no duplicate shared chrome.

#### Stage 4 Answer Key
Answer 4.1: "_SpikeLanding";  
Answer 4.2: "Title"  
Answer 4.3: "Contact";  
Answer 4.4: "py-5 py-lg-12 bg-primary-subtle"  
Answer 4.5: "Home"  
Answer 4.6: "Index"  
Answer 4.7: "py-lg-12 py-7 bg-light-gray"  
Answer 4.8: "bg-primary py-lg-11 py-5 position-relative"  

---

### Stage 5 - Catalog View Simplify and Remove Broken Static Links
Why this stage matters: Catalog currently carries large template-only static references that cause broken navigation and unstable rendering.

Task 5.1 - Replace `Catalog/Index.cshtml` with MVC-safe body-only content
Type: full file replacement
File: `PrintingPlatform/Views/Catalog/Index.cshtml`

After (apply):
```cshtml
@{
		Layout = TODO_5_1; @* Expectation: set shared landing layout string *_
		ViewData[TODO_5_2] = TODO_5_3; @* Expectation: set page title key and value for Catalog *@
}

<section class=TODO_5_4> @* Expectation: use banner classes consistent with landing pages *@
	<div class="container-fluid">
		<div class="d-flex justify-content-between flex-md-nowrap flex-wrap">
			<h2 class="fs-16 fw-bolder mb-0">Catalog</h2>
			<div class="d-flex align-items-center gap-6">
				<a asp-controller=TODO_5_5 asp-action=TODO_5_6 class="text-muted fw-bolder link-primary fs-3 text-uppercase"> @* Expectation: route breadcrumb back to Home/Index *@
					Home
				</a>
				<iconify-icon icon="solar:alt-arrow-right-outline" class="fs-5 text-muted"></iconify-icon>
				<a href="#" class="text-primary link-primary fw-bolder fs-3 text-uppercase">Catalog</a>
			</div>
		</div>
	</div>
</section>

<section class=TODO_5_7> @* Expectation: keep section spacing for catalog cards *@
	<div class="container-fluid">
		<div class="row g-4">
			<!-- Keep or re-add catalog cards/content from your preferred existing dataset -->
		</div>
	</div>
</section>

<section class=TODO_5_8> @* Expectation: keep focus CTA section classes consistent with Pricing/Contact *@
	<!-- Reuse same CTA block structure used by Pricing and Contact pages -->
</section>
```

Task 5.2 - Post-change checks
Type: command + browser checks

```bash
dotnet build PrintingPlatform/PrintingPlatform.csproj
```

Then verify in browser dev tools network tab:
- no requests to `frontend-landingpage.html`
- no requests to `authentication-register.html`
- no requests to `~/../../npm/iconify-icon%401.0.8/dist/iconify-icon.min.js`

User Validation Checkpoint (Stage 5):
- Confirm `/Catalog`, `/Pricing`, and `/Contact` render with one header/footer and no template-HTML 404s.

#### Stage 5 Answer Key
Answer 5.1: "_SpikeLanding";  
Answer 5.2: "Title"  
Answer 5.3: "Catalog";  
Answer 5.4: "py-5 py-lg-12 bg-primary-subtle"  
Answer 5.5: "Home"  
Answer 5.6: "Index"  
Answer 5.7: "py-5 py-md-14 py-lg-11"  
Answer 5.8: "bg-primary py-lg-11 py-5 position-relative"  

---

### Final Validation Stage 6

Plan Compliance Check
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

Agent Compliance Check
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
- [ ] `prepare-compliance-baseline.sh` is auto-executed by the agent during final validation so baseline files are ready for user compliance run. (Attempted twice; execution was explicitly skipped by user.)
- [x] `compliance-script.sh` is created in the same directory as `run-all-tests.sh` and runs with one manual command.
- [x] `compliance-script.sh` performs line-by-line comparisons and enforces a 1:1 green-light rule.
- [x] `compliance-script.sh` uses explicit full expected file names/paths for every compared file.
- [x] `compliance-script.sh` self-deletes only after all comparisons pass.
- [x] Scope was not expanded without asking.

Compliance Script Invocation
- Build rules source applied: `.github/instructions/compliance-script-build.md`.
- Updated files:
	- `compliance-script.sh`
	- `prepare-compliance-baseline.sh`
	- `.github/instructions/compliance-mappings.txt`
- Baseline preparation command attempted by agent (execution was skipped by user twice):

```bash
./prepare-compliance-baseline.sh
```

- Manual compliance run command for user:

```bash
./compliance-script.sh
```
