using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace QBOTest.QuickBooks.Dtos
{
    [AutoMapFrom(typeof(QuickBooksToken))]
    public class QuickBooksTokenDto : EntityDto<int>
    {
        public string RealmId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}