# 构建解决方案。优先使用仓库内的 .dotnet SDK。
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '..')

$dotnet = Join-Path $root '.dotnet\dotnet.exe'
if (-not (Test-Path $dotnet)) {
    $dotnet = 'dotnet'
}

& $dotnet build (Join-Path $root 'Seeker.DevToolbox.sln') -c $Configuration --nologo
exit $LASTEXITCODE
