using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Localization;
using System;

namespace QBOTest.QuickBooks.Dtos
{
    [AutoMapFrom(typeof(QuickBooksSyncLog))]
    public class QuickBooksSyncLogDto : CreationAuditedEntityDto<Guid>
    {
        public string EntityName { get; set; }

        public string Description { get; set; }

        public QuickBooksSyncStatus SyncStatus { get; set; }

        public string SyncStatusColor => QuickBooksSyncHelper.GetStatusColor(SyncStatus);

        public string SyncStatusLocalized => LocalizationHelper.GetString(QBOTestConsts.LocalizationSourceName, SyncStatus.ToString());

        public QuickBooksSyncDirection SyncDirection { get; set; }

        public string SyncDirectionTemplate => QuickBooksSyncHelper.GetDirectionTemplate(SyncDirection);

        public string SyncDirectionLocalized => LocalizationHelper.GetString(QBOTestConsts.LocalizationSourceName, SyncDirection.ToString());

        public QuickBooksSyncEntity SyncEntity { get; set; }

        public string SyncEntityLocalized => LocalizationHelper.GetString(QBOTestConsts.LocalizationSourceName, SyncEntity.ToString());

        public QuickBooksSyncAction SyncAction { get; set; }

        public string SyncActionLocalized => LocalizationHelper.GetString(QBOTestConsts.LocalizationSourceName, SyncAction.ToString());
    }
}