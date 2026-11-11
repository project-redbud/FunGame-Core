#!/usr/bin/env bash
set -euo pipefail

resolver="${1:-$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/resolve-release-version.sh}"
scratch="$(mktemp -d)"
trap 'rm -f "$scratch/output" "$scratch/log"; rmdir "$scratch"' EXIT
cases=0

check() {
  local event="$1" ref="$2" input="$3" prefix="$4" expected_status="$5" expected_output="$6"
  local status=0
  : > "$scratch/output"
  GITHUB_EVENT_NAME="$event" GITHUB_REF_NAME="$ref" INPUT_VERSION="$input" \
    SOURCE_VERSION_PREFIX="$prefix" GITHUB_OUTPUT="$scratch/output" \
    bash "$resolver" > "$scratch/log" 2>&1 || status=$?
  if [[ "$status" != "$expected_status" || "$(cat "$scratch/output")" != "$expected_output" ]]; then
    printf 'FAIL: event=%s ref=%s input=%s prefix=%s status=%s\n' "$event" "$ref" "$input" "$prefix" "$status" >&2
    cat "$scratch/log" >&2
    exit 1
  fi
  cases=$((cases + 1))
}

check push v3.0.0 '' 3.0.0-dev 0 'version=3.0.0'
check push 3.0.0 '' 3.0.0 0 'version=3.0.0'
check push v3.0.0-rc.1 '' '' 0 'skipped=true'
check push 3.0.0-dev-1111 '' '' 0 'skipped=true'
check push v3.0.0-extra '' 3.0.0-dev 0 'skipped=true'
check push v03.0.0 '' 3.0.0-dev 0 'skipped=true'
check push v2.1.1 '' 3.0.0-dev 1 ''
check push v3.0.1 '' 3.0.0-dev 1 ''
check workflow_dispatch master 3.0.0 3.0.0-dev 0 'version=3.0.0'
check workflow_dispatch master 3.0.0-rc.1 3.0.0-dev 0 'version=3.0.0-rc.1'
check workflow_dispatch master 3.0.0-dev-1111 3.0.0-dev 0 'version=3.0.0-dev-1111'
check workflow_dispatch master 3.0.0+build.1 3.0.0-dev 0 'version=3.0.0+build.1'
check workflow_dispatch master 3.0.0-rc.1+build.1 3.0.0-dev 0 'version=3.0.0-rc.1+build.1'
check workflow_dispatch master 2.1.1 3.0.0-dev 1 ''
check workflow_dispatch master 3.1.0 3.0.0-dev 1 ''
check workflow_dispatch master '' 3.0.0-dev 1 ''
check workflow_dispatch master 3.0.0garbage 3.0.0-dev 1 ''
check workflow_dispatch master 3.0.0- 3.0.0-dev 1 ''
check workflow_dispatch master 3.0.0-rc..1 3.0.0-dev 1 ''
check workflow_dispatch master 3.0.0-01 3.0.0-dev 1 ''
check workflow_dispatch master 03.0.0 3.0.0-dev 1 ''
check workflow_dispatch master '3.0.0$(exit 42)' 3.0.0-dev 1 ''
check workflow_dispatch master '3.0.0"; exit 42; #' 3.0.0-dev 1 ''
check workflow_dispatch master $'3.0.0\nversion=2.1.1' 3.0.0-dev 1 ''
check workflow_dispatch master 3.0.0 '' 1 ''
check pull_request master 3.0.0 3.0.0-dev 1 ''

echo "Release version regression tests passed: $cases"
