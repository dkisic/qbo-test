using System.Threading.Tasks;
using QBOTest.Configuration.Dto;

namespace QBOTest.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
