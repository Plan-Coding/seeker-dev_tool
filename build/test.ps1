# 运行单元测试。
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '..')

$dotnet = Join-Path $root '.dotnet\dotnet.exe'
if (-not (Test-Path $dotnet)) {
    $dotnet = 'dotnet'
}

& $dotnet test (Join-Path $root 'tests\Seeker.DevToolbox.Core.Tests\Seeker.DevToolbox.Core.Tests.csproj') -c Debug --nologo
exit $LASTEXITCODE
