using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QBOTest.Configuration;
using QBOTest.Controllers;
using QBOTest.QuickBooks;
using QBOTest.QuickBooks.Dtos;
using QBOTest.Web.Models.QuickBooks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace QBOTest.Web.Areas.App.Controllers
{
    public class QuickBooksController : QBOTestControllerBase
    {
        private readonly IQuickBooksTokenAppService _quickBooksTokenAppService;
        private readonly IQuickBooksSyncLogAppService _quickBooksSyncLogAppService;
        private readonly QuickBooksConfig _quickBooksConfig;
        public OAuth2Client oAuth2Client;

        public QuickBooksController(IQuickBooksTokenAppService quickBooksTokenAppService,
            IQuickBooksSyncLogAppService quickBooksSyncLogAppService,
            IOptions<QuickBooksConfig> quickBooksConfig)
        {
            _quickBooksTokenAppService = quickBooksTokenAppService;
            _quickBooksSyncLogAppService = quickBooksSyncLogAppService;
            _quickBooksConfig = quickBooksConfig.Value;
        }

        #region QBOConnection

        public async Task<ActionResult> Home(GetAllQuickBooksSyncLogsInput input)
        {
            var logs = await _quickBooksSyncLogAppService.GetAll(input);

            var realmId = await SettingManager.GetSettingValueAsync(AppSettingNames.QuickBooksRealmId);

            return View(new QuickBooksViewModel
            {
                Logs = logs,
                RealmId = realmId,
                IsQuickBooksConnected = !string.IsNullOrEmpty(realmId)
            });
        }

        public async Task<ActionResult> Index()
        {
            var state = Request.Query["state"];

            var code = Request.Query["code"].ToString();

            var realmId = Request.Query["realmId"].ToString();

            if (state.Count > 0 && !string.IsNullOrEmpty(code))
            {
                await GetAuthTokensAsync(code, realmId);

                return RedirectToAction("Home");
            }

            return RedirectToAction("Connect");
        }

        [HttpGet]
        public async Task<IActionResult> ConnectAsync()
        {
            var realmId = await SettingManager.GetSettingValueAsync(AppSettingNames.QuickBooksRealmId);

            if (!string.IsNullOrEmpty(realmId))
                return RedirectToAction("Index", "Settings", new { tab = "QuickBooks" });

            oAuth2Client = new OAuth2Client(_quickBooksConfig.ClientId, _quickBooksConfig.ClientSecret, _quickBooksConfig.RedirectUrl, _quickBooksConfig.Environment);

            var scopes = new List<OidcScopes>();

            scopes.Add(OidcScopes.Accounting);

            var authorizeUrl = oAuth2Client.GetAuthorizationURL(scopes);

            QuickBooksConfig.AuthURL = authorizeUrl;

            return Redirect(authorizeUrl);
        }

        public async Task<IActionResult> DisconnectAsync()
        {
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, AppSettingNames.QuickBooksRealmId, null);

            return RedirectToAction("Index", "Settings", new { tab = "QuickBooks" });
        }

        private async Task GetAuthTokensAsync(string code, string realmId)
        {
            oAuth2Client = new OAuth2Client(_quickBooksConfig.ClientId, _quickBooksConfig.ClientSecret, _quickBooksConfig.RedirectUrl, _quickBooksConfig.Environment);

            var tokenResponse = await oAuth2Client.GetBearerTokenAsync(code);

            _quickBooksConfig.RealmId = realmId;

            await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, AppSettingNames.QuickBooksRealmId, realmId);

            await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, AppSettingNames.IsQBOConnected, "true");

            var token = await _quickBooksTokenAppService.GetTokenForView(new GetAllQuickBooksTokensInput { RealmId = realmId });

            if (token == null)
            {
                await _quickBooksTokenAppService.CreateOrEdit(new CreateOrEditQuickBooksTokenDto
                {
                    RealmId = realmId,
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken
                });
            }
        }

        #endregion
    }
}