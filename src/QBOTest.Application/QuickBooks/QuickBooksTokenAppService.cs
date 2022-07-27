using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using QBOTest.QuickBooks.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QBOTest.QuickBooks
{
    [AbpAuthorize]
    public class QuickBooksTokenAppService : QBOTestAppServiceBase, IQuickBooksTokenAppService
    {
        private readonly IRepository<QuickBooksToken> _quickBooksTokenRepository;

        public QuickBooksTokenAppService(IRepository<QuickBooksToken> quickBooksTokenRepository)
        {
            _quickBooksTokenRepository = quickBooksTokenRepository;
        }

        public async Task<PagedResultDto<QuickBooksTokenDto>> GetAll(GetAllQuickBooksTokensInput input)
        {
            var query = _quickBooksTokenRepository.GetAll();

            var count = await query.CountAsync();

            var tokens = ObjectMapper.Map<List<QuickBooksTokenDto>>(await query.ToListAsync());

            return new PagedResultDto<QuickBooksTokenDto>(count, tokens);
        }

        public async Task<QuickBooksTokenDto> GetTokenForView(GetAllQuickBooksTokensInput input)
        {
            var token = await _quickBooksTokenRepository
                .GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.RealmId), x => x.RealmId == input.RealmId)
                .FirstOrDefaultAsync();

            return ObjectMapper.Map<QuickBooksTokenDto>(token);
        }

        public async Task CreateOrEdit(CreateOrEditQuickBooksTokenDto input)
        {
            if (input.Id == null)
                await Create(input);
            else
                await Update(input);
        }

        protected virtual async Task Create(CreateOrEditQuickBooksTokenDto input)
        {
            var quickBooks = ObjectMapper.Map<QuickBooksToken>(input);

            if (AbpSession.TenantId != null)
                quickBooks.TenantId = (int)AbpSession.TenantId;

            await _quickBooksTokenRepository.InsertAsync(quickBooks);
        }

        protected virtual async Task Update(CreateOrEditQuickBooksTokenDto input)
        {
            var quickBooks = await _quickBooksTokenRepository.FirstOrDefaultAsync((int)input.Id);

            ObjectMapper.Map(input, quickBooks);

            await _quickBooksTokenRepository.UpdateAsync(quickBooks);
        }
    }
}