---
name: Compliance Script Build Rules
description: "Use when producing Final Validation Stage compliance checks in Tutor Mode Agent outputs. Defines how to build compliance-script.sh and invocation requirements."
applyTo: "**"
---

# Compliance Script Build Rules

## Script Target and Location
- Script file name is strict: `compliance-script.sh`.
- Script location is strict: same directory as `run-all-tests.sh`.
- Script must be executable.
- Compliance mappings location is strict: `.github/instructions/compliance-mappings.txt` at repository root.
- Baseline preparation helper is required: `prepare-compliance-baseline.sh` in the same directory as `compliance-script.sh`.

## Path Anchoring Rules (Mandatory)
- Generated script must anchor execution to its own directory first:
	- `APP_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"`
	- `cd "$APP_ROOT"`
- Generated script must also compute repository root from script location:
	- `REPO_ROOT="$(cd "$APP_ROOT/.." && pwd)"`
- Actual file paths must be app-root relative paths only (for example `Interfaces/IPermissionService.cs`, not `WarehouseManagementSoftware/Interfaces/IPermissionService.cs`).
- Expected file paths must be repository-root relative paths under `ImplementationPacks/...`.
- Before compare, script must resolve both sides into explicit full paths:
	- `actual_file="$APP_ROOT/$actual_rel"`
	- `expected_file="$REPO_ROOT/$expected_rel"`
- Script must print anchored roots at start (`APP_ROOT` and `REPO_ROOT`) for traceability.
- Script and helper must both load mappings from `MAPPINGS_FILE="$REPO_ROOT/.github/instructions/compliance-mappings.txt"`.

## Script Behavior (Strict)
- Use `#!/usr/bin/env bash` and `set -euo pipefail`.
- Run all file comparisons in a deterministic order.
- Build comparison work dynamically from task definitions so task count drives comparison count automatically.
- If 5 tasks are defined, script must run 5 comparisons; if `N` tasks are defined, script must run `N` comparisons.
- Comparisons must be line-by-line using `diff --strip-trailing-cr` (or `git diff --no-index --no-color --word-diff=none`).
- Every comparison pair must use explicit full file paths (actual file + expected file).
- Do not use wildcards for comparison targets.
- Print explicit console status messages during execution (start, per-file check, mismatch/failure, and final status).
- If any comparison fails, print a clear red/failure message with the exact file pair that failed, then exit non-zero and do not delete itself.
- If the script crashes unexpectedly, print a crash/error message that includes the exact file pair being processed when the crash occurred.
- If all comparisons pass, print a clear green/success message before deletion, prompt the user to press any key to continue, and only then self-delete.

## Output Contract for Final Validation Stage
- After compliance check insertion, add section title: `Compliance Script Invocation`.
- Explicitly state this file was used: `.github/instructions/compliance-script-build.md`.
- The agent must auto-execute `./prepare-compliance-baseline.sh` during final validation after creating scripts.
- Provide one-command manual execution usage only for users: `./compliance-script.sh` (run from the same directory as `run-all-tests.sh`).
- Do not output a wall of compare commands in the final validation section when the script is used.

## Minimum Script Structure
1. Resolve app root and `cd` to it.
2. Resolve repo root from app root (`APP_ROOT/..`).
3. Declare explicit task inputs (or explicit actual/expected mappings) in deterministic order.
4. Use `actual_rel|expected_rel` mapping format where `actual_rel` is app-relative and `expected_rel` is repo-relative.
5. Dynamically generate one comparison pair per task from those mappings.
6. Validate generated pair count equals task count before execution; fail fast if mismatched.
7. Track the current comparison pair in variables before each compare.
8. Expand relative mappings into explicit full paths using `APP_ROOT` and `REPO_ROOT`.
9. Add deterministic console messages for each check (file pair start + result).
10. Iterate generated pairs and run line-by-line compare command.
11. Track failures and print summary.
12. Add crash handling (for example with a trap) that reports the exact file pair active at crash time.
13. Exit non-zero on any mismatch.
14. On full pass, print green-light, wait for keypress confirmation, and self-delete.

## Required Helper Structure (Baseline Preparation)
1. Resolve app root and `cd` to it.
2. Resolve repo root from app root (`APP_ROOT/..`).
3. Load mappings from `.github/instructions/compliance-mappings.txt`.
4. For each mapping, copy actual file into expected baseline location under `ImplementationPacks/...`.
5. Write each generated expected file path to `.compliance-generated-files.txt` manifest.
6. Print summary counts (total mappings, copied files, missing actual files, copy errors).
7. Exit non-zero if any mapping could not be prepared.

## Dynamic Scaling Verification Requirement
- Include a verification log line that prints task count and generated comparison count before comparisons begin.
- Required success condition: `task_count == comparison_count`.
- Required failure behavior: print a red/failure message with both counts and exit non-zero.
