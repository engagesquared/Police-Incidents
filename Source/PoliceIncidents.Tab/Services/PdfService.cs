namespace PoliceIncidents.Tab.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Tab.Helpers;

    public class PdfService
    {
        private readonly ILogger<PdfService> logger;
        private readonly PoliceIncidentsDbContext dbContext;
        private readonly GraphApiService graphApiService;

        public PdfService(PoliceIncidentsDbContext dbContext, ILogger<PdfService> logger, GraphApiService graphApiService)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.graphApiService = graphApiService;
        }

        public async Task<string> GenerateAndUploadPdf(long incidentId, bool onClosure = false)
        {
            try
            {
                var incident = this.dbContext.IncidentDetails.Where(x => x.Id == incidentId).Include(x => x.District).FirstOrDefault();
                if (incident == null)
                {
                    this.logger.LogError($"No incident was found with '{incidentId}' Id.");
                    return null;
                }

                if (incident.District == null || !incident.District.TeamGroupId.HasValue)
                {
                    this.logger.LogError($"Incident '{incidentId}' has no district associated with or district is invalid");
                    return null;
                }

                var district = incident.District;

                if (string.IsNullOrEmpty(district.RootFolderPath))
                {
                    var rfolder = await this.graphApiService.GetRootDriveUrl(district.TeamGroupId.ToString());
                    district.RootFolderPath = rfolder;
                    this.dbContext.Update(district);
                    await this.dbContext.SaveChangesAsync();
                }

                var chatMessage = await this.graphApiService.GetChatMessage(district.TeamGroupId.ToString(), district.ConversationId, incident.ChatConverstaionId);
                var chatMessageReplies = await this.graphApiService.GetChatMessageReplies(district.TeamGroupId.ToString(), district.ConversationId, incident.ChatConverstaionId);

                var pdfBytes = PdfGenerator.PrepareDocument(chatMessage, chatMessageReplies);
                MemoryStream stream = new MemoryStream(pdfBytes);
                var folder = incident.FileReportFolderName ?? $"/IncidentReports/{incident.Id}-{new Regex("[\"*:<>?\\/\\|]").Replace(incident.Title, string.Empty)}";
                if (folder.Length > 80)
                {
                    folder = folder.Substring(0, 80);
                }

                if (!folder.Equals(incident.FileReportFolderName))
                {
                    incident.FileReportFolderName = folder;
                    this.dbContext.Update(incident);
                    await this.dbContext.SaveChangesAsync();
                }

                var name = onClosure ? "ClosureReport" : "Report";

                var pdfDocUrl = await this.graphApiService.UploadFileToTeams(district.TeamGroupId.ToString(), $"{folder}/{name}-{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}.pdf", stream);
                return pdfDocUrl;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"GenerateAndUploadPdf for incident '{incidentId}' error.");
                throw;
            }
        }
    }
}
