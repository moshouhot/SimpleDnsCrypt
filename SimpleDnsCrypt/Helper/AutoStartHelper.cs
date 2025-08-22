using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using Caliburn.Micro;

namespace SimpleDnsCrypt.Helper
{
    /// <summary>
    /// 开机自启动管理辅助类
    /// </summary>
    public static class AutoStartHelper
    {
        private static readonly ILog Log = LogManagerHelper.Factory();
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ApplicationName = "SimpleDnsCrypt";

        /// <summary>
        /// 获取当前应用程序的可执行文件路径
        /// </summary>
        /// <returns>可执行文件的完整路径</returns>
        private static string GetApplicationPath()
        {
            return Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");
        }

        /// <summary>
        /// 检查是否已启用开机自启动
        /// </summary>
        /// <returns>如果已启用返回true，否则返回false</returns>
        public static bool IsAutoStartEnabled()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false))
                {
                    if (key == null) return false;
                    
                    var value = key.GetValue(ApplicationName) as string;
                    var currentPath = GetApplicationPath();
                    
                    // 检查注册表中的路径是否与当前应用程序路径匹配
                    return !string.IsNullOrEmpty(value) && 
                           string.Equals(value.Trim('"'), currentPath, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 启用开机自启动
        /// </summary>
        /// <returns>操作是否成功</returns>
        public static bool EnableAutoStart()
        {
            try
            {
                var applicationPath = GetApplicationPath();
                
                if (!File.Exists(applicationPath))
                {
                    Log.Error(new FileNotFoundException($"应用程序文件不存在: {applicationPath}"));
                    return false;
                }

                using (var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
                {
                    if (key == null)
                    {
                        Log.Error(new InvalidOperationException("无法打开注册表项进行写入"));
                        return false;
                    }

                    // 设置注册表值，路径用引号包围以处理包含空格的路径
                    key.SetValue(ApplicationName, $"\"{applicationPath}\"", RegistryValueKind.String);
                    Log.Info($"已启用开机自启动: {applicationPath}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 禁用开机自启动
        /// </summary>
        /// <returns>操作是否成功</returns>
        public static bool DisableAutoStart()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
                {
                    if (key == null)
                    {
                        Log.Warn("注册表项不存在，可能已经禁用了开机自启动");
                        return true;
                    }

                    // 检查值是否存在
                    if (key.GetValue(ApplicationName) != null)
                    {
                        key.DeleteValue(ApplicationName, false);
                        Log.Info("已禁用开机自启动");
                    }
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 切换开机自启动状态
        /// </summary>
        /// <returns>操作后的状态（true=已启用，false=已禁用）</returns>
        public static bool ToggleAutoStart()
        {
            try
            {
                if (IsAutoStartEnabled())
                {
                    DisableAutoStart();
                    return false;
                }
                else
                {
                    EnableAutoStart();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return IsAutoStartEnabled(); // 返回当前状态
            }
        }
    }
}
