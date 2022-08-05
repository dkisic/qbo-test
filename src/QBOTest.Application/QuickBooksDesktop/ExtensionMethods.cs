using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop
{
	public static class ExtensionMethods
	{
		
		public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
		{
			if (cacheManager.IsSet(key))
			{
				return cacheManager.Get<T>(key);
			}

			var result = acquire();
			cacheManager.Set(key, result, cacheTime);
			return result;
		}


		public static int ToInt0(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return 0;

			int result;
			if (Int32.TryParse(value, out result))
				return result;
			return 0;
		}

		public static decimal? ToDecimal(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;

			decimal result;
			if (decimal.TryParse(value, out result))
				return result;
			return null;
		}

		public static decimal ToDecimal0(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return 0;

			decimal result;
			if (decimal.TryParse(value, out result))
				return result;
			return 0;
		}

		public static DateTime FromTimeStampToDate(this string date)
		{
			return DateTimeOffset.Parse(date.Trim()).LocalDateTime;
		}

		public static string ToCleanStringQB(this string value)
		{
			if (value == null)
				return "";

			var result = Regex.Replace(value, @"[^ A-Za-z0-9_,.?@&!#'~*\-+;$]", " ",
				
				RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline).Trim();

			return result;
		}

		public static string ToCleanStringSub0(this string value, int no)
		{
			if (value == null)
				return "";

			var result = value.ToCleanStringQB().Trim();
			return result.Length <= no || no < 0 ? result : result.Substring(0, no);
		}
	}
}
