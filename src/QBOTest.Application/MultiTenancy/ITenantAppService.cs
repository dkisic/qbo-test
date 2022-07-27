using Abp.Application.Services;
using QBOTest.MultiTenancy.Dto;

namespace QBOTest.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

