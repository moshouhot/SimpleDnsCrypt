# Gitee 备用源配置说明

## 概述
为了提升中国用户的访问速度和可靠性，SimpleDnsCrypt 现已添加 Gitee 作为备用源。

## 配置的备用源

### 1. 公共解析器列表
- **主要源**: https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/public-resolvers.md
- **官方备用源**: https://download.dnscrypt.info/resolvers-list/v3/public-resolvers.md
- **🆕 Gitee 镜像源**: https://gitee.com/ilangel/dnscrypt-resolvers/raw/master/v3/public-resolvers.md

### 2. 中继服务器列表
- **主要源**: https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/relays.md
- **官方备用源**: https://download.dnscrypt.info/resolvers-list/v3/relays.md
- **🆕 Gitee 镜像源**: https://gitee.com/ilangel/dnscrypt-resolvers/raw/master/v3/relays.md

## 修改的文件

### 1. dnscrypt-proxy/dnscrypt-proxy.toml
```toml
[sources.'public-resolvers']
urls = [
    'https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/public-resolvers.md',
    'https://download.dnscrypt.info/resolvers-list/v3/public-resolvers.md',
    'https://gitee.com/ilangel/dnscrypt-resolvers/raw/master/v3/public-resolvers.md'
]

[sources.'relays']
urls = [
    'https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/relays.md',
    'https://download.dnscrypt.info/resolvers-list/v3/relays.md',
    'https://gitee.com/ilangel/dnscrypt-resolvers/raw/master/v3/relays.md'
]
```

### 2. Helper/PatchHelper.cs
- 为版本 0.6.8-0.7.0 和 0.7.1 的补丁添加了 Gitee 源
- 新增 0.8.0 版本补丁，自动为现有用户添加 Gitee 备用源

### 3. README.md
- 更新了文档，说明新增的 Gitee 镜像源

## 工作原理

### 1. 故障转移机制
dnscrypt-proxy 会按顺序尝试 URLs 列表中的源：
1. 首先尝试 GitHub 原始文件服务
2. 如果失败，尝试官方备用源
3. 如果仍然失败，尝试 Gitee 镜像源

### 2. 自动更新
- 新安装的用户：直接使用包含 Gitee 源的配置
- 现有用户：通过 PatchHelper 0.8.0 补丁自动添加 Gitee 源

### 3. 缓存机制
- 成功下载的列表会缓存到本地
- 中继列表每 72 小时刷新一次
- 使用 minisign 验证文件完整性

## 优势

### 1. 提升访问速度
- 🇨🇳 **中国用户**: Gitee 服务器在国内，访问速度更快
- 🌍 **国际用户**: 不影响原有的访问方式

### 2. 增强可靠性
- **多重备份**: 三个独立的源确保高可用性
- **自动故障转移**: 一个源失败时自动切换到下一个

### 3. 保持同步
- Gitee 镜像与官方仓库保持同步
- 确保获取最新的服务器列表

## 验证方法

### 1. 检查配置文件
```bash
# 查看 dnscrypt-proxy.toml 配置
cat dnscrypt-proxy/dnscrypt-proxy.toml | grep -A 5 "sources"
```

### 2. 测试网络连接
```bash
# 测试 Gitee 源的可访问性
curl -I https://gitee.com/ilangel/dnscrypt-resolvers/raw/master/v3/public-resolvers.md
curl -I https://gitee.com/ilangel/dnscrypt-resolvers/raw/master/v3/relays.md
```

### 3. 查看日志
启动 SimpleDnsCrypt 后，检查日志文件中的源下载信息。

## 注意事项

### 1. 签名验证
- 所有源使用相同的 minisign 密钥验证
- 确保文件完整性和安全性

### 2. 更新频率
- 中继列表：每 72 小时自动刷新
- 公共解析器列表：按需更新

### 3. 兼容性
- 完全向后兼容现有配置
- 不影响现有用户的使用体验

## 故障排除

### 1. 如果 Gitee 源无法访问
- 程序会自动回退到 GitHub 和官方源
- 不会影响正常功能

### 2. 如果所有源都无法访问
- 使用本地缓存的服务器列表
- 建议检查网络连接

### 3. 配置更新问题
- 确保程序有写入配置文件的权限
- 检查 PatchHelper 是否正确执行

## 总结

添加 Gitee 备用源是一个重要的改进，特别是对中国用户而言：

✅ **提升速度**: 国内访问更快
✅ **增强可靠性**: 多重备份机制
✅ **保持兼容**: 不影响现有功能
✅ **自动更新**: 现有用户自动获得改进

这个改进确保了 SimpleDnsCrypt 在各种网络环境下都能稳定运行。
