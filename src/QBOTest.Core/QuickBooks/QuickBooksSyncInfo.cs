namespace QBOTest.QuickBooks
{
    public class QuickBooksSyncInfo
    {
        public QuickBooksSyncEntity SyncEntity { get; set; }

        public string SyncEntityString => SyncEntity.ToString();

        public int SuccessfulSyncs { get; set; }

        public int UnsuccessfulSyncs { get; set; }
    }
}