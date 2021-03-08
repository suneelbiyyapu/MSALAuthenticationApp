using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Essentials;

namespace MSALApp.Services
{
    public class AuthService
    {
        readonly string AppId = "com.companyname.msalapp";
        readonly string TenantId = "4e492e5e-330b-4c7d-83c6-583dc3dfcb04";
        readonly string ClientID = "11ae2af9-bd98-409e-9f15-5a4542df37e1";
        readonly string[] Scopes = { "User.Read" };
        readonly IPublicClientApplication _pca;

        // Android uses this to determine which activity to use to show
        // the login screen dialog from.
        public static object ParentWindow { get; set; }

        string RedirectUri
        {
            get
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    return $"msauth://{AppId}/3ZLW%2FTAqPvR43Zh79ejFQDOdka8%3D";
                }
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                { 
                    return $"msauth.{AppId}://auth";
                }
                return string.Empty;
            }
        }

        public AuthService()
        {
            _pca = PublicClientApplicationBuilder.Create(ClientID)
                .WithIosKeychainSecurityGroup(AppId)
                .WithRedirectUri(RedirectUri)
                .WithAuthority($"https://login.microsoftonline.com/{TenantId}")
                .Build();
        }

        public async Task<bool> SignInAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await _pca.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await _pca.AcquireTokenInteractive(Scopes)
                                                .WithParentActivityOrWindow(ParentWindow)
                                                .WithUseEmbeddedWebView(true)
                                                .ExecuteAsync();

                    // Store the access token securely for later use.
                    await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                    return true;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await _pca.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await _pca.GetAccountsAsync();
                }

                // Clear our access token from secure storage.
                SecureStorage.Remove("AccessToken");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
