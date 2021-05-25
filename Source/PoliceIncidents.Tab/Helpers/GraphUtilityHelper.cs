// <copyright file="GraphUtilityHelper.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Helpers
{
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
    }
}
