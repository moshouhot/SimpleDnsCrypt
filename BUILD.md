# SimpleDnsCrypt 构建指南

本文档说明如何使用GitHub Actions自动构建SimpleDnsCrypt的MSI安装包和ZIP绿色版。

## 🚀 快速开始

### 自动构建（推荐）

1. **创建发布版本**：
   ```bash
   git tag v0.8.0
   git push origin v0.8.0
   ```

2. **手动触发构建**：
   - 访问GitHub仓库的Actions页面
   - 选择"Build and Release"工作流
   - 点击"Run workflow"
   - 输入版本号（如：0.8.0）

### 本地构建

使用提供的PowerShell脚本：

```powershell
# 构建所有包（ZIP + MSI）
.\build-package.ps1 -Version 0.8.0 -All

# 只构建ZIP绿色版
.\build-package.ps1 -Version 0.8.0 -CreateZIP

# 跳过编译，只打包现有文件
.\build-package.ps1 -SkipBuild -CreateZIP
```

## 📦 构建输出

### ZIP绿色版
- `SimpleDnsCrypt-{版本}-x64-portable.zip` - 64位绿色版

### MSI安装包
- `SimpleDnsCrypt-{版本}-x64-setup.msi` - 64位安装包
- 安装路径：`C:\Program Files\bitbeans\Simple DNSCrypt x64`

## 🔧 环境要求

### GitHub Actions（自动构建）
- ✅ 已配置完成，无需额外设置
- 自动安装所需工具和依赖

### 本地构建环境
- Windows 10/11
- .NET Framework 4.8 SDK
- Visual Studio 2019/2022 或 Build Tools
- NuGet CLI
- WiX Toolset v3.11（仅MSI构建需要）

## 📋 工作流说明

### 1. 构建测试工作流 (`build-test.yml`)
**触发条件**：
- 推送到主分支（main/master/develop）
- 创建Pull Request

**功能**：
- 编译Debug和Release版本
- 验证构建成功
- 上传构建产物供测试

### 2. 发布构建工作流 (`build-release.yml`)
**触发条件**：
- 推送标签（v*）
- 手动触发

**功能**：
- 编译Release版本（x64和x86）
- 创建ZIP绿色版
- 创建MSI安装包
- 自动创建GitHub Release

## 🛠 自定义配置

### 修改MSI安装包信息

编辑 `.github/workflows/build-release.yml` 中的WiX配置：

```xml
<Product Id="*" 
         Name="SimpleDnsCrypt" 
         Language="1033" 
         Version="$env:VERSION.0" 
         Manufacturer="你的公司名称" 
         UpgradeCode="你的升级代码">
```

### 添加代码签名

1. 将代码签名证书添加到GitHub Secrets
2. 在工作流中添加签名步骤：

```yaml
- name: Sign executables
  run: |
    signtool sign /f certificate.pfx /p ${{ secrets.CERT_PASSWORD }} /t http://timestamp.digicert.com SimpleDnsCrypt.exe
```

### 修改构建配置

在 `SimpleDnsCrypt.csproj` 中调整：
- 目标框架
- 平台配置
- 输出路径
- 依赖包版本

## 🔍 故障排除

### 常见问题

1. **NuGet包还原失败**
   ```bash
   nuget restore SimpleDnsCrypt.sln
   ```

2. **MSBuild找不到**
   - 安装Visual Studio Build Tools
   - 或安装完整的Visual Studio

3. **WiX工具集未找到**
   - 下载安装：https://github.com/wixtoolset/wix3/releases
   - 确保安装到默认路径

4. **构建失败**
   - 检查.NET Framework 4.8是否安装
   - 确认所有NuGet包已正确还原
   - 查看详细错误日志

### 调试构建

启用详细日志：
```powershell
msbuild SimpleDnsCrypt.sln /p:Configuration=Release /p:Platform=x64 /verbosity:detailed
```

## 📝 版本管理

### 语义化版本
使用语义化版本号：`主版本.次版本.修订版本`

示例：
- `v1.0.0` - 首个正式版本
- `v1.1.0` - 新功能版本
- `v1.1.1` - 修复版本

### 发布流程
1. 更新版本号相关文件
2. 提交更改
3. 创建标签：`git tag v1.0.0`
4. 推送标签：`git push origin v1.0.0`
5. GitHub Actions自动构建并发布

## 🎯 下一步计划

- [ ] 添加自动化测试
- [ ] 集成代码签名
- [ ] 支持多语言构建
- [ ] 添加更新检查机制
- [ ] 优化MSI安装包配置

## 📞 支持

如有问题，请：
1. 查看GitHub Actions构建日志
2. 检查本文档的故障排除部分
3. 在GitHub仓库创建Issue
