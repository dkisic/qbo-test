using System.Collections.Generic;
using Abp.Configuration;

namespace QBOTest.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
                new SettingDefinition(AppSettingNames.QuickBooksRealmId, null, scopes: SettingScopes.Tenant, isEncrypted: true),
                new SettingDefinition(AppSettingNames.IsQBOConnected, "false", scopes: SettingScopes.Tenant)
            };
        }
    }
}
