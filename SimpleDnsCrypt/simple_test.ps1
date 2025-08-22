# 简单测试开机自启动功能
Write-Host "=== SimpleDnsCrypt 开机自启动功能测试 ===" -ForegroundColor Green

# 检查注册表项
$registryPath = "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
$appName = "SimpleDnsCrypt"

Write-Host "检查当前开机自启动状态:" -ForegroundColor Yellow
$currentValue = Get-ItemProperty -Path $registryPath -Name $appName -ErrorAction SilentlyContinue
if ($currentValue) {
    Write-Host "开机自启动已启用" -ForegroundColor Green
    Write-Host "路径: $($currentValue.$appName)" -ForegroundColor Gray
} else {
    Write-Host "开机自启动未启用" -ForegroundColor Red
}

Write-Host "测试完成" -ForegroundColor Green
