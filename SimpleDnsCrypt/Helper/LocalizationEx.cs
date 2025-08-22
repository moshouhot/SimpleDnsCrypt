using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using SimpleDnsCrypt.Models;
using WPFLocalizeExtension.Engine;

namespace SimpleDnsCrypt.Helper
{
	/// <summary>
	///     Class for translation management.
	/// </summary>
	public static class LocalizationEx
	{
		/// <summary>
		///     Get a translated string by key and culture.
		/// </summary>
		/// <param name="key">The key to retrieve.</param>
		/// <param name="culture">The culture to retrieve the key from.</param>
		/// <returns>Translated string.</returns>
		public static string GetUiString(string key, CultureInfo culture)
		{
			return (string)LocalizeDictionary.Instance.GetLocalizedObject("simplednscrypt", "Translation", key, culture);
		}

		/// <summary>
		///		Get the supported languages.
		/// </summary>
		/// <returns></returns>
		public static ObservableCollection<Language> GetSupportedLanguages()
		{
			var supportedLanguages = new ObservableCollection<Language>
			{
				new Language {Name = "简体中文", ShortCode = "zh-Hans", CultureCode = "zh-Hans"},
				new Language {Name = "English", ShortCode = "en", CultureCode = "en-US"}
			};
			return supportedLanguages;
		}

		/// <summary>
		///     Sets the localization culture.
		/// </summary>
		/// <param name="culture">ISO code of the new culture.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static CultureInfo SetCulture(string culture)
		{
			var ci = CultureInfo.InvariantCulture;
			try
			{
				ci = new CultureInfo(culture);
			}
			catch (CultureNotFoundException)
			{
				try
				{
					// Try language without region
					ci = new CultureInfo(culture.Substring(0, 2));
				}
				catch (Exception)
				{
					ci = CultureInfo.InvariantCulture;
				}
			}
			finally
			{
				LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
				LocalizeDictionary.Instance.Culture = ci;
				// fixes the culture in threads
				CultureInfo.DefaultThreadCurrentCulture = ci;
				CultureInfo.DefaultThreadCurrentUICulture = ci;
				Thread.CurrentThread.CurrentCulture = ci;
				Thread.CurrentThread.CurrentUICulture = ci;
			}
			return ci;
		}
	}
}
