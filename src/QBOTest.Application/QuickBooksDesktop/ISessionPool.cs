using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop
{
	/// <summary>
	///     ISessionPool is the interface for session classes. There can be many ways to
	///     store information between calls from QBWC. Possibly implementations could
	///     include memory sessions, database sessions, cookies, or whatever.
	/// </summary>
	public interface ISessionPool
	{
		void Put(string key, QuickBookSession sess);
		QuickBookSession GetCache(string key);
		void Invalidate(string key);
	}
}
