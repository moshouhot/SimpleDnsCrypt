Simple DNSCrypt v0.8.0

Simple DNSCrypt 是一款针对 https://github.com/DNSCrypt/dnscrypt-proxy 的 Windows 图形化管理工具，提供简单易用的界面来配置和管理 DNS 加密服务。

最新版本特性 (v0.8.0)

• 升级到 dnscrypt-proxy 2.1.13 - 最新稳定版本，修复多个安全和性能问题
• 升级到 v3 解析器列表 - 支持更多现代化 DNS 服务和协议
• 优化启动速度 - 默认关闭自动更新检查，显著提升程序启动速度
• 简化语言支持 - 精简为中英文双语，减少程序体积
• 默认中文界面 - 针对中文用户优化，首次启动即显示中文界面
• 完整的签名验证 - 恢复 minisign 数字签名验证功能，确保更新安全性

概述

Simple DNSCrypt 允许您通过 DNSCrypt 和 DNS-over-HTTPS (DoH) 协议加密 DNS 查询。它提供了用户友好的界面来配置 dnscrypt-proxy，无需手动编辑配置文件。

功能特性

核心功能

• DNSCrypt 支持：使用 DNSCrypt 协议加密 DNS 查询

• DNS-over-HTTPS (DoH)：支持 DNS-over-HTTPS 服务器

• 服务管理：将 dnscrypt-proxy 作为 Windows 服务安装、启动、停止和卸载

• 自动模式：自动选择最佳可用 DNS 服务器

• 手动服务器选择：从精选列表中选择特定 DNS 服务器

高级功能

• 查询日志：记录 DNS 查询以进行监控和调试

• 黑名单：阻止特定域名和 IP 地址

• 匿名 DNS：通过中继路由 DNS 查询以增强隐私

• 网络接口管理：为特定网络适配器配置 DNS 设置

• 备用解析器：配置备用 DNS 服务器

• IPv4/IPv6 支持：同时支持 IPv4 和 IPv6 DNS 服务器

安全功能

• DNSSEC 验证：要求启用 DNSSEC 的服务器

• 无日志政策：筛选不记录查询的服务器

• 无过滤政策：使用未过滤的 DNS 服务器

• 临时密钥：为每个 DNS 查询生成唯一密钥（DNSCrypt）

• 禁用 TLS 会话票证：增强 DoH 连接的隐私性

系统要求

• 操作系统：Windows 7/8/10/11（32 位或 64 位）

• .NET Framework：4.8 或更高版本

• 管理员权限：需要服务安装和网络配置权限

安装指南

1. 从 https://github.com/bitbeans/SimpleDnsCrypt/releases 下载最新版本
2. 将压缩包解压到您选择的文件夹
3. 以管理员身份运行 SimpleDnsCrypt.exe
4. 应用程序将自动安装并配置 dnscrypt-proxy

使用方法

基本设置

1. 以管理员身份启动 Simple DNSCrypt
2. 点击"安装服务"来安装 dnscrypt-proxy
3. 启用"自动模式"进行简单设置，或手动选择 DNS 服务器
4. 点击"启动服务"开始 DNS 加密

配置选项

服务器选择

• 自动模式：让应用程序自动选择最佳服务器

• 手动选择：从可用的 DNSCrypt 和 DoH 服务器列表中选择

• 服务器筛选器：按协议（DNSCrypt/DoH）、日志政策和 DNSSEC 支持筛选服务器

网络配置

• 配置哪些网络适配器使用加密 DNS

• 设置自定义监听地址和端口

• 配置备用解析器以实现冗余

日志和监控

• 启用查询日志以监控 DNS 请求

• 查看 DNS 查询和响应的实时日志

• 配置日志轮换和保留策略

隐私和安全

• 启用通过中继的匿名 DNS 路由

• 配置域名和 IP 黑名单

• 设置自定义转发和伪装规则

项目结构


SimpleDnsCrypt/
├── Config/                 # 配置类和常量
├── Controls/              # 自定义 WPF 控件
├── Converters/            # WPF 值转换器
├── DNSx64/               # 64 位 dnscrypt-proxy 可执行文件
├── DNSx86/               # 32 位 dnscrypt-proxy 可执行文件
├── Extensions/           # 扩展方法和实用工具
├── Helper/               # 各种操作的辅助类
├── Images/               # 应用程序图标和图像
├── Logger/               # 日志记录基础设施
├── Models/               # 数据模型和配置类
├── Properties/           # 程序集信息和资源
├── Resources/            # 本地化文件和许可证
├── Styles/               # WPF 样式和主题
├── ViewModels/           # MVVM 视图模型
├── Views/                # WPF 用户控件和视图
├── Windows/              # WPF 窗口和对话框
└── dnscrypt-proxy/       # dnscrypt-proxy 配置和许可证


关键组件

DNS 服务器管理

• DnsCryptProxyManager：管理 dnscrypt-proxy 服务生命周期

• DnscryptProxyConfigurationManager：处理配置文件管理

• AvailableResolver：表示具有其功能的 DNS 服务器

网络接口管理

• LocalNetworkInterfaceManager：管理 Windows 网络适配器 DNS 设置

• LocalNetworkInterface：表示网络适配器及其 DNS 配置

配置模型

• DnscryptProxyConfiguration：dnscrypt-proxy 的主要配置模型

• Source：表示 DNS 服务器列表来源（公共解析器、中继）

• Route：表示匿名 DNS 路由配置

DNS 服务器来源

DOH 服务器来源分析

SimpleDnsCrypt 项目中的 DNS over HTTPS (DOH) 服务器完全来自外部源，没有内置的服务器列表。具体分析如下：

1. 外部获取机制

DNS 服务器列表通过 dnscrypt-proxy 从以下外部 URL 获取：

公共解析器列表：
• 主要源：https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/public-resolvers.md

• 备用源：https://download.dnscrypt.info/resolvers-list/v3/public-resolvers.md

• 中国镜像源：https://gitee.com/ilangel/dnscrypt-resolvers/raw/master/v3/public-resolvers.md

中继服务器列表：
• 主要源：https://github.com/DNSCrypt/dnscrypt-resolvers/raw/master/v3/relays.md

• 备用源：https://download.dnscrypt.info/resolvers-list/v3/relays.md
• 中国镜像源：https://gitee.com/ilangel/dnscrypt-resolvers/raw/master/v3/relays.md

2. 配置位置

服务器源配置在以下文件中定义：
• 配置文件：dnscrypt-proxy/dnscrypt-proxy.toml

• 代码位置：Helper/PatchHelper.cs（动态添加中继源）

3. 缓存机制

• 本地缓存：下载的服务器列表缓存为 public-resolvers.md 和 relays.md

• 更新频率：中继列表每 72 小时刷新一次（refresh_delay = 72）

• 签名验证：使用 minisign 密钥验证文件完整性

4. 相关代码文件

• Helper/DnsCryptProxyManager.cs - 获取可用解析器列表

• Helper/RelayHelper.cs - 处理中继服务器

• Models/DnscryptProxyConfiguration.cs - 配置模型

• Models/AvailableResolver.cs - 解析器数据模型

5. 工作流程

1. dnscrypt-proxy 启动时从配置的 URL 下载服务器列表
2. 使用 minisign 验证下载文件的签名
3. 将验证后的列表缓存到本地文件
4. SimpleDnsCrypt 通过调用 dnscrypt-proxy -list -json 获取可用服务器
5. 应用程序解析 JSON 响应并显示在用户界面中

6. 安全特性

• 数字签名：所有服务器列表都使用 minisign 进行数字签名验证

• 多源备份：每个列表都有主要和备用下载源

• 定期更新：确保服务器信息的时效性

结论：SimpleDnsCrypt 项目不包含任何内置的 DOH 服务器，所有服务器信息都是从 DNSCrypt 项目的官方仓库动态获取的，确保了服务器列表的实时性和安全性。

中继服务器功能详解

中继服务器（Relay Servers）是 SimpleDnsCrypt 提供的匿名 DNS 功能，通过中继路由 DNS 查询来增强用户隐私保护。

1. 功能概述

中继服务器功能允许用户通过一个或多个中继节点来路由 DNS 查询，实现以下目标：
• 隐藏用户真实 IP 地址：DNS 服务器只能看到中继服务器的 IP
• 增强隐私保护：防止 DNS 服务器直接关联用户身份
• 地理位置混淆：通过不同地区的中继服务器路由查询
• 抗审查能力：绕过某些网络限制和封锁

2. 默认状态和配置

默认状态：
• 中继功能默认未启用，需要用户主动配置
• 配置文件中 skip_incompatible = false（不跳过不兼容的中继）
• 只有用户为特定解析器配置路由后才会生效

配置位置：
• 主配置：dnscrypt-proxy/dnscrypt-proxy.toml 中的 [anonymized_dns] 部分
• 代码配置：Helper/PatchHelper.cs 中动态添加中继源
• 路由配置：存储在 DnscryptProxyConfiguration.anonymized_dns.routes 中

3. 相关代码文件

核心模型类：
• Models/DnscryptProxyConfiguration.cs - AnonymizedDns 和 Route 类定义
  - AnonymizedDns：匿名 DNS 配置容器
  - Route：单个路由配置（server_name + via 中继列表）

管理和辅助类：
• Helper/RelayHelper.cs - 中继服务器列表获取和处理
• ViewModels/RouteViewModel.cs - 路由管理界面的视图模型
• ViewModels/MainViewModel.cs - HandleManageRoutes 方法处理路由管理

用户界面：
• Views/RouteView.xaml - 路由配置对话框界面
• Views/MainView.xaml - 主界面中的"管理路由"按钮（Shuffle 图标）

转换器：
• Converters/RouteStateToColorConverter.cs - 路由状态颜色指示器

4. 用户操作流程

启用中继路由：
1. 在主界面的解析器列表中选择支持 DNSCrypt 协议的服务器
2. 点击解析器旁边的"管理路由"按钮（Shuffle 图标）
3. 在路由配置对话框中，从下方的中继列表拖拽中继服务器到上方的路由列表
4. 可以配置多个中继服务器形成链式路由
5. 点击确定保存配置

禁用中继路由：
1. 再次点击"管理中继服务器"按钮
2. 删除路由列表中的所有中继服务器
3. 点击确定保存，该解析器将恢复直连模式

5. 技术实现细节

中继服务器获取：
• 从 v3/relays.md 获取最新的中继服务器列表
• 使用 minisign 验证文件签名确保安全性
• 每 72 小时自动刷新中继列表

路由配置存储：
```csharp
// 路由配置数据结构
public class Route {
    public string server_name;           // 目标解析器名称
    public ObservableCollection<string> via;  // 中继服务器名称列表
}
```

路由状态指示：
• 绿色：已配置中继路由
• 灰色：未配置中继路由
• 按钮工具提示显示当前路由状态

6. 安全和性能考虑

安全特性：
• 所有中继服务器都经过 DNSCrypt 项目验证
• 支持多跳中继增强匿名性
• 自动跳过不兼容的中继服务器

性能影响：
• 中继路由会增加 DNS 查询延迟
• 建议选择地理位置较近的中继服务器
• 可以通过查询日志监控性能影响

7. 配置示例

TOML 配置文件示例：
```toml
[anonymized_dns]
skip_incompatible = false

[[anonymized_dns.routes]]
server_name = "cloudflare"
via = ["anon-cs-fr", "anon-cs-se"]
```

对应的用户操作：
1. 选择 cloudflare 解析器
2. 配置通过 anon-cs-fr 和 anon-cs-se 两个中继的路由
3. DNS 查询路径：用户 → anon-cs-fr → anon-cs-se → cloudflare

8. 故障排除

常见问题：
• 中继路由配置后查询变慢：正常现象，可减少中继跳数
• 某些中继不可用：系统会自动跳过不兼容的中继
• 路由配置丢失：检查配置文件权限和格式

调试方法：
• 启用查询日志查看路由路径
• 检查 dnscrypt-proxy 日志文件
• 使用 dnscrypt-proxy -check 验证配置

总结：中继服务器功能为高级用户提供了强大的隐私保护能力，通过简单的拖拽界面即可配置复杂的匿名路由，在隐私保护和使用便利性之间取得了良好平衡。

本地化支持

Simple DNSCrypt 支持以下语言：
• 简体中文（默认）- 针对中文用户优化的界面
• English - 国际标准英文界面

注：为了减少程序体积和提升启动速度，当前版本只保留了中英文两种语言支持。

依赖项

• Caliburn.Micro：MVVM 框架

• MahApps.Metro：现代 UI 框架

• Newtonsoft.Json：JSON 序列化

• NLog：日志记录框架

• YamlDotNet：YAML 配置解析

• Nett：TOML 配置解析

许可证

本项目采用 MIT 许可证。所有依赖项的许可证请参见 Resources/Licenses/ 文件夹。

贡献指南

欢迎贡献！请随时提交问题和拉取请求。

支持

如需支持和问题解答，请访问 https://github.com/bitbeans/SimpleDnsCrypt。