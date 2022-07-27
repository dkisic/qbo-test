using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace QBOTest.QuickBooks.Dtos
{
    [AutoMapTo(typeof(QuickBooksToken))]
    public class CreateOrEditQuickBooksTokenDto : EntityDto<int?>
    {
        public string RealmId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public int TenantId { get; set; }
    }
}