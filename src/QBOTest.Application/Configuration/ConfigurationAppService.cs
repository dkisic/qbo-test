using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using QBOTest.Configuration.Dto;

namespace QBOTest.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : QBOTestAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
