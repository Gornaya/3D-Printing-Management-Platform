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
- During context intake, if you spot a meaningful learning opportunity, propose it as an explicit optional decision before finalizing the plan.
- Do not force a stretch decision gate on every run; ask it only when a real, task-relevant learning opportunity exists.
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
- Conditionally invoke `.github/instructions/tutor-mode-plan-helper.instructions.md` only when a teaching-opportunity gate is triggered (new concept/stretch or new pattern decision).

## Convention Priority and Conflict Resolution
- When the existing codebase already shows a convention, pattern, folder structure, naming style, or architecture choice, follow that observed pattern first.
- If an established process/pattern already exists in `.github/instructions/pattern-cheatsheet.instructions.md` and current code usage, preserve it and do not invent a replacement pattern that causes code drift.
- New pattern proposals are allowed only when the user explicitly raised coding proficiency for this task and the change is framed as an app-wide pattern update (not an isolated deviation).
- If new pattern proposal criteria are met, the plan must ask whether to postpone the current task plan and run a full feature/app-wide refactor plan to keep behavior consistent across the same assembly/workflow (for example, shared Edit-button behavior across all relevant pages).
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
- Default coding proficiency is 2/10 unless I explicitly set a different level in my prompt.
- Canonical coding proficiency contract for consistency with shared and agent files:
	- Use one proficiency value per task cycle in `p/10` format.
	- If unspecified, default to `4/10`.
	- Proficiency scale is integer `1-10`.
	- For non-JavaScript TODO blanks (applied by Agent mode), target ratio is dynamic by `p`:
		- minimum `= p * 10 - 5`
		- maximum `= p * 10 + 5`
	- JavaScript snippets are always complete with zero blanks.
	- At `10/10`, TODO markers are bare with no inline expectation notes or explanatory text.
	- Plan mode does not generate TODO snippets; this contract is included here as the single source-of-truth reference for handoff consistency.
- Calibrate planning complexity, terminology, decomposition size, and explanation depth to my current coding proficiency.
- Treat coding proficiency as the default maximum concept difficulty for plan design; do not introduce above-level concepts unless explicitly requested.
- Stretch option (explicit approval required): planning may propose concepts up to +1 or +2 levels above current proficiency only if:
	- both the baseline-level and stretch-level options are explained in plain language suitable for the current proficiency,
	- the tradeoff is justified (why stretch helps learning now), and
	- the user explicitly approves stretch before plan finalization.
- At 4/10, build plans around concepts and solution shapes that someone at level 4 should reasonably know already.
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
- List concrete learning resources the user should read to understand and implement the planned approach.
- This section is never allowed to be empty.
- Minimum requirement: include at least 3 clickable resource links when external resources exist for the topic.
- Prioritize official docs first (framework/vendor docs), then high-quality secondary learning references.
- Include clickable website links in Markdown format for each resource, for example: `[ASP.NET Core MVC overview](https://learn.microsoft.com/aspnet/core/mvc/overview)`.
- For each link, include one short line explaining which concept in the plan it supports and which stage(s) it helps with.
- Every resource must be directly relevant to the current Tutor-Mode task scope and approved stages; do not include generic or unrelated references.
- Do not enforce a strict 1:1 resource-to-stage mapping.
- A single resource may support multiple relevant stages, and a single stage may include multiple resources when needed.
- Do not use placeholder text such as `No external web resources were used for this plan.`

7. **Clarification Gate**
- If there are multiple valid approaches, architectural options, naming choices, or placement choices, stop and ask me before finalizing the plan.
- If proposing a +1 or +2 stretch concept, ask for explicit approval before finalizing, and include:
	- baseline option explanation,
	- stretch option explanation,
	- recommendation with a short justification for why stretch is worth it now.
- If no meaningful stretch opportunity exists for the current task, skip the stretch question and continue with baseline planning.
- If proposing a new pattern, ask an explicit postpone decision before finalizing:
	- keep current plan with established pattern, or
	- postpone and switch to an app-wide refactor plan that updates the pattern consistently.
- For this decision, explain both options in plain language at the learner's proficiency level and include why your recommendation is safer for consistency.

8. **Tutoring Decision Helper (Conditional)**
- Teaching-opportunity decision logging is governed by `.github/instructions/tutor-mode-plan-helper.instructions.md`.
- Invoke that helper only when a teaching-opportunity gate is asked:
	- stretch/new concept decision (+1/+2), or
	- new pattern/app-wide refactor postpone decision.
- If no teaching-opportunity gate is asked, do not invoke the helper and do not append to `.github/instructions/tutoring-decisions.md`.

9. **Chat Response Format**
- Do not paste the full plan in chat.
- Chat must only include a compact checklist report of completed planning sections and a pointer to `/memories/session/plan.md`.
- Chat checklist must include the exact section header label written to memory.

## Planning Rules
- Do not include implementation snippets unless a tiny illustrative snippet is necessary to explain the plan.
- Do not generate answer keys in Plan Mode.
- Do not generate an implementation pack in Plan Mode.
- Do not modify workspace files or editor contents in Plan Mode.
- Exception: when the tutoring decision helper is invoked for a teaching-opportunity gate, update `.github/instructions/tutoring-decisions.md` per helper rules.
- Memory updates to `/memories/session/plan.md` are required for final plan output.
- Use the canonical coding proficiency contract above to keep plan-to-agent handoff coherent; do not redefine alternate formulas in this file.
- Keep teaching decisions contextual: only add stretch prompts when justified by task needs and learner benefit.
- When established project patterns already solve the task, keep those patterns and teach through them instead of introducing a new pattern.
- Do not introduce isolated "new pattern" changes in a single feature when the rest of the app uses an established pattern.
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
- If a stretch decision was made, include an explicit note in the approved plan output describing: approved stretch level (+1 or +2), scope limits, and rationale so Agent mode can execute without reopening the same decision.
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
- [ ] Resource section is not empty and does not include a "no resources used" statement.
- [ ] Resource entries are actionable learning references the user can read to implement the approved stages.
- [ ] At least 3 relevant clickable links are included when external references exist for the topic.
- [ ] Every resource is directly relevant to the current Tutor-Mode task and mapped to the approved stage work.
- [ ] Resource mapping is relevance-based (shared resources across stages and multiple resources per stage are allowed where appropriate).
- [ ] Ambiguities or branching decisions are surfaced as questions.
- [ ] Stretch decision gate was asked only when a meaningful task-relevant learning opportunity existed (not by default every run).
- [ ] If a +1/+2 stretch option was proposed, explicit user approval was captured before finalizing the plan.
- [ ] Stretch proposal (when used) included baseline explanation, stretch explanation, and recommendation rationale.
- [ ] If stretch was approved, the plan handoff includes an explicit stretch decision note for Agent mode (level, scope limits, rationale).
- [ ] If a teaching-opportunity gate was asked (stretch/new concept or new pattern), `.github/instructions/tutor-mode-plan-helper.instructions.md` was invoked.
- [ ] If the helper was invoked, `.github/instructions/tutoring-decisions.md` append behavior followed helper rules.
- [ ] If no teaching-opportunity gate was asked, no tutoring decision log entry was appended.
- [ ] No implementation pack or answer key is included.
- [ ] Existing project conventions were prioritized over proficiency simplification where conflicts existed.
- [ ] Existing established process patterns from `.github/instructions/pattern-cheatsheet.instructions.md` were preserved when applicable and no artificial replacement pattern was introduced.
- [ ] Any new pattern proposal was made only after explicit user proficiency raise for this task and was framed as an app-wide update.
- [ ] When a new pattern was proposed, an explicit postpone decision (continue current plan vs switch to app-wide refactor plan) was asked and captured before finalizing.
- [ ] Proficiency logic used in this plan aligns with the canonical coding proficiency contract and does not introduce an alternate formula.
- [ ] The plan preserves MVC, folder structure, and naming conventions.
- [ ] The handoff to Agent mode is explicit but lightweight.
