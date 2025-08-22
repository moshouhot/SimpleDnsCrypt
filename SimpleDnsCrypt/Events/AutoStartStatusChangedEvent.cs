namespace SimpleDnsCrypt.Events
{
    /// <summary>
    /// 开机自启动状态变化事件
    /// </summary>
    public class AutoStartStatusChangedEvent
    {
        /// <summary>
        /// 开机自启动是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isEnabled">开机自启动是否启用</param>
        public AutoStartStatusChangedEvent(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }
}
