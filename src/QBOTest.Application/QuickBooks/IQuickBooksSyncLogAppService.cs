using Abp.Application.Services;
using QBOTest.QuickBooks.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QBOTest.QuickBooks
{
    public interface IQuickBooksSyncLogAppService : IApplicationService
    {
        Task<List<QuickBooksSyncLogDto>> GetAll(GetAllQuickBooksSyncLogsInput input);
    }
}