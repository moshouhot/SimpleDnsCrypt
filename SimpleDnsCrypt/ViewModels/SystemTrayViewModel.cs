using Caliburn.Micro;
using SimpleDnsCrypt.Helper;
using SimpleDnsCrypt.Events;
using System.Windows;

namespace SimpleDnsCrypt.ViewModels
{
	public class SystemTrayViewModel : Screen, IHandle<AutoStartStatusChangedEvent>
	{
		private readonly IWindowManager _windowManager;
		private readonly MainViewModel _mainViewModel;
		private readonly IEventAggregator _events;
		private bool _isAutoStartEnabled;

		public SystemTrayViewModel(IWindowManager windowManager, IEventAggregator events, MainViewModel mainViewModel)
		{
			_windowManager = windowManager;
			_events = events;
			_mainViewModel = mainViewModel;

			// 订阅开机自启动状态变化事件
			_events.Subscribe(this);

			// 初始化开机自启动状态
			UpdateAutoStartStatus();
		}

		protected override void OnActivate()
		{
			base.OnActivate();

			NotifyOfPropertyChange(() => CanShowWindow);
			NotifyOfPropertyChange(() => CanHideWindow);
			UpdateAutoStartStatus();
		}

		/// <summary>
		/// 更新开机自启动状态
		/// </summary>
		private void UpdateAutoStartStatus()
		{
			_isAutoStartEnabled = AutoStartHelper.IsAutoStartEnabled();
			NotifyOfPropertyChange(() => IsAutoStartEnabled);
			NotifyOfPropertyChange(() => AutoStartMenuText);
		}

		/// <summary>
		/// 开机自启动状态
		/// </summary>
		public bool IsAutoStartEnabled
		{
			get => _isAutoStartEnabled;
			set
			{
				_isAutoStartEnabled = value;
				NotifyOfPropertyChange(() => IsAutoStartEnabled);
				NotifyOfPropertyChange(() => AutoStartMenuText);
			}
		}

		/// <summary>
		/// 开机自启动菜单项显示文本
		/// </summary>
		public string AutoStartMenuText
		{
			get
			{
				var text = LocalizationEx.GetUiString("trayicon_auto_start", System.Threading.Thread.CurrentThread.CurrentCulture);
				return _isAutoStartEnabled ? $"✓ {text}" : $"X {text}" ;
			}
		}

		public void ShowWindow()
		{
			if (!_mainViewModel.IsActive)
			{
				_windowManager.ShowWindow(_mainViewModel);
			}
			NotifyOfPropertyChange(() => CanShowWindow);
			NotifyOfPropertyChange(() => CanHideWindow);
		}

		public bool CanShowWindow => !_mainViewModel.IsActive;

		public void HideWindow()
		{
			_mainViewModel.TryClose();

			NotifyOfPropertyChange(() => CanShowWindow);
			NotifyOfPropertyChange(() => CanHideWindow);
		}

		public bool CanHideWindow => _mainViewModel.IsActive;

		/// <summary>
		/// 切换开机自启动状态
		/// </summary>
		public void ToggleAutoStart()
		{
			var newStatus = AutoStartHelper.ToggleAutoStart();

			// 更新配置文件中的设置
			Properties.Settings.Default.AutoStart = newStatus;
			Properties.Settings.Default.Save();

			// 发布状态变化事件，通知其他组件（如设置窗口）
			_events.PublishOnUIThread(new AutoStartStatusChangedEvent(newStatus));

			// 更新状态显示
			UpdateAutoStartStatus();
		}

		public void ExitApplication()
		{
			Application.Current.Shutdown();
		}

		/// <summary>
		/// 处理开机自启动状态变化事件
		/// </summary>
		/// <param name="message">事件消息</param>
		public void Handle(AutoStartStatusChangedEvent message)
		{
			// 更新本地状态（不触发注册表操作，避免循环）
			_isAutoStartEnabled = message.IsEnabled;
			NotifyOfPropertyChange(() => IsAutoStartEnabled);
			NotifyOfPropertyChange(() => AutoStartMenuText);
		}
	}
}
