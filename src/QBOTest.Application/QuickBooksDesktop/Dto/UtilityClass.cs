using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop.Dto
{
	public class UtilityClass
	{
		public string RequestId { get; set; }
		public Guid EntityID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string QbDesktopId { get; set; }
		public string EditSequence { get; set; }
		public string LineItemsTnxID { get; set; }
		public string Iterator { get; set; }
		public string IteratorID { get; set; }
		public string IteratorRemainingCount { get; set; }
		public bool IsFirstTry { get; set; }
		public List<long> ServiceAddressListId { get; set; }
	}
}
