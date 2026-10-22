#!/usr/bin/env bash
# Build the plugin and assemble a package with the framework's packer.
#
# Requires a sibling PluginManager checkout, or HOSIHIKARI_PLUGINMANAGER.
# Usage: bash ./pack.sh [output-root]
set -euo pipefail

here=$(cd "$(dirname "$0")" && pwd)
name='Hosihikari.Plugin.Templates.FSharp'
root=${1:-dist}
case "$root" in
    /*) ;;
    *) root="$here/$root" ;;
esac

dotnet build "$here/$name.fsproj" -c Release --nologo
output="$here/bin/Release/net10.0"

core=${HOSIHIKARI_PLUGINMANAGER:-"$(cd "$here/.." && pwd)/PluginManager"}
pack_tool="$core/tools/Pack/Hosihikari.Plugin.Pack.csproj"
if [ ! -f "$pack_tool" ]; then
    echo "找不到核心打包器：$pack_tool。请并列检出 PluginManager，或设置 HOSIHIKARI_PLUGINMANAGER。" >&2
    exit 1
fi

dotnet run --project "$pack_tool" -c Release -- pack-plugin "$output" "$here/manifest.json" "$root"
