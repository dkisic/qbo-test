using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBOTest.Items
{
    [Table("Items")]
    [Audited]
    public class Item : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual string FullName { get; set; }

        public virtual string Notes { get; set; }

        public virtual decimal? Price { get; set; }

        public virtual string QuickBooksRealmId { get; set; }

        public virtual string QuickBooksId { get; set; }

        public virtual DateTime? QuickBooksLastUpdatedTime { get; set; }

        #region FKs

        public int TenantId { get; set; }

        #endregion

        public void SetItemFullName()
        {
            FullName = "(" + Code + ") " + Name;
        }
    }
}