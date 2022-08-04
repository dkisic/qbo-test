using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBOTest.Partners
{
    [Table("Partners")]
    [Audited]
    public class Partner : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual string FullName { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual bool IsCustomer { get; set; }

        public virtual bool IsSupplier { get; set; }

        public virtual string Notes { get; set; }

        public virtual string QuickBooksRealmId { get; set; }

        public virtual string QuickBooksId { get; set; }

        public virtual DateTime? QuickBooksLastUpdatedTime { get; set; }

        public virtual string QuickBooksDesktopId { get; set; }

        #region FKs

        public int TenantId { get; set; }

        #endregion

        public void SetPartnerFullName()
        {
            FullName = "(" + Code + ") " + Name;
        }
    }
}