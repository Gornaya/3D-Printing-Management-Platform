---
name: Tutor Mode Plan Helper
description: "Helper rules for plan-mode teaching-opportunity decision logging (stretch/new concept and new pattern decisions only). Invoked conditionally by tutor-mode-for-plan-mode.instructions.md."
applyTo: "**"
---

# Tutor-Mode Plan Helper

## Activation Contract
- This helper is used only with `.github/instructions/tutor-mode-for-plan-mode.instructions.md`.
- Do not invoke this helper by default.
- Invoke this helper only when plan mode asks a teaching-opportunity gate:
  - stretch/new concept decision (+1/+2), or
  - new pattern/app-wide refactor postpone decision.

## Scope Control
- This helper does not govern routine design choices.
- Do not log every architectural or naming decision.
- Only teaching-opportunity decisions belong in `.github/instructions/tutoring-decisions.md`.

## Logging Rules (Append-Only)
When invoked, append one decision entry to `.github/instructions/tutoring-decisions.md` with:
- timestamp,
- prompt problem summary,
- teaching-opportunity type (`stretch/new concept` or `new pattern`),
- options explained,
- user decision,
- recommendation rationale,
- rationale against the non-selected option.

## Chat Logging Boundary
- Do not log basic chat Q/A in `.github/instructions/tutoring-decisions.md`.
- Log only teaching-opportunity decision summaries (decision gate, options, chosen path, and rationale).

## Helper Compliance Check
Before finalizing plan output in runs where this helper was invoked, verify:
- [ ] A teaching-opportunity gate was actually asked.
- [ ] Log entry was appended only for the teaching-opportunity decision.
- [ ] Routine non-teaching design decisions were not logged.
- [ ] Basic chat Q/A was not logged; only teaching-opportunity decision summaries were appended.
- [ ] Entry language is clear for the learner's current proficiency level.
