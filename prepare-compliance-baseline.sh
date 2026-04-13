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
MANIFEST_FILE="$APP_ROOT/.compliance-generated-files.txt"

echo "[baseline] APP_ROOT: $APP_ROOT"
echo "[baseline] REPO_ROOT: $REPO_ROOT"
echo "[baseline] MAPPINGS_FILE: $MAPPINGS_FILE"

if [[ ! -f "$MAPPINGS_FILE" ]]; then
	echo "[baseline][FAIL] mappings file not found: $MAPPINGS_FILE"
	exit 1
fi

total_mappings=0
copied_files=0
missing_actual=0
copy_errors=0

rm -f "$MANIFEST_FILE"
touch "$MANIFEST_FILE"

while IFS='|' read -r actual_rel expected_rel || [[ -n "${actual_rel:-}" ]]; do
	[[ -z "${actual_rel// }" ]] && continue
	[[ "${actual_rel:0:1}" == "#" ]] && continue
	[[ -z "${expected_rel:-}" ]] && continue

	total_mappings=$((total_mappings + 1))

	actual_file="$APP_ROOT/$actual_rel"
	expected_file="$REPO_ROOT/$expected_rel"

	if [[ ! -f "$actual_file" ]]; then
		echo "[baseline][MISSING] actual file not found: $actual_file"
		missing_actual=$((missing_actual + 1))
		continue
	fi

	mkdir -p "$(dirname "$expected_file")"
	if cp "$actual_file" "$expected_file"; then
		copied_files=$((copied_files + 1))
		echo "$expected_file" >> "$MANIFEST_FILE"
		echo "[baseline][OK] copied: $actual_rel -> $expected_rel"
	else
		copy_errors=$((copy_errors + 1))
		echo "[baseline][FAIL] copy error: $actual_rel -> $expected_rel"
	fi
done < "$MAPPINGS_FILE"

echo "[baseline] total mappings: $total_mappings"
echo "[baseline] copied files: $copied_files"
echo "[baseline] missing actual files: $missing_actual"
echo "[baseline] copy errors: $copy_errors"

if (( missing_actual > 0 || copy_errors > 0 )); then
	echo "[baseline][FAIL] baseline preparation incomplete."
	exit 1
fi

echo "[baseline][PASS] baseline files prepared successfully."
