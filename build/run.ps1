# 本地运行(不发布)。
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

# 让 WinExe 能找到仓库内的 .NET 运行时。
$env:DOTNET_ROOT = Join-Path $root '.dotnet'

& $dotnet run --project (Join-Path $root 'src\Seeker.DevToolbox.App\Seeker.DevToolbox.App.csproj') -c $Configuration
exit $LASTEXITCODE
