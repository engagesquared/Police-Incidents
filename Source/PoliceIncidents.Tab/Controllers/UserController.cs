namespace PoliceIncidents.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using PoliceIncidents.Models;



    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> logger;

        public UserController(
            IOptions<AzureAdOptions> azureAdOptions,
            ILogger<UserController> logger,
            IConfidentialClientApplication confidentialClientApp)
            : base(confidentialClientApp, azureAdOptions, logger)
        {
            this.logger = logger;
        }

        [HttpGet("GetToken")]
        public async Task<IActionResult> GetToken()
        {
            try
            {
                string accessToken = await this.GetAccessTokenAsync();
                return this.Ok(accessToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetToken: {ex.Message}");
                throw;
            }
        }
    }
}