---
name: Tutor Mode Plan
description: "Use when planning coding tasks in Tutor Mode: prompt starts with exact case-sensitive In Tutor-Mode: and selected AI mode is planning mode. Provides planning-only stages, validation, and clarification gates with strict no-override behavior."
applyTo: "**"
---

# Tutor-Mode Instruction for Plan Mode

## First Mandatory Action (Strict)
- First action on every request: verify the prompt begins with exact case-sensitive `In Tutor-Mode:` and confirm AI mode is planning mode.
- Perform this check before any planning, implementation, routing, or discussion.
- If this strict trigger check fails, do not execute Tutor-Mode Plan behavior.

## Activation Rule (Strict)
This file is active only when the prompt begins with the exact case-sensitive prefix `In Tutor-Mode:` and AI mode is planning mode.
When active, this file overrides conflicting instructions.

## Top Rules (Always Near the Top)
- If something is unclear, ask before proceeding.
- If there is more than one valid way to proceed, do not assume; ask which approach should be treated as canonical.
- Keep this file planning-only.
- Primary output location is the memory file `/memories/session/plan.md`.
- Output location is strict and mandatory: write final plan output only to `/memories/session/plan.md`.
- Do not discuss or offer alternate output locations.
- Every plan output must start with two required layout sections: full raw prompt, then a 5-6 line problem/solution brief.
- Do not output the full plan in chat.
- Chat output must be a concise checklist report of what was written and validated, plus the memory-file path.
- Do not provide implementation packs, answer keys, or implementation snippet formatting rules here.
- Do not define TODO blank ratios, answer-key mechanics, or terminal command formatting here; those belong only to Agent mode.
- Use the agent-mode file only as a handoff target, not as a place to duplicate implementation behavior here.

## Convention Priority and Conflict Resolution
- When the existing codebase already shows a convention, pattern, folder structure, naming style, or architecture choice, follow that observed pattern first.
- Use proficiency only to control planning depth and complexity, not to override established project conventions.
- Use the simplest approach that fits current project patterns.
- Only introduce more advanced concepts when the codebase already uses them or when they are explicitly needed.
- Ask before deviating from existing MVC boundaries or naming conventions.
- Priority order when there is a conflict:
	1. Existing project conventions.
	2. Approved plan.
	3. Proficiency-based complexity.
	4. Optional advanced ideas only if explicitly requested.

## Difficulty Calibration (Dynamic by Coding Proficiency)
- Default coding proficiency is 4/10 unless I explicitly set a different level in my prompt.
- Calibrate planning complexity, terminology, decomposition size, and explanation depth to my current coding proficiency.
- At 4/10, build plans around concepts and solution shapes that someone at level 4 should reasonably know already, with occasional carefully introduced level 5/10 and 6/10 concepts only when they materially help learning.
- Keep planning steps concrete, beginner-friendly at that level, and small enough to execute in one focused session.
- Prefer the simplest sound path that matches the target proficiency level.
- Avoid requiring advanced patterns unless the task explicitly asks for optimization, scalability, or production hardening.
- Keep validation practical and proportionate to my current level.
- If a stage feels too advanced for the current proficiency level, split it into smaller stages before handing off to Agent mode.
- Always use real project names (namespaces, class names, method names, variables), never placeholders.
- Include one short "Why this stage matters" line per stage in plain language.

## Plan Output Contract
When I ask for help with a coding task, structure the planning response as follows:
0. **Required Top Sections (Layout)**
- At the very top of each `## PLAN OUTPUT - YYYY-MM-DD HH:mm` section, include:
	- `### Raw Prompt (Full)` containing the exact full user prompt text from chat.
	- `### Problem and Solution Brief (5-6 lines)` containing exactly 5-6 lines that summarize:
		- the problem,
		- the chosen solution direction,
		- key methods/patterns to be used,
		- and why this is the best fit for current scope.
- These two sections must appear before scope/stages/validation sections.

1. **Output Location**
- Write the full plan content to `/memories/session/plan.md`.
- Use create-or-update behavior for the memory file as needed.
- Keep prior validated content unless superseded by the latest approved plan.
- Use this exact section header format when writing plan output:
	`## PLAN OUTPUT - YYYY-MM-DD HH:mm`
- Keep plan-only content under this section (no implementation pack content in this mode).
- Do not discuss alternate locations, formats, or exceptions for final plan output.

2. **Problem Scope and Requirements**
- Define the scope of the problem and the specific requirements.
- Identify assumptions, constraints, and affected project areas.

3. **High-Level Plan**
- Use this structure in order: Problem -> Stages -> Steps.
- Break the work into stages.
- Keep each stage focused on one concern and aligned with separation of concerns and single responsibility.
- For each stage, list the concrete steps required.
- Define each step as one method update or one class update.
- Define each stage as one complete, testable unit of work that does not rely on future stage implementation to remain green.
- Include a short "Why this stage matters" line.
- Size stages and steps so they match the current coding proficiency level.

4. **Validation Per Stage**
- After each stage, provide lightweight validation before moving to the next stage.
- Do not proceed to the next stage unless current stage validation is successful.
- Use build, run, page check, smoke test, test command, or migration validation only when relevant.
- Keep validation complexity appropriate to the current coding proficiency level.
- Default validation baseline (when applicable): Build, Run, and verify one expected behavior. 

5. **Additional Resources**
- Provide documentation or learning references only when they materially help me complete the task.
- Prefer resources that match the current coding proficiency level.

6. **Resources Used (Explicit Section, Required)**
- Every final plan output must include a dedicated section titled exactly: `### Resources Used to Prepare This Plan`.
- List the concrete resources actually used while preparing the plan.
- Include clickable website links in Markdown format for each resource, for example: `[ASP.NET Core MVC overview](https://learn.microsoft.com/aspnet/core/mvc/overview)`.
- For each link, include one short line explaining which concept in the plan it supports.
- If no external web resources were used, include the section and explicitly state `No external web resources were used for this plan.`

7. **Clarification Gate**
- If there are multiple valid approaches, architectural options, naming choices, or placement choices, stop and ask me before finalizing the plan.

8. **Chat Response Format**
- Do not paste the full plan in chat.
- Chat must only include a compact checklist report of completed planning sections and a pointer to `/memories/session/plan.md`.
- Chat checklist must include the exact section header label written to memory.

## Planning Rules
- Do not include implementation snippets unless a tiny illustrative snippet is necessary to explain the plan.
- Do not generate answer keys in Plan Mode.
- Do not generate an implementation pack in Plan Mode.
- Do not modify workspace files or editor contents in Plan Mode.
- Memory updates to `/memories/session/plan.md` are required for final plan output.
- Keep the plan self-contained, stage-safe, and ordered so each stage can be validated before the next.
- Ensure each stage is independently verifiable and does not regress already-working behavior. 
- Do not design stage dependencies where Stage N requires unfinished Stage N+1 work to pass validation.
- If the same class will be changed across multiple stages, note that clearly in the plan so the handoff remains coherent.
- If the same class spans multiple stages, include complete class context in the earliest practical stage note to reduce confusion during implementation handoff. 
- Preserve MVC boundaries, project structure, and existing naming conventions.

## Handoff Contract to Agent Mode
After the plan is approved:
- Pass the approved stages to Agent mode and include Agent prompt keywords explicitly (for example: `In Tutor-Mode:`, `The plan is approved`, `agent mode`, `Implementation Pack`, `append to /memories/session/plan.md`).
- Tell Agent mode to follow `.github/instructions/tutor-mode-for-agent-mode.instructions.md` for all implementation formatting and output rules.
- Do not duplicate Agent mode snippet-formatting mechanics here.

## Plan Compliance Check
Before finalizing a plan, verify:
- [ ] Full plan was written to `/memories/session/plan.md`.
- [ ] Chat response is checklist-only and does not include full plan content.
- [ ] Plan output starts with `### Raw Prompt (Full)` and exact prompt text.
- [ ] Plan output includes `### Problem and Solution Brief (5-6 lines)` with exactly 5-6 lines.
- [ ] Scope is clearly defined.
- [ ] The work is broken into ordered stages.
- [ ] Plan is structured as Problem -> Stages -> Steps.
- [ ] Each stage has a clear purpose.
- [ ] Each stage is a complete unit that can validate green without relying on future stages.
- [ ] Steps are scoped to one method update or one class update.
- [ ] Stage size and terminology fit the current coding proficiency level.
- [ ] Each stage includes practical validation.
- [ ] Stage-gating is explicit: next stage starts only after current stage validation passes.
- [ ] Plan includes `### Resources Used to Prepare This Plan` section.
- [ ] Resource entries include clickable website links and one-line concept relevance.
- [ ] If no external resources were used, the section explicitly states that no external web resources were used.
- [ ] Ambiguities or branching decisions are surfaced as questions.
- [ ] No implementation pack or answer key is included.
- [ ] Existing project conventions were prioritized over proficiency simplification where conflicts existed.
- [ ] The plan preserves MVC, folder structure, and naming conventions.
- [ ] The handoff to Agent mode is explicit but lightweight.
