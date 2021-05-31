// <copyright file="GraphUtilityHelper.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Graph;

    public class GraphUtilityHelper
    {
        private readonly GraphServiceClient graphClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphUtilityHelper"/> class.
        /// </summary>
        /// <param name="accessToken">Token to access MS graph.</param>
        public GraphUtilityHelper(string accessToken)
        {
            this.graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        await Task.Run(() =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                                "Bearer",
                                accessToken);
                        });
                    }));
        }

        public async Task<List<string>> GetUserUpns(List<Guid> ids)
        {
            var users = await this.graphClient.Users.Request().Filter(string.Join(" or ", ids.Select(x => $"id eq '{x}'"))).Select(x => x.UserPrincipalName).GetAsync();
            return users.Select(x => x.UserPrincipalName).ToList();
        }
    }
}
