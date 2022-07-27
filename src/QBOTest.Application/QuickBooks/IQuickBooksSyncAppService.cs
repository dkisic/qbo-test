using Abp.Application.Services;
using System.Threading.Tasks;

namespace QBOTest.QuickBooks
{
    public interface IQuickBooksSyncAppService : IApplicationService
    {
        Task Sync();
    }
}