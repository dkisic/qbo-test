using QBOTest.QuickBooks;
using QBOTest.QuickBooks.Dtos;
using System.Collections.Generic;

namespace QBOTest.Web.Models.QuickBooks
{
    public class QuickBooksViewModel
    {
        public string RealmId { get; set; }

        public bool IsQuickBooksConnected { get; set; }

        public List<QuickBooksSyncLogDto> Logs { get; set; }
    }
}
