namespace QBOTest.QuickBooks
{
    public static class QuickBooksSyncHelper
    {
        public static string GetStatusColor(QuickBooksSyncStatus status)
        {
            return status switch
            {
                QuickBooksSyncStatus.Synced => "success",
                QuickBooksSyncStatus.Failed => "danger",
                _ => string.Empty,
            };
        }

        public static string GetDirectionTemplate(QuickBooksSyncDirection direction)
        {
            return direction switch
            {
                QuickBooksSyncDirection.FromQuickBooks => "QBO <i class='flaticon2-left-arrow-1 ki ki-bold-wide-arrow-next px-1'></i> ERP Net",
                QuickBooksSyncDirection.ToQuickBooks => "ERP Net <i class='flaticon2-left-arrow-1 ki ki-bold-wide-arrow-next px-1'></i> QBO",
                _ => string.Empty,
            };
        }
    }

    public enum QuickBooksSyncStatus
    {
        Synced,
        Failed
    }

    public enum QuickBooksSyncDirection
    {
        ToQuickBooks,
        FromQuickBooks
    }

    public enum QuickBooksSyncEntity
    {
        Partner,
        Item
    }

    public enum QuickBooksSyncAction
    {
        Create,
        Update,
        Delete
    }
}