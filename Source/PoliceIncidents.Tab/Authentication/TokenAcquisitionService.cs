namespace PoliceIncidents.Tab.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using PoliceIncidents.Models;

    public class TokenAcquisitionService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOptions<AzureAdOptions> azureAdOptions;
        private readonly IConfidentialClientApplication confidentialClientApp;
        private readonly ILogger<TokenAcquisitionService> logger;

        public TokenAcquisitionService(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AzureAdOptions> azureAdOptions,
            IConfidentialClientApplication confidentialClientApp,
            ILogger<TokenAcquisitionService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.azureAdOptions = azureAdOptions;
            this.confidentialClientApp = confidentialClientApp;
            this.logger = logger;
        }

        public string UserObjectId
        {
            get
            {
                var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
                var claim = this.httpContextAccessor.HttpContext.User.Claims.First(p => oidClaimType.Equals(p.Type, StringComparison.Ordinal));
                return claim.Value;
            }
        }

        /// <summary>
        /// Get user Azure AD access token.
        /// </summary>
        /// <returns>Token to access MS graph.</returns>
        public async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            List<string> scopeList = this.azureAdOptions.Value.GraphScope.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

            try
            {
                // Gets user account from the accounts available in token cache.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.clientapplicationbase.getaccountasync?view=azure-dotnet
                // Concatenation of UserObjectId and TenantId separated by a dot is used as unique identifier for getting user account.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.accountid.identifier?view=azure-dotnet#Microsoft_Identity_Client_AccountId_Identifier
                var account = await this.confidentialClientApp.GetAccountAsync($"{this.UserObjectId}.{this.azureAdOptions.Value.TenantId}");

                // Attempts to acquire an access token for the account from the user token cache.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.clientapplicationbase.acquiretokensilent?view=azure-dotnet
                AuthenticationResult result = await this.confidentialClientApp
                    .AcquireTokenSilent(scopeList, account)
                    .ExecuteAsync();
                return result;
            }
            catch (MsalUiRequiredException msalex)
            {
                // Getting new token using AddTokenToCacheFromJwtAsync as AcquireTokenSilent failed to load token from cache.
                try
                {
                    this.logger.LogInformation($"MSAL exception occurred while trying to acquire new token. MSAL exception details are found {msalex}.");
                    var jwtToken = AuthenticationHeaderValue.Parse(this.httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()).Parameter;
                    return await this.AddTokenToCacheFromJwtAsync(this.azureAdOptions.Value.GraphScope, jwtToken);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, $"An error occurred in GetAccessTokenAsync: {ex.Message}.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in fetching token : {ex.Message}.");
                throw;
            }
        }

        private readonly string[] scopesRequestedByMsalNet = new string[]
        {
            "openid",
            "profile",
            "offline_access",
        };

        /// <summary>
        /// Adds token to cache.
        /// </summary>
        /// <param name="graphScopes">Graph scopes to be added to token.</param>
        /// <param name="jwtToken">JWT bearer token.</param>
        /// <returns>Token with graph scopes.</returns>
        public async Task<AuthenticationResult> AddTokenToCacheFromJwtAsync(string graphScopes, string jwtToken)
        {
            if (jwtToken == null)
            {
                throw new ArgumentNullException(jwtToken, "tokenValidationContext.SecurityToken should be a JWT Token");
            }

            UserAssertion userAssertion = new UserAssertion(jwtToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");
            IEnumerable<string> requestedScopes = graphScopes.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

            // Result to make sure that the cache is filled-in before the controller tries to get access tokens
            var result = await this.confidentialClientApp.AcquireTokenOnBehalfOf(requestedScopes.Except(this.scopesRequestedByMsalNet), userAssertion)
                .ExecuteAsync();
            return result;
        }
    }
}
