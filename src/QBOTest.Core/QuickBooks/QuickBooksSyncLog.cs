using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace QBOTest.QuickBooks
{
    public class QuickBooksSyncLog : CreationAuditedEntity<Guid>, IMustHaveTenant
    {
        [Required]
        [StringLength(QuickBooksConsts.MaxEntityNameLength, MinimumLength = QuickBooksConsts.MinEntityNameLength)]
        public string EntityName { get; set; }

        [StringLength(QuickBooksConsts.MaxDescriptionLength, MinimumLength = QuickBooksConsts.MinDescriptionLength)]
        public string Description { get; set; }

        public QuickBooksSyncStatus SyncStatus { get; set; }

        public QuickBooksSyncDirection SyncDirection { get; set; }

        public QuickBooksSyncEntity SyncEntity { get; set; }

        public QuickBooksSyncAction SyncAction { get; set; }

        public int TenantId { get; set; }
    }
}