using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop
{
	public static class CacheExtensions
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
	}
}
