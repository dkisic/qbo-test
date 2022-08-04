using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

	}
}
