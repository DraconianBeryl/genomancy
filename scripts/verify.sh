#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd -- "$script_dir/.." && pwd)"
dotnet_work_dir="${DOTNET_WORK_DIR:-$repo_root/.dotnet-work}"

export HOME="${DOTNET_VERIFY_HOME:-$dotnet_work_dir/home}"
export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-$dotnet_work_dir/cli-home}"
export DOTNET_NOLOGO="${DOTNET_NOLOGO:-true}"
export DOTNET_CLI_TELEMETRY_OPTOUT="${DOTNET_CLI_TELEMETRY_OPTOUT:-true}"
export XDG_CACHE_HOME="${XDG_CACHE_HOME:-$dotnet_work_dir/xdg-cache}"
export XDG_DATA_HOME="${XDG_DATA_HOME:-$dotnet_work_dir/xdg-data}"
export NUGET_PACKAGES="${NUGET_PACKAGES:-$dotnet_work_dir/nuget-packages}"
export NUGET_HTTP_CACHE_PATH="${NUGET_HTTP_CACHE_PATH:-$dotnet_work_dir/nuget-http-cache}"
export NUGET_SCRATCH="${NUGET_SCRATCH:-$dotnet_work_dir/nuget-scratch}"
export NUGET_PLUGINS_CACHE_PATH="${NUGET_PLUGINS_CACHE_PATH:-$dotnet_work_dir/nuget-plugins-cache}"
export MSBuildUserExtensionsPath="${MSBuildUserExtensionsPath:-$dotnet_work_dir/msbuild-user-extensions}"
export MSBUILDTERMINALLOGGER="${MSBUILDTERMINALLOGGER:-off}"

mkdir -p \
  "$HOME" \
  "$DOTNET_CLI_HOME" \
  "$XDG_CACHE_HOME" \
  "$XDG_DATA_HOME" \
  "$NUGET_PACKAGES" \
  "$NUGET_HTTP_CACHE_PATH" \
  "$NUGET_SCRATCH" \
  "$NUGET_PLUGINS_CACHE_PATH" \
  "$MSBuildUserExtensionsPath"

cd "$repo_root"

dotnet restore Genomancy.sln --tl:off --disable-build-servers
dotnet build Genomancy.sln --no-restore --tl:off --disable-build-servers
dotnet run --no-restore --no-build --project tests/Genomancy.Tests/Genomancy.Tests.csproj
