#!/usr/bin/env bash
set -euo pipefail

# Refreshed for Tutor-Mode checkout/cart flow patch cycle (2026-04-12)

APP_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$APP_ROOT"

REPO_ROOT="$(cd "$APP_ROOT/.." && pwd)"
if [[ ! -f "$REPO_ROOT/.github/instructions/compliance-mappings.txt" && -f "$APP_ROOT/.github/instructions/compliance-mappings.txt" ]]; then
	REPO_ROOT="$APP_ROOT"
fi

MAPPINGS_FILE="$REPO_ROOT/.github/instructions/compliance-mappings.txt"

echo "[compliance] APP_ROOT: $APP_ROOT"
echo "[compliance] REPO_ROOT: $REPO_ROOT"
echo "[compliance] MAPPINGS_FILE: $MAPPINGS_FILE"

if [[ ! -f "$MAPPINGS_FILE" ]]; then
	echo "[compliance][FAIL] mappings file not found: $MAPPINGS_FILE"
	exit 1
fi

declare -a mappings=()
while IFS='|' read -r actual_rel expected_rel || [[ -n "${actual_rel:-}" ]]; do
	[[ -z "${actual_rel// }" ]] && continue
	[[ "${actual_rel:0:1}" == "#" ]] && continue
	[[ -z "${expected_rel:-}" ]] && continue
	mappings+=("$actual_rel|$expected_rel")
done < "$MAPPINGS_FILE"

task_count=${#mappings[@]}
comparison_count=${#mappings[@]}

echo "[compliance] task count: $task_count"
echo "[compliance] generated comparison count: $comparison_count"

if (( task_count != comparison_count )); then
	echo "[compliance][FAIL] dynamic scaling mismatch. task_count=$task_count, comparison_count=$comparison_count"
	exit 1
fi

if (( task_count == 0 )); then
	echo "[compliance][FAIL] no mappings found to compare."
	exit 1
fi

current_actual=""
current_expected=""

handle_crash() {
	local exit_code=$?
	if (( exit_code != 0 )); then
		echo "[compliance][CRASH] script aborted while processing:"
		echo "  actual  : $current_actual"
		echo "  expected: $current_expected"
	fi
	exit $exit_code
}
trap handle_crash EXIT

failed_count=0

for mapping in "${mappings[@]}"; do
	actual_rel="${mapping%%|*}"
	expected_rel="${mapping#*|}"

	actual_file="$APP_ROOT/$actual_rel"
	expected_file="$REPO_ROOT/$expected_rel"

	current_actual="$actual_file"
	current_expected="$expected_file"

	echo "[compliance] checking"
	echo "  actual  : $actual_file"
	echo "  expected: $expected_file"

	if [[ ! -f "$actual_file" ]]; then
		echo "[compliance][FAIL] missing actual file: $actual_file"
		failed_count=$((failed_count + 1))
		continue
	fi

	if [[ ! -f "$expected_file" ]]; then
		echo "[compliance][FAIL] missing expected file: $expected_file"
		failed_count=$((failed_count + 1))
		continue
	fi

	if diff --strip-trailing-cr "$actual_file" "$expected_file" >/dev/null; then
		echo "[compliance][OK] match"
	else
		echo "[compliance][FAIL] mismatch for pair:"
		echo "  actual  : $actual_file"
		echo "  expected: $expected_file"
		failed_count=$((failed_count + 1))
	fi
done

echo "[compliance] total pairs: $comparison_count"
echo "[compliance] failed pairs: $failed_count"

if (( failed_count > 0 )); then
	echo "[compliance][FAIL] compliance check failed."
	exit 1
fi

echo "[compliance][PASS] GREEN LIGHT: all mapped files match 1:1 exactly."
read -r -n 1 -p "Press any key to continue and delete compliance script..." _
echo
trap - EXIT
rm -- "$0"
echo "[compliance] compliance-script.sh deleted after full pass."
