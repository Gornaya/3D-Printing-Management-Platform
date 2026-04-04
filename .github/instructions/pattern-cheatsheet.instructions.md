---
name: Repository Pattern Cheatsheet
description: "Use for implementation tasks and general project architecture questions in this repository. Prioritize this for Tutor-Mode agent implementation and CRUD feature work."
applyTo: "WarehouseManagementSoftware/**/*.{cs,cshtml,js}"
---

# Assistant Pattern Cheatsheet
- Date noted: 2026-04-03
- Purpose: fast baseline for implementing new feature flows consistently in this repository.

## Invocation Scope
- Use this cheatsheet by default for:
  - Tutor-Mode agent implementation tasks (`In Tutor-Mode:` + agent mode)
  - General project questions about architecture, feature flow, naming, folder placement, and patterns
- Do not use this cheatsheet as the governing source for Tutor-Mode plan output formatting.

## Conflict Priority (Solution-Bound)
- When multiple instructions apply, use this order:
  1. `.github/instructions/tutor-mode-for-agent-mode.instructions.md` when Tutor-Mode agent implementation is active
  2. `.github/copilot-instructions.md` universal constraints
  3. This cheatsheet for repository implementation conventions
- If conflict remains, prefer existing codebase conventions over invention.

## Naming and Structure
- Keep MVC boundaries: Controllers -> Services -> Shared DTOs -> Entities; Views use Form/Page/Row ViewModels.
- Contracts in `Interfaces/` as `I{Entity}Service`.
- Service implementations in `Services/Account/` (existing account pattern) or feature-consistent folder if already established.
- DTO naming pattern in `Shared/`:
  - `Create{Entity}DTO`
  - `{Entity}DTO`
  - `Edit{Entity}DTO`
  - `Update{Entity}DTO`
- ViewModel naming pattern:
  - `Create{Entity}FormViewModel`
  - `Edit{Entity}FormViewModel`
  - `{Entity}RowViewModel`
  - `{Entity}sPageViewModel`
- Mapping extensions in `Services/.../{Entity}MappingExtensions.cs`.

## Action Pattern (List + Create + Edit + Delete)
- LIST action pattern:
  - set `ViewData["Title"]`
  - `SetLayoutState(...)`
  - sanitize page: `page < 1 ? 1 : page`
  - pageSize = 10 for management tables
  - call service paged method returning `(rows, totalCount)`
  - return page view model with pagination helpers
- CREATE GET:
  - set title/layout
  - set `ViewBag.ReturnPage`
  - return new create form model
- CREATE POST:
  - anti-forgery + ModelState check
  - AJAX detection via `Request.Headers["X-Requested-With"] == "XMLHttpRequest"`
  - return JSON `{ success, message, errors?, redirectUrl? }` for AJAX
  - return view/redirect with TempData for non-AJAX
- EDIT GET/POST:
  - fetch editable DTO by selected id
  - map DTO -> edit form
  - update via `Update{Entity}DTO`
- DELETE POST:
  - selected id + current page
  - service delete result -> TempData message -> redirect

## UI + Script Pattern
- Data table conventions:
  - Bootstrap striped/bordered/hover table
  - selectable row with `data-*` attributes for id/name/protection flags
  - pagination with `Previous/Next` and numeric pages
- Action panel conventions in shared layout:
  - Add action is route link preserving current page query
  - Edit/Delete typically button-driven from selected table row
- JavaScript conventions in `wwwroot/js/site.js`:
  - page-scoped IIFE blocks
  - shared row action helper (`createEntityTableActionFlow` style)
  - create-form AJAX block with validation clearing + server error rendering + success modal + redirect

## EF/Data Pattern
- Add entity under `Data/Entities/`.
- Register DbSet in `WMSContext`.
- Add explicit fluent config in `OnModelCreating` for:
  - keys
  - required fields / max lengths / precision
  - indexes (unique where needed)
- Migration naming style:
  - timestamped migration files in project history
  - descriptive stage/task names used by team workflows

## Flowchart (Shorthand)
- Add Feature Flow:
  - Requirement -> match existing entity pattern -> define Entity/DbSet/config -> migration
  - Interface/DTOs/ViewModels -> Mapping extensions -> Service methods
  - Controller LIST/CREATE/EDIT/DELETE -> Views -> action panel wiring -> site.js block
  - Build -> smoke test list/create/edit/delete -> run repo test scripts
- Create Action Flow:
  - Form submit -> client validation -> AJAX POST -> server ModelState/service checks
  - success -> modal -> redirect back to paged list
  - failure -> field/summary errors rendered

## Validation Commands (Default)
- Run from repository root:
  - `dotnet build WarehouseManagementSoftware/WarehouseManagementSoftware.csproj`
  - `./run-all-tests.sh`
- Run from `WarehouseManagementSoftware/` when feature touches web app behavior:
  - `./run-all-tests.sh`
  - smoke-check target pages for list/create/edit/delete flow

## When Not To Use
- Do not apply this cheatsheet for:
  - Tutor-Mode planning output mechanics (Plan Mode formatting/sections)
  - Non-project writing tasks (for example interview prep documents outside the repo)
  - Pure test-log summarization with no implementation/design decision

## Guardrails
- Prefer consistency over novelty unless user asks for architectural change.
- Keep Add/Edit/Delete UX aligned with Users/Roles/Permissions baseline.
- Preserve existing route naming, pagination behavior, and TempData messaging style.
