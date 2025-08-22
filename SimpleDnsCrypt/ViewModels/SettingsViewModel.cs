using Caliburn.Micro;
using SimpleDnsCrypt.Models;
using SimpleDnsCrypt.Helper;
using SimpleDnsCrypt.Events;
using System.ComponentModel.Composition;

namespace SimpleDnsCrypt.ViewModels
{
	[Export(typeof(SettingsViewModel))]
	public class SettingsViewModel : Screen, IHandle<AutoStartStatusChangedEvent>
	{
		private readonly IWindowManager _windowManager;
		private readonly IEventAggregator _events;
		private string _windowTitle;
		private bool _isAdvancedSettingsTabVisible;
		private bool _isStartInTrayEnabled;
		private bool _isTrayModeEnabled;
		private bool _isQueryLogTabVisible;
		private bool _isDomainBlacklistTabVisible;
		private bool _isDomainBlockLogTabVisible;
		private bool _isAddressBlacklistTabVisible;
		private bool _isAddressBlockLogTabVisible;
		private bool _isCloakAndForwardTabVisible;
		private bool _isAutoUpdateEnabled;
		private bool _isAutoStartEnabled;
		private bool _backupAndRestoreConfigOnUpdate;

		private UpdateType _selectedUpdateType;

		public SettingsViewModel()
		{
		}

		[ImportingConstructor]
		public SettingsViewModel(IWindowManager windowManager, IEventAggregator events)
		{
			_windowManager = windowManager;
			_events = events;
			_events.Subscribe(this);
			_isAdvancedSettingsTabVisible = Properties.Settings.Default.IsAdvancedSettingsTabVisible;
			_isStartInTrayEnabled = Properties.Settings.Default.StartInTray;
			_isTrayModeEnabled = Properties.Settings.Default.TrayMode;
			_isQueryLogTabVisible = Properties.Settings.Default.IsQueryLogTabVisible;
			_isDomainBlacklistTabVisible = Properties.Settings.Default.IsDomainBlacklistTabVisible;
			_isDomainBlockLogTabVisible = Properties.Settings.Default.IsDomainBlockLogTabVisible;
			_isAddressBlacklistTabVisible = Properties.Settings.Default.IsAddressBlacklistTabVisible;
			_isAddressBlockLogTabVisible = Properties.Settings.Default.IsAddressBlockLogTabVisible;
			_isCloakAndForwardTabVisible = Properties.Settings.Default.IsCloakAndForwardTabVisible;
			_isAutoUpdateEnabled = Properties.Settings.Default.AutoUpdate;
			_isAutoStartEnabled = Properties.Settings.Default.AutoStart;
			_selectedUpdateType = (UpdateType)Properties.Settings.Default.MinUpdateType;
			_backupAndRestoreConfigOnUpdate = Properties.Settings.Default.BackupAndRestoreConfigOnUpdate;

			// 同步开机自启动状态
			SyncAutoStartStatus();
		}

		/// <summary>
		///     The title of the window.
		/// </summary>
		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				NotifyOfPropertyChange(() => WindowTitle);
			}
		}

		public UpdateType SelectedUpdateType
		{
			get => _selectedUpdateType;
			set
			{
				_selectedUpdateType = value;
				Properties.Settings.Default.MinUpdateType = (int)_selectedUpdateType;
				NotifyOfPropertyChange(() => SelectedUpdateType);
			}
		}

		public bool IsCloakAndForwardTabVisible
		{
			get => _isCloakAndForwardTabVisible;
			set
			{
				_isCloakAndForwardTabVisible = value;
				Properties.Settings.Default.IsCloakAndForwardTabVisible = _isCloakAndForwardTabVisible;
				NotifyOfPropertyChange(() => IsCloakAndForwardTabVisible);
			}
		}

		public bool IsAdvancedSettingsTabVisible
		{
			get => _isAdvancedSettingsTabVisible;
			set
			{
				_isAdvancedSettingsTabVisible = value;
				Properties.Settings.Default.IsAdvancedSettingsTabVisible = _isAdvancedSettingsTabVisible;
				NotifyOfPropertyChange(() => IsAdvancedSettingsTabVisible);
			}
		}

		public bool IsStartInTrayEnabled
		{
			get => _isStartInTrayEnabled;
			set
			{
				_isStartInTrayEnabled = value;
				Properties.Settings.Default.StartInTray = _isStartInTrayEnabled;
				NotifyOfPropertyChange(() => IsStartInTrayEnabled);
			}
		}
		public bool IsTrayModeEnabled
		{
			get => _isTrayModeEnabled;
			set
			{
				_isTrayModeEnabled = value;
				Properties.Settings.Default.TrayMode = _isTrayModeEnabled;
				NotifyOfPropertyChange(() => IsTrayModeEnabled);
				if (!IsTrayModeEnabled && IsStartInTrayEnabled) IsStartInTrayEnabled = false;
			}
		}

		public bool IsQueryLogTabVisible
		{
			get => _isQueryLogTabVisible;
			set
			{
				_isQueryLogTabVisible = value;
				Properties.Settings.Default.IsQueryLogTabVisible = _isQueryLogTabVisible;
				NotifyOfPropertyChange(() => IsQueryLogTabVisible);
			}
		}

		public bool IsDomainBlockLogTabVisible
		{
			get => _isDomainBlockLogTabVisible;
			set
			{
				_isDomainBlockLogTabVisible = value;
				Properties.Settings.Default.IsDomainBlockLogTabVisible = _isDomainBlockLogTabVisible;
				NotifyOfPropertyChange(() => IsDomainBlockLogTabVisible);
			}
		}

		public bool IsDomainBlacklistTabVisible
		{
			get => _isDomainBlacklistTabVisible;
			set
			{
				_isDomainBlacklistTabVisible = value;
				Properties.Settings.Default.IsDomainBlacklistTabVisible = _isDomainBlacklistTabVisible;
				NotifyOfPropertyChange(() => IsDomainBlacklistTabVisible);
			}
		}

		public bool IsAddressBlockLogTabVisible
		{
			get => _isAddressBlockLogTabVisible;
			set
			{
				_isAddressBlockLogTabVisible = value;
				Properties.Settings.Default.IsAddressBlockLogTabVisible = _isAddressBlockLogTabVisible;
				NotifyOfPropertyChange(() => IsAddressBlockLogTabVisible);
			}
		}

		public bool IsAddressBlacklistTabVisible
		{
			get => _isAddressBlacklistTabVisible;
			set
			{
				_isAddressBlacklistTabVisible = value;
				Properties.Settings.Default.IsAddressBlacklistTabVisible = _isAddressBlacklistTabVisible;
				NotifyOfPropertyChange(() => IsAddressBlacklistTabVisible);
			}
		}

		public bool IsAutoUpdateEnabled
		{
			get => _isAutoUpdateEnabled;
			set
			{
				_isAutoUpdateEnabled = value;
				Properties.Settings.Default.AutoUpdate = _isAutoUpdateEnabled;
				NotifyOfPropertyChange(() => IsAutoUpdateEnabled);
			}
		}

		public bool IsAutoStartEnabled
		{
			get => _isAutoStartEnabled;
			set
			{
				_isAutoStartEnabled = value;
				Properties.Settings.Default.AutoStart = _isAutoStartEnabled;

				// 应用开机自启动设置
				if (_isAutoStartEnabled)
				{
					AutoStartHelper.EnableAutoStart();
				}
				else
				{
					AutoStartHelper.DisableAutoStart();
				}

				// 发布状态变化事件，通知其他组件
				_events.PublishOnUIThread(new AutoStartStatusChangedEvent(_isAutoStartEnabled));

				NotifyOfPropertyChange(() => IsAutoStartEnabled);
			}
		}

		public bool BackupAndRestoreConfigOnUpdate
		{
			get => _backupAndRestoreConfigOnUpdate;
			set
			{
				_backupAndRestoreConfigOnUpdate = value;
				Properties.Settings.Default.BackupAndRestoreConfigOnUpdate = _backupAndRestoreConfigOnUpdate;
				NotifyOfPropertyChange(() => BackupAndRestoreConfigOnUpdate);
			}
		}

		/// <summary>
		/// 同步开机自启动状态
		/// </summary>
		private void SyncAutoStartStatus()
		{
			// 检查实际的注册表状态
			var actualAutoStartEnabled = AutoStartHelper.IsAutoStartEnabled();

			// 如果配置文件和实际状态不一致，以配置文件为准，应用到系统
			if (_isAutoStartEnabled != actualAutoStartEnabled)
			{
				if (_isAutoStartEnabled)
				{
					AutoStartHelper.EnableAutoStart();
				}
				else
				{
					AutoStartHelper.DisableAutoStart();
				}
				NotifyOfPropertyChange(() => IsAutoStartEnabled);
			}
		}

		/// <summary>
		/// 处理开机自启动状态变化事件
		/// </summary>
		/// <param name="message">事件消息</param>
		public void Handle(AutoStartStatusChangedEvent message)
		{
			// 只有当状态真的不同时才更新，避免循环事件
			if (_isAutoStartEnabled != message.IsEnabled)
			{
				_isAutoStartEnabled = message.IsEnabled;
				Properties.Settings.Default.AutoStart = _isAutoStartEnabled;
				Properties.Settings.Default.Save();
				NotifyOfPropertyChange(() => IsAutoStartEnabled);
			}
		}
	}
}