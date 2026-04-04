---
name: Tutor Mode Agent Helper
description: "Helper checklist and finalization reference for Tutor Mode agent outputs. Loaded by tutor-mode-for-agent-mode.instructions.md."
applyTo: "**"
---

# Tutor-Mode Agent Helper

## Activation Contract
- This helper is used only with `.github/instructions/tutor-mode-for-agent-mode.instructions.md`.
- Parent-load timing is strict and optimized: load this helper at the beginning of Agent mode execution.
- Do not delay helper loading to middle or end stages; early load prevents format drift and avoids regeneration rework.

## Agent Compliance Check
Before finalizing an implementation response, verify:
- [ ] Full implementation output was appended to `/memories/session/plan.md`.
- [ ] Chat response is checklist-only and does not include full implementation content.
- [ ] Memory append used header format `## IMPLEMENTATION OUTPUT - Full Solution - YYYY-MM-DD HH:mm`.
- [ ] All approved stages were implemented in one pass.
- [ ] Each stage contains an explicit user validation checkpoint before the following stage.
- [ ] No direct file/editor modifications are performed except creating or updating `compliance-script.sh` and `prepare-compliance-baseline.sh` in the same directory as `run-all-tests.sh`, plus `.github/instructions/compliance-mappings.txt`.
- [ ] An Implementation Pack is provided.
- [ ] Existing project conventions were prioritized over proficiency simplification where conflicts existed.
- [ ] Non-JavaScript snippets use numbered TODO blanks at the correct ratio.
- [ ] TODO blank difficulty and memory-reinforcement blanks are calibrated to the current coding proficiency.
- [ ] Proficiency scaling follows a strict linear 1-10 directive style from direct (1) to abstract (9) and TODO-only mode (10).
- [ ] For small-scope tasks, low-level memory-reinforcement blanks were increased as needed to meet the target blank ratio.
- [ ] For larger-scope tasks, low-level memory-reinforcement blanks are no more than 25% of all TODO blanks and quality priority is preserved.
- [ ] Long string literals by type (messages, errors, configs) were not converted into TODO blanks.
- [ ] Every TODO includes an inline expectation note describing exactly what to fill.
- [ ] For proficiency levels 1-9, every TODO expectation note is specific and scope-clear (not generic) and can be followed without guessing.
- [ ] For proficiency level 10, TODO markers are bare (for example `TODO_4_7`) with no inline expectation text and no explanatory clue text.
- [ ] Commands that require names include the exact expected name in the TODO command.
- [ ] Command-related TODOs include full expected target names (migration/file/project/etc.), not vague placeholders.
- [ ] Any 5/10-6/10 concept or coding-practice upgrade is minimal, justified for learning, and appropriate to the current proficiency.
- [ ] JavaScript snippets are complete with zero blanks.
- [ ] The structure and explanation depth fit the current coding proficiency level.
- [ ] Exact project names and insertion points are used.
- [ ] A stage-level Answer Key is included immediately after its corresponding stage.
- [ ] Answer Key entries are line-by-line (one per line), not inline.
- [ ] Answer Key numbering uses strict `<Stage>.<Task>` format (for example `1.1`, `1.2`, `3.2`).
- [ ] Every Answer Key line uses a markdown hard line break so preview keeps one-answer-per-line layout.
- [ ] Validation commands are present when needed.
- [ ] A `Final Validation Stage <N>` exists where `N = last stage + 1`.
- [ ] Final validation stage includes copied `Plan Compliance Check` and `Agent Compliance Check` blocks.
- [ ] Tutor-Mode agent flow explicitly loaded and applied `.github/instructions/pattern-cheatsheet.instructions.md` immediately after helper load (passing requirement).
- [ ] Final validation stage includes `Compliance Script Invocation` and explicit reference to `.github/instructions/compliance-script-build.md`.
- [ ] `prepare-compliance-baseline.sh` is created in the same directory as `run-all-tests.sh`.
- [ ] `prepare-compliance-baseline.sh` is auto-executed by the agent during final validation so baseline files are ready for user compliance run.
- [ ] `compliance-script.sh` is created in the same directory as `run-all-tests.sh` and runs with one manual command.
- [ ] `compliance-script.sh` performs line-by-line comparisons and enforces a 1:1 green-light rule.
- [ ] `compliance-script.sh` uses explicit full expected file names/paths for every compared file.
- [ ] `compliance-script.sh` self-deletes only after all comparisons pass.
- [ ] Scope was not expanded without asking.
