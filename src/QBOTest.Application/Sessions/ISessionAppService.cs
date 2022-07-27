using System.Threading.Tasks;
using Abp.Application.Services;
using QBOTest.Sessions.Dto;

namespace QBOTest.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
