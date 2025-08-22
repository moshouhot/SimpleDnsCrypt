# SimpleDnsCrypt 构建和打包脚本
param(
    [string]$Version = "0.8.0",
    [string]$Configuration = "Release",
    [switch]$SkipBuild,
    [switch]$CreateMSI,
    [switch]$CreateZIP,
    [switch]$All
)

# 如果指定了 -All，则启用所有选项
if ($All) {
    $CreateMSI = $true
    $CreateZIP = $true
}

# 如果没有指定任何打包选项，默认创建ZIP
if (-not $CreateMSI -and -not $CreateZIP) {
    $CreateZIP = $true
}

Write-Host "SimpleDnsCrypt 构建脚本" -ForegroundColor Green
Write-Host "版本: $Version" -ForegroundColor Yellow
Write-Host "配置: $Configuration" -ForegroundColor Yellow

# 检查必要工具
function Test-Tool {
    param([string]$Name, [string]$Command)
    
    try {
        & $Command --version 2>$null | Out-Null
        Write-Host "✓ $Name 已安装" -ForegroundColor Green
        return $true
    } catch {
        Write-Host "✗ $Name 未找到" -ForegroundColor Red
        return $false
    }
}

# 检查MSBuild
$msbuildPath = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
if (-not (Test-Path $msbuildPath)) {
    $msbuildPath = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
}
if (-not (Test-Path $msbuildPath)) {
    $msbuildPath = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
}
if (-not (Test-Path $msbuildPath)) {
    $msbuildPath = "msbuild.exe"
}

# 创建输出目录
$distDir = "dist"
if (Test-Path $distDir) {
    Remove-Item $distDir -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $distDir | Out-Null

# 构建项目
if (-not $SkipBuild) {
    Write-Host "`n开始构建项目..." -ForegroundColor Cyan
    
    # 还原NuGet包
    Write-Host "还原NuGet包..." -ForegroundColor Yellow
    & nuget restore SimpleDnsCrypt.sln
    if ($LASTEXITCODE -ne 0) {
        Write-Host "NuGet还原失败" -ForegroundColor Red
        exit 1
    }
    
    # 构建x64版本
    Write-Host "构建 $Configuration x64..." -ForegroundColor Yellow
    & $msbuildPath SimpleDnsCrypt.sln /p:Configuration=$Configuration /p:Platform=x64 /p:OutputPath=bin\$Configuration\net48\
    if ($LASTEXITCODE -ne 0) {
        Write-Host "x64构建失败" -ForegroundColor Red
        exit 1
    }
    
    # 构建x86版本
    Write-Host "构建 $Configuration x86..." -ForegroundColor Yellow
    & $msbuildPath SimpleDnsCrypt.sln /p:Configuration=$Configuration /p:Platform=x86 /p:OutputPath=bin\$Configuration\net48-x86\
    if ($LASTEXITCODE -ne 0) {
        Write-Host "x86构建失败" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✓ 构建完成" -ForegroundColor Green
}

# 创建ZIP绿色版
if ($CreateZIP) {
    Write-Host "`n创建ZIP绿色版..." -ForegroundColor Cyan
    
    $x64Path = "SimpleDnsCrypt\bin\$Configuration\net48"
    $x86Path = "SimpleDnsCrypt\bin\$Configuration\net48-x86"
    
    if (Test-Path $x64Path) {
        $zipName = "SimpleDnsCrypt-$Version-x64-portable.zip"
        Compress-Archive -Path "$x64Path\*" -DestinationPath "$distDir\$zipName" -Force
        Write-Host "✓ 创建了 $zipName" -ForegroundColor Green
    } else {
        Write-Host "✗ x64构建输出不存在: $x64Path" -ForegroundColor Red
    }
    
    if (Test-Path $x86Path) {
        $zipName = "SimpleDnsCrypt-$Version-x86-portable.zip"
        Compress-Archive -Path "$x86Path\*" -DestinationPath "$distDir\$zipName" -Force
        Write-Host "✓ 创建了 $zipName" -ForegroundColor Green
    } else {
        Write-Host "✗ x86构建输出不存在: $x86Path" -ForegroundColor Red
    }
}

# 创建MSI安装包
if ($CreateMSI) {
    Write-Host "`n创建MSI安装包..." -ForegroundColor Cyan
    
    # 检查WiX工具集
    $wixPath = "${env:ProgramFiles(x86)}\WiX Toolset v3.11\bin"
    if (-not (Test-Path "$wixPath\candle.exe")) {
        Write-Host "✗ WiX Toolset未安装，请先安装WiX Toolset v3.11" -ForegroundColor Red
        Write-Host "下载地址: https://github.com/wixtoolset/wix3/releases" -ForegroundColor Yellow
    } else {
        # 这里需要WiX配置文件，暂时跳过
        Write-Host "⚠ MSI创建需要WiX配置文件，请参考GitHub Actions中的配置" -ForegroundColor Yellow
    }
}

Write-Host "`n构建完成！" -ForegroundColor Green
Write-Host "输出目录: $distDir" -ForegroundColor Yellow

if (Test-Path $distDir) {
    Write-Host "`n生成的文件:" -ForegroundColor Cyan
    Get-ChildItem $distDir | ForEach-Object {
        $size = [math]::Round($_.Length / 1MB, 2)
        Write-Host "  $($_.Name) ($size MB)" -ForegroundColor White
    }
}

Write-Host "`n使用说明:" -ForegroundColor Cyan
Write-Host "  .\build-package.ps1 -Version 1.2.3 -All          # 构建所有包" -ForegroundColor White
Write-Host "  .\build-package.ps1 -CreateZIP                   # 只创建ZIP包" -ForegroundColor White
Write-Host "  .\build-package.ps1 -SkipBuild -CreateZIP        # 跳过构建，只打包" -ForegroundColor White
