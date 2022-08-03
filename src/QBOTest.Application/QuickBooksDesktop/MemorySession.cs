
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop
{
	
	public sealed class MemorySession : ISessionPool
	{
		private readonly ICacheManager cache = new MemoryCacheManager();
		//private readonly Hashtable sessionStore = new Hashtable();

		public void Put(string key, QuickBookSession sess)
		{
			cache.Set(key, sess, 300);
		}

		public QuickBookSession GetCache(string key)
		{
			var cacheData = cache.Get(key, 300, () => GetEmpty(key));
			return cacheData;
		}

		public void Invalidate(string key)
		{
			cache.Remove(key);
		}

		private QuickBookSession GetEmpty(string key)
		{
			return (QuickBookSession)new Hashtable()[key];
		}
	}
}
