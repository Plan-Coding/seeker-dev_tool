# 发布免安装版本。
#   .\build\publish.ps1                  -> 目录绿色版 + zip
#   .\build\publish.ps1 -SingleFile      -> 单文件 exe + zip
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [ValidateSet('win-x64', 'win-arm64')]
    [string]$Runtime = 'win-x64',

    [switch]$SingleFile,

    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

$dotnet = Join-Path $root '.dotnet\dotnet.exe'
if (-not (Test-Path $dotnet)) {
    $dotnet = 'dotnet'
}

$props = Get-Content (Join-Path $root 'Directory.Build.props') -Raw
$version = [regex]::Match($props, '<Version>([^<]+)</Version>').Groups[1].Value
if (-not $version) {
    $version = '0.1.0'
}

if (-not $SkipTests) {
    & (Join-Path $PSScriptRoot 'test.ps1')
    if ($LASTEXITCODE -ne 0) {
        throw '测试未通过,已中止发布。'
    }
}

$kind = if ($SingleFile) { 'single' } else { 'portable' }
$artifactRoot = Join-Path $root 'artifacts'
$output = Join-Path $artifactRoot "Seeker.DevToolbox-$version-$kind"
if (Test-Path $output) {
    Remove-Item $output -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $output | Out-Null

$publishArgs = @(
    'publish',
    (Join-Path $root 'src\Seeker.DevToolbox.App\Seeker.DevToolbox.App.csproj'),
    '-c', $Configuration,
    '-r', $Runtime,
    '--self-contained', 'true',
    '-p:DebugType=None',
    '-p:DebugSymbols=false',
    '-o', $output
)

if ($SingleFile) {
    $publishArgs += @(
        '-p:PublishSingleFile=true',
        '-p:IncludeNativeLibrariesForSelfExtract=true',
        '-p:EnableCompressionInSingleFile=true'
    )
}
else {
    $publishArgs += @('-p:PublishSingleFile=false')
}

Write-Host "Publishing $version ($Runtime, singleFile=$($SingleFile.IsPresent)) ..."
& $dotnet @publishArgs
if ($LASTEXITCODE -ne 0) {
    throw 'dotnet publish 失败。'
}

if (-not $SingleFile) {
    # 便携模式:数据写到程序目录下的 data\。
    New-Item -ItemType File -Path (Join-Path $output 'portable.marker') -Force | Out-Null

    @"
Seeker DevToolbox $version(绿色版)
====================================

1. 双击 Seeker.DevToolbox.exe 运行,无需安装。
2. 设置、日志、缓存都会写入本目录下的 data\ 文件夹。
3. 迁移时复制整个文件夹;卸载时直接删除整个文件夹。
4. 删除 portable.marker 后,数据会改写到 %LOCALAPPDATA%\Seeker\DevToolbox。

系统要求:Windows 10 1809+ / Windows 11(x64)。
"@ | Set-Content (Join-Path $output '使用说明.txt') -Encoding UTF8
}

$zipName = "Seeker.DevToolbox-$version-$kind-$Runtime.zip"
$zipPath = Join-Path $artifactRoot $zipName

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Compress-Archive -Path $output -DestinationPath $zipPath -CompressionLevel Optimal

$sha256 = [System.Security.Cryptography.SHA256]::Create()
$stream = [System.IO.File]::OpenRead($zipPath)
try {
    $hash = -join ($sha256.ComputeHash($stream) | ForEach-Object { $_.ToString('x2') })
}
finally {
    $stream.Dispose()
    $sha256.Dispose()
}

$sumsPath = Join-Path $artifactRoot 'SHA256SUMS.txt'
$existingSums = @()

if (Test-Path $sumsPath) {
    $existingSums = @(Get-Content $sumsPath | Where-Object { $_ -and $_ -notmatch [regex]::Escape($zipName) })
}

$existingSums = @($existingSums) + "$hash  $zipName"
$existingSums | Set-Content $sumsPath -Encoding ASCII

$sizeMb = [math]::Round((Get-Item $zipPath).Length / 1MB, 1)

Write-Host ''
Write-Host "Output : $output"
Write-Host "Archive: $zipPath ($sizeMb MB)"
Write-Host "SHA256 : $hash"
