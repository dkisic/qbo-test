using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace QBOTest.QuickBooks
{
    public class QuickBooksToken : Entity<int>, IMustHaveTenant
    {
        [Required]
        [StringLength(QuickBooksConsts.MaxRealmIdLength, MinimumLength = QuickBooksConsts.MinRealmIdLength)]
        public string RealmId { get; set; }

        [Required]
        [StringLength(QuickBooksConsts.MaxTokenLength, MinimumLength = QuickBooksConsts.MinTokenLength)]
        public string AccessToken { get; set; }

        [Required]
        [StringLength(QuickBooksConsts.MaxTokenLength, MinimumLength = QuickBooksConsts.MinTokenLength)]
        public string RefreshToken { get; set; }

        public int TenantId { get; set; }
    }
}