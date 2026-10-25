# 安装本仓库使用的 .NET SDK 到 .dotnet/ 目录(不需要管理员权限)。
$ErrorActionPreference = 'Stop'

$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$installDir = Join-Path $root '.dotnet'
$toolsDir = Join-Path $root '.tools'
New-Item -ItemType Directory -Force -Path $toolsDir | Out-Null
$scriptPath = Join-Path $toolsDir 'dotnet-install.ps1'

if (-not (Test-Path $scriptPath)) {
    Write-Host 'Downloading dotnet-install.ps1 ...'
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest 'https://dot.net/v1/dotnet-install.ps1' -OutFile $scriptPath -UseBasicParsing
}

$globalJson = Get-Content (Join-Path $root 'global.json') -Raw | ConvertFrom-Json
$version = $globalJson.sdk.version

Write-Host "Installing .NET SDK $version to $installDir ..."
& $scriptPath -Version $version -InstallDir $installDir -NoPath

Write-Host ''
Write-Host 'Done. 常用命令:'
Write-Host '  .\build\build.ps1'
Write-Host '  .\build\run.ps1'
Write-Host '  .\build\test.ps1'
Write-Host '  .\build\publish.ps1'
