# 测试开机自启动功能
Write-Host "=== SimpleDnsCrypt 开机自启动功能测试 ===" -ForegroundColor Green

# 检查注册表项
$registryPath = "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
$appName = "SimpleDnsCrypt"

Write-Host "`n1. 检查当前开机自启动状态:" -ForegroundColor Yellow
try {
    $currentValue = Get-ItemProperty -Path $registryPath -Name $appName -ErrorAction SilentlyContinue
    if ($currentValue) {
        Write-Host "   ✓ 开机自启动已启用" -ForegroundColor Green
        Write-Host "   路径: $($currentValue.$appName)" -ForegroundColor Gray
    } else {
        Write-Host "   × 开机自启动未启用" -ForegroundColor Red
    }
} catch {
    Write-Host "   × 检查失败: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n2. 测试启用开机自启动:" -ForegroundColor Yellow
$exePath = Join-Path $PSScriptRoot "bin\Debug\net48\SimpleDnsCrypt.exe"
if (Test-Path $exePath) {
    try {
        Set-ItemProperty -Path $registryPath -Name $appName -Value "`"$exePath`"" -Type String
        Write-Host "   ✓ 开机自启动已启用" -ForegroundColor Green
        
        # 验证设置
        $verifyValue = Get-ItemProperty -Path $registryPath -Name $appName -ErrorAction SilentlyContinue
        if ($verifyValue) {
            Write-Host "   验证: $($verifyValue.$appName)" -ForegroundColor Gray
        }
    } catch {
        Write-Host "   × 启用失败: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "   × 可执行文件不存在: $exePath" -ForegroundColor Red
}

Write-Host "`n3. 测试禁用开机自启动:" -ForegroundColor Yellow
try {
    Remove-ItemProperty -Path $registryPath -Name $appName -ErrorAction SilentlyContinue
    Write-Host "   ✓ 开机自启动已禁用" -ForegroundColor Green
    
    # 验证删除
    $verifyValue = Get-ItemProperty -Path $registryPath -Name $appName -ErrorAction SilentlyContinue
    if (-not $verifyValue) {
        Write-Host "   验证: 注册表项已删除" -ForegroundColor Gray
    } else {
        Write-Host "   警告: 注册表项仍然存在" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   × 禁用失败: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== 测试完成 ===" -ForegroundColor Green
Write-Host "请在程序中测试以下功能:" -ForegroundColor Cyan
Write-Host "1. 打开设置窗口，查看开机自启选项" -ForegroundColor White
Write-Host "2. 勾选/取消勾选开机自启选项，观察注册表变化" -ForegroundColor White
Write-Host "3. 右键点击系统托盘图标，查看开机自启菜单项" -ForegroundColor White
Write-Host "4. 点击托盘菜单中的开机自启项，观察状态变化" -ForegroundColor White
