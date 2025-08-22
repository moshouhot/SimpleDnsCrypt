# 开机自启动状态同步测试指南

## 问题解决方案

### 1. 分隔线修复
- **问题**: 分隔线左边缺少一段
- **解决**: 为Separator添加自定义样式，确保完整显示

### 2. 状态同步机制
- **问题**: 设置窗口与托盘菜单状态不同步
- **解决**: 使用Caliburn.Micro事件聚合器实现双向同步

## 技术实现

### 事件驱动架构
```csharp
// 事件定义
public class AutoStartStatusChangedEvent
{
    public bool IsEnabled { get; set; }
    public AutoStartStatusChangedEvent(bool isEnabled) => IsEnabled = isEnabled;
}

// 发布事件
_events.PublishOnUIThread(new AutoStartStatusChangedEvent(newStatus));

// 处理事件
public void Handle(AutoStartStatusChangedEvent message)
{
    // 更新本地状态
    _isAutoStartEnabled = message.IsEnabled;
    NotifyOfPropertyChange(() => IsAutoStartEnabled);
}
```

### 同步流程
1. **设置窗口修改** → 发布事件 → 托盘菜单更新
2. **托盘菜单修改** → 发布事件 → 设置窗口更新
3. **避免循环** → 检查状态差异再更新

## 测试步骤

### 1. 分隔线测试
1. 右键点击系统托盘SimpleDnsCrypt图标
2. 观察分隔线是否完整显示
3. 确认分隔线从左边缘到右边缘完整

### 2. 状态同步测试

#### 测试A：设置窗口 → 托盘菜单
1. 打开SimpleDnsCrypt主界面
2. 点击"设置"按钮
3. 勾选/取消勾选"开机自启"选项
4. 右键点击托盘图标
5. 观察托盘菜单中的开机自启状态是否同步

#### 测试B：托盘菜单 → 设置窗口
1. 右键点击托盘图标
2. 点击"开机自启"菜单项
3. 打开设置窗口
4. 观察设置中的开机自启选项是否同步

#### 测试C：注册表验证
1. 修改开机自启状态
2. 使用命令验证注册表：
   ```cmd
   reg query "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v SimpleDnsCrypt
   ```
3. 确认注册表状态与界面一致

## 预期结果

### 分隔线显示
```
┌─────────────────────┐
│✓ 开机自启          │
├─────────────────────┤  ← 完整的分隔线
│退出                │
└─────────────────────┘
```

### 状态同步
- ✅ 设置窗口修改 → 托盘菜单立即更新
- ✅ 托盘菜单修改 → 设置窗口立即更新
- ✅ 注册表状态与界面状态一致
- ✅ 程序重启后状态正确保持

## 技术细节

### 事件聚合器优势
1. **解耦合**: 组件间不直接依赖
2. **可扩展**: 易于添加新的监听者
3. **线程安全**: PublishOnUIThread确保UI线程更新
4. **避免循环**: 通过状态检查防止无限循环

### 防循环机制
```csharp
// 在Handle方法中检查状态差异
if (_isAutoStartEnabled != message.IsEnabled)
{
    // 只有状态真的不同时才更新
    _isAutoStartEnabled = message.IsEnabled;
    // 更新UI和配置
}
```

## 故障排除

### 如果同步不工作
1. 检查事件订阅是否正确
2. 确认PublishOnUIThread调用
3. 验证Handle方法实现
4. 检查是否有异常阻止事件传播

### 如果出现循环更新
1. 确认Handle方法中有状态检查
2. 避免在Handle中再次发布相同事件
3. 检查事件发布的时机

## 成功标准
- ✅ 分隔线完整显示
- ✅ 双向状态同步正常
- ✅ 无循环更新问题
- ✅ 注册表状态一致
- ✅ 重启后状态保持
