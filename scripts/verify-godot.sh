#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd -- "$script_dir/.." && pwd)"
godot_work_dir="${GODOT_WORK_DIR:-$repo_root/.godot-work}"

godot_command="${GODOT_COMMAND:-godot}"
godot_binary="$(command -v "$godot_command")"
godot_binary="$(readlink -f -- "$godot_binary")"
godot_root="$(dirname -- "$godot_binary")"
godot_nuget_source="${GODOT_NUGET_SOURCE:-$godot_root/GodotSharp/Tools/nupkgs}"

if [[ "$("$godot_command" --version)" != *".mono."* ]]; then
  echo "Godot verification requires a Mono-enabled Godot build." >&2
  exit 1
fi

if [[ ! -f "$godot_nuget_source/Godot.NET.Sdk.4.7.0.nupkg" ]]; then
  echo "Godot.NET.Sdk 4.7.0 was not found in '$godot_nuget_source'." >&2
  exit 1
fi

export HOME="${GODOT_VERIFY_HOME:-$godot_work_dir/home}"
export XDG_DATA_HOME="${XDG_DATA_HOME:-$godot_work_dir/xdg-data}"
export XDG_CONFIG_HOME="${XDG_CONFIG_HOME:-$godot_work_dir/xdg-config}"
export XDG_CACHE_HOME="${XDG_CACHE_HOME:-$godot_work_dir/xdg-cache}"
export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-$godot_work_dir/dotnet-cli-home}"
export NUGET_PACKAGES="${NUGET_PACKAGES:-$godot_work_dir/nuget-packages}"
export NUGET_HTTP_CACHE_PATH="${NUGET_HTTP_CACHE_PATH:-$godot_work_dir/nuget-http-cache}"
export NUGET_SCRATCH="${NUGET_SCRATCH:-$godot_work_dir/nuget-scratch}"
export NUGET_PLUGINS_CACHE_PATH="${NUGET_PLUGINS_CACHE_PATH:-$godot_work_dir/nuget-plugins-cache}"
export MSBuildUserExtensionsPath="${MSBuildUserExtensionsPath:-$godot_work_dir/msbuild-user-extensions}"
export DOTNET_NOLOGO="${DOTNET_NOLOGO:-true}"
export DOTNET_CLI_TELEMETRY_OPTOUT="${DOTNET_CLI_TELEMETRY_OPTOUT:-true}"
export MSBUILDTERMINALLOGGER="${MSBUILDTERMINALLOGGER:-off}"

mkdir -p \
  "$HOME" \
  "$XDG_DATA_HOME" \
  "$XDG_CONFIG_HOME" \
  "$XDG_CACHE_HOME" \
  "$DOTNET_CLI_HOME" \
  "$NUGET_PACKAGES" \
  "$NUGET_HTTP_CACHE_PATH" \
  "$NUGET_SCRATCH" \
  "$NUGET_PLUGINS_CACHE_PATH" \
  "$MSBuildUserExtensionsPath"

cd "$repo_root"

dotnet build tests/Genomancy.Godot.Smoke/Genomancy.Godot.Smoke.csproj \
  --tl:off \
  --disable-build-servers \
  -m:1 \
  -p:RestoreSources="$godot_nuget_source" \
  -p:RestoreBuildInParallel=false \
  -p:BuildInParallel=false

"$godot_command" --headless --path tests/Genomancy.Godot.Smoke
