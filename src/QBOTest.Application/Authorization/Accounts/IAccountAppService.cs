using System.Threading.Tasks;
using Abp.Application.Services;
using QBOTest.Authorization.Accounts.Dto;

namespace QBOTest.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
