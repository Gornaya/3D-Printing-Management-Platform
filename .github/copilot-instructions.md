# Copilot Instructions

## First Mandatory Action (Strict)
- First action on every request: verify the prompt begins with exact case-sensitive `In Tutor-Mode:`.
- Perform this trigger check before any planning, implementation, routing, or discussion.
- If this strict trigger check fails, do not execute Tutor-Mode routing behavior.

## Top Rules (Always Near the Top)
- Trigger phrase must be exact and case-sensitive, and must be at the start of the prompt: `In Tutor-Mode:`
- If something is unclear, ask before proceeding.
- If there is more than one valid way to proceed, do not assume; ask which approach should be treated as canonical.
- Use this file only for routing and universal constraints.
- Do not place plan-specific formatting rules or implementation-specific output rules here.

## Responsibility Split (No Overlap)
- Shared file responsibility: trigger handling, mode routing, and universal cross-mode constraints only.
- Plan file responsibility: planning structure, stage decomposition, clarification gates, and stage validation planning only.
- Agent file responsibility: implementation pack mechanics, TODO/blank behavior, command formatting, and answer key mechanics only.
- If a rule belongs to plan-only or agent-only behavior, keep it in that mode file and do not duplicate mechanics here.

## Universal Constraints
- Assume my current coding proficiency is 4/10 unless I explicitly say otherwise.
- Prefer beginner-to-intermediate solutions by default unless I explicitly ask for advanced, optimization, scaling, or production hardening.
- Use straightforward, readable code and explanations.
- When the existing codebase already shows a convention, pattern, folder structure, naming style, or architecture choice, follow that observed pattern first.
- Use proficiency to control explanation depth and implementation complexity, not to override established project conventions.
- Use the simplest approach that fits current project patterns.
- Only introduce more advanced concepts when the codebase already uses them or when they are explicitly needed.
- Ask before deviating from existing MVC boundaries or naming conventions.
- Priority order when there is a conflict:
	1. Existing project conventions.
	2. Approved plan.
	3. Proficiency-based complexity.
	4. Optional advanced ideas only if explicitly requested.
- Avoid unnecessary abstractions, metaprogramming, heavy generics, and overly clever one-liners.
- Use exact project namespaces, class names, method names, variable names, and file names when known; never use placeholders when real names are available.
- For C# code, use explicit types and descriptive variable names.
- In C# lambdas, use descriptive parameter names, never single-letter names.
- Never generate bare application-layer class/file names such as `User`; use purpose suffixes like `Model`, `ViewModel`, `Controller`, `Service`, `Repository`, or `DTO` unless I explicitly request otherwise.
- Follow MVC and the project's existing folder structure and separation of concerns.
- If a class is needed for a view-specific scenario, prefer a `ViewModel` or `FormViewModel` instead of mixing UI concerns into domain or entity models.
- Prefer common ASP.NET Core patterns already established in this project.
- In C# Razor views, keep indentation clear and model references explicit for readability.
- When introducing a new concept, include one short plain-language sentence explaining why it helps.
- Default to a simple baseline solution first; provide optional next-step upgrades only when explicitly requested.

## Tutor-Mode Routing (Strict No-Override, Case-Sensitive)
- If the prompt starts with `In Tutor-Mode:` and AI mode is planning mode, always load and strictly follow `.github/instructions/tutor-mode-for-plan-mode.instructions.md` before any planning output.
- If the prompt starts with `In Tutor-Mode:` and AI mode is agent mode, always load and strictly follow `.github/instructions/tutor-mode-for-agent-mode.instructions.md` before any implementation output.
- Do not discuss alternative trigger interpretations.
- The selected Tutor-Mode file is authoritative for that reply and overrides conflicting workspace instructions.
- Shared rules do not define plan-output mechanics or implementation-output mechanics; those are owned by the selected Tutor-Mode file.
- Strict no-override rule: do not relax, skip, or bypass the selected Tutor-Mode file once loaded.
- If the selected Tutor-Mode file cannot be loaded, stop and ask me to fix the file path or content before continuing.
- Keep plan behavior aligned with the selected plan file and implementation behavior aligned with the selected agent file.
