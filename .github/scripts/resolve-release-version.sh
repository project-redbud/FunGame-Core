#!/usr/bin/env bash
set -euo pipefail

number='(0|[1-9][0-9]*)'
core="$number\.$number\.$number"
label='(0|[1-9][0-9]*|[0-9]*[A-Za-z-][0-9A-Za-z-]*)'
version_pattern="^$core(-$label(\.$label)*)?(\+[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?$"

case "$GITHUB_EVENT_NAME" in
  push)
    version="${GITHUB_REF_NAME#v}"
    if [[ ! "$version" =~ ^$core$ ]]; then
      echo "tag $GITHUB_REF_NAME 不是正式版本，本次跳过发布。"
      echo 'skipped=true' >> "$GITHUB_OUTPUT"
      exit 0
    fi
    ;;
  workflow_dispatch)
    version="${INPUT_VERSION:-}"
    if [[ ! "$version" =~ $version_pattern ]]; then
      echo '无效的版本号：请填写完整的 SemVer 版本，如 3.0.0 或 3.0.0-rc.1。' >&2
      exit 1
    fi
    ;;
  *)
    echo '不支持的发布事件。' >&2
    exit 1
    ;;
esac

if [[ ! "${SOURCE_VERSION_PREFIX:-}" =~ ^($core)(-|$) ]]; then
  echo '项目 VersionPrefix 必须以完整的三段版本号开头。' >&2
  exit 1
fi
source_version="${BASH_REMATCH[1]}"
if [[ "${version%%[-+]*}" != "$source_version" ]]; then
  echo "发布版本 $version 与源码 VersionPrefix $SOURCE_VERSION_PREFIX 不匹配。" >&2
  exit 1
fi

echo "version=$version" >> "$GITHUB_OUTPUT"
