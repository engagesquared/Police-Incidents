// <copyright file="CreateIncident.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Functions.Functions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PoliceIncidents.Core.Interfaces;
    using PoliceIncidents.Core.Models;

    public class CreateIncident
    {
        private const string FuncName = nameof(CreateIncident);
        private readonly ILogger<CreateIncident> log;
        private readonly IIncidentService incidentService;
        private readonly HttpClient httpClient;

        public CreateIncident(ILogger<CreateIncident> log, IIncidentService incidentService, HttpClient httpClient)
        {
            this.log = log;
            this.incidentService = incidentService;
            this.httpClient = httpClient;
        }

        [FunctionName(FuncName)]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "CreateIncident")] HttpRequest req)
        {
            try
            {
                this.log.LogInformation($"{FuncName} started.");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                this.log.LogInformation($"Request body: '{requestBody}'");

                var model = this.ParseAndValidateModel(requestBody);
                var newIncidentId = await this.incidentService.CreateIncident(model);
                try
                {
                    var botNotifyPath = Core.Common.Constants.IncidentCreatedBotRoute.Replace("{id}", newIncidentId.ToString());
                    await this.httpClient.GetAsync(WebConfig.BotBaseUrl + botNotifyPath);
                }
                catch (Exception ex)
                {
                    this.log.LogError(ex, "Notifying bot error.");
                }

                return new OkResult();
            }
            catch (ValidationException ex)
            {
                this.log.LogWarning(ex, $"Validation error: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                this.log.LogError(ex, "Fatal error");
                var result = new JsonResult(ex.Message);
                result.StatusCode = 500;
                return result;
            }
            finally
            {
                this.log.LogInformation($"{FuncName} finished.");
            }
        }

        private IncidentInputModel ParseAndValidateModel(string requestBody)
        {
            IncidentInputModel inputModel;
            try
            {
                inputModel = JsonConvert.DeserializeObject<IncidentInputModel>(requestBody);
            }
            catch (Exception)
            {
                this.log.LogError("Parsing body JSON error");
                throw;
            }

            if (string.IsNullOrEmpty(inputModel.Id))
            {
                throw new ValidationException("Id field can't be empty");
            }

            return inputModel;
        }
    }
}
