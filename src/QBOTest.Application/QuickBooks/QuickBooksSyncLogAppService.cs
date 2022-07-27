using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using QBOTest.QuickBooks.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QBOTest.QuickBooks
{
    [AbpAuthorize]
    public class QuickBooksSyncLogAppService : QBOTestAppServiceBase, IQuickBooksSyncLogAppService
    {
        private readonly IRepository<QuickBooksSyncLog, Guid> _repository;

        public QuickBooksSyncLogAppService(
            IRepository<QuickBooksSyncLog, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<List<QuickBooksSyncLogDto>> GetAll(GetAllQuickBooksSyncLogsInput input)
        {
            var logs = await _repository
                .GetAll()
                .AsNoTracking()
                .ToListAsync();

            return ObjectMapper.Map<List<QuickBooksSyncLogDto>>(logs);
        }
    }
}