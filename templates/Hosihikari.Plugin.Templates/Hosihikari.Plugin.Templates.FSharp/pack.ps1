<#
    Build the plugin and assemble a package with the framework's packer.

    Requires a sibling PluginManager checkout, or HOSIHIKARI_PLUGINMANAGER.
    Usage: pwsh ./pack.ps1 [-OutputRoot dist]
#>
param(
    [string]$OutputRoot = 'dist'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$name = 'Hosihikari.Plugin.Templates.FSharp'

& dotnet build (Join-Path $here "$name.fsproj") -c Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'the plugin failed to build' }

$output = Join-Path $here 'bin/Release/net10.0'
$root = if ([IO.Path]::IsPathRooted($OutputRoot)) { $OutputRoot } else { Join-Path $here $OutputRoot }

$core = if ($env:HOSIHIKARI_PLUGINMANAGER) {
    $env:HOSIHIKARI_PLUGINMANAGER
} else {
    Join-Path (Split-Path $here -Parent) 'PluginManager'
}
$packTool = Join-Path $core 'tools/Pack/Hosihikari.Plugin.Pack.csproj'
if (-not (Test-Path $packTool)) {
    throw "找不到核心打包器：$packTool。请并列检出 PluginManager，或设置 HOSIHIKARI_PLUGINMANAGER。"
}

& dotnet run --project $packTool -c Release -- pack-plugin $output (Join-Path $here 'manifest.json') $root
if ($LASTEXITCODE -ne 0) { throw 'the packer failed' }
