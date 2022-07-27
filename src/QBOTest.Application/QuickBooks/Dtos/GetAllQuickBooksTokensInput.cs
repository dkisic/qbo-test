using Abp.Application.Services.Dto;

namespace QBOTest.QuickBooks.Dtos
{
    public class GetAllQuickBooksTokensInput : PagedAndSortedResultRequestDto
    {
        public string RealmId { get; set; }
    }
}