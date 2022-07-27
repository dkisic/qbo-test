using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Intuit.Ipp.Core;
using Intuit.Ipp.Core.Configuration;
using Intuit.Ipp.Exception;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QBOTest.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QBOTest.QuickBooks.Api
{
    public class QuickBooksApiManager : DomainService, IQuickBooksApiManager
    {
        private readonly IRepository<QuickBooksToken> _repository;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly QuickBooksConfig _quickBooksConfig;

        public QuickBooksApiManager(IRepository<QuickBooksToken> repository,
            IHostEnvironment hostEnvironment,
            IOptions<QuickBooksConfig> quickBooksConfig)
        {
            _repository = repository;
            _hostEnvironment = hostEnvironment;
            _quickBooksConfig = quickBooksConfig.Value;
        }

        [UnitOfWork]
        public async Task QBOApiCall(Func<ServiceContext, UserIdentifier, Task> apiCallFunction, UserIdentifier user)
        {
            using (UnitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var oauthClient = new OAuth2Client(
                _quickBooksConfig.ClientId,
                _quickBooksConfig.ClientSecret,
                _quickBooksConfig.RedirectUrl,
                _quickBooksConfig.Environment);

                var realmId = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.QuickBooksRealmId, user.TenantId.Value);

                var token = await _repository.FirstOrDefaultAsync(t => t.RealmId == realmId);

                try
                {
                    if (!string.IsNullOrEmpty(realmId))
                        if (token.AccessToken != null && token.RealmId != null)
                        {
                            var reqValidator = new OAuth2RequestValidator(token.AccessToken);

                            var appSettingsFile = _hostEnvironment.IsProduction() ? "\\appsettings.production.json" : "\\appsettings.json";

                            var configurationProvider = new JsonFileConfigurationProvider(Directory.GetCurrentDirectory() + appSettingsFile);

                            var context = new ServiceContext(token.RealmId, IntuitServicesType.QBO, reqValidator, configurationProvider);

                            context.IppConfiguration.BaseUrl.Qbo = _quickBooksConfig.QBOBaseUrl;

                            context.IppConfiguration.MinorVersion.Qbo = "56";

                            await apiCallFunction(context, user);
                        }
                }
                catch (IdsException ex)
                {
                    if (ex.Message == "Unauthorized-401")
                    {
                        var tokens = await oauthClient.RefreshTokenAsync(token.RefreshToken);

                        if (tokens.AccessToken != null && tokens.RefreshToken != null)
                        {
                            await UpdateTokens(token.RealmId, tokens.AccessToken, tokens.RefreshToken);

                            await QBOApiCall(apiCallFunction, user);
                        }
                    }
                }
            }
        }

        [UnitOfWork]
        public async Task<ServiceContext> GetServiceContext(int tenantId)
        {
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var oauthClient = new OAuth2Client(
                _quickBooksConfig.ClientId,
                _quickBooksConfig.ClientSecret,
                _quickBooksConfig.RedirectUrl,
                _quickBooksConfig.Environment);

                var realmId = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.QuickBooksRealmId, tenantId);

                var token = await _repository.FirstOrDefaultAsync(t => t.RealmId == realmId);

                try
                {
                    if (!string.IsNullOrEmpty(realmId))
                        if (token.AccessToken != null && token.RealmId != null)
                        {
                            var reqValidator = new OAuth2RequestValidator(token.AccessToken);

                            var appSettingsFile = _hostEnvironment.IsProduction() ? "\\appsettings.production.json" : "\\appsettings.json";

                            var configurationProvider = new JsonFileConfigurationProvider(Directory.GetCurrentDirectory() + appSettingsFile);

                            var context = new ServiceContext(token.RealmId, IntuitServicesType.QBO, reqValidator, configurationProvider);

                            context.IppConfiguration.BaseUrl.Qbo = _quickBooksConfig.QBOBaseUrl;

                            return context;
                        }

                    return null;
                }
                catch (IdsException ex)
                {
                    if (ex.Message == "Unauthorized-401")
                    {
                        var tokens = await oauthClient.RefreshTokenAsync(token.RefreshToken);

                        if (tokens.AccessToken != null && tokens.RefreshToken != null)
                        {
                            await UpdateTokens(token.RealmId, tokens.AccessToken, tokens.RefreshToken);

                            await GetServiceContext(tenantId);
                        }
                    }

                    return null;
                }
            }
        }

        public async Task<QuickBooksToken> UpdateTokens(string realmId, string newAccessToken, string newRefreshToken)
        {
            var token = await _repository.FirstOrDefaultAsync(t => t.RealmId == realmId);

            if (token != null)
            {
                token.AccessToken = newAccessToken;
                token.RefreshToken = newRefreshToken;
            }

            await _repository.UpdateAsync(token);

            return token;
        }
    }
}