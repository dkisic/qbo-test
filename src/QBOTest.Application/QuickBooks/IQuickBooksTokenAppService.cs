using Abp.Application.Services;
using Abp.Application.Services.Dto;
using QBOTest.QuickBooks.Dtos;
using System.Threading.Tasks;

namespace QBOTest.QuickBooks
{
    public interface IQuickBooksTokenAppService : IApplicationService
    {
        Task<PagedResultDto<QuickBooksTokenDto>> GetAll(GetAllQuickBooksTokensInput input);

        Task<QuickBooksTokenDto> GetTokenForView(GetAllQuickBooksTokensInput input);

        Task CreateOrEdit(CreateOrEditQuickBooksTokenDto input);
    }
}