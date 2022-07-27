using Abp;
using Abp.Authorization;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using System.Threading.Tasks;

namespace QBOTest.QuickBooks
{
    [AbpAuthorize]
    public class QuickBooksSyncAppService : QBOTestAppServiceBase, IQuickBooksSyncAppService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public QuickBooksSyncAppService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public async Task Sync()
        {
            await _backgroundJobManager.EnqueueAsync<QuickBooksSyncJob, QuickBooksSyncJobArgs>(new QuickBooksSyncJobArgs
            {
                User = AbpSession.ToUserIdentifier()
            });
        }
    }
}