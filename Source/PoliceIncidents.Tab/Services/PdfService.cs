namespace PoliceIncidents.Tab.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using PoliceIncidents.Core.DB;
    using WkHtmlToPdfDotNet;
    using WkHtmlToPdfDotNet.Contracts;

    public class PdfService
    {
        private readonly ILogger<PdfService> logger;
        private readonly PoliceIncidentsDbContext dbContext;
        private readonly GraphApiService graphApiService;
        private readonly IConverter pdfConverter;

        public PdfService(PoliceIncidentsDbContext dbContext, ILogger<PdfService> logger, GraphApiService graphApiService, IConverter pdfConverter)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.graphApiService = graphApiService;
            this.pdfConverter = pdfConverter;
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
                var pdfBytes = this.PrepareDocument(chatMessage, chatMessageReplies);
                using (MemoryStream stream = new MemoryStream(pdfBytes))
                {
                    var pdfDocUrl = await this.graphApiService.UploadFileToTeams(district.TeamGroupId.ToString(), $"{folder}/{name}-{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}.pdf", stream);
                    return pdfDocUrl;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"GenerateAndUploadPdf for incident '{incidentId}' error.");
                throw;
            }
        }

        private readonly string chatMessageTemplate = "<div style=\"border: 1px solid #edebe9; border-radius: .3rem .3rem 0 0; padding: 15px; text-align: left;\"><div style=\" color: black; display: flex; align-items: center; \"><span style=\" margin-right: 5px; font-size: 12px; font-weight: 600;\">Police Incidents Bot</span><span style=\" font-size: 12px;\">{{CreateDate}}</span></div><div style=\" color: black; margin-top: 10px;\">{{Content}}</div></div>";
        private readonly string chatMessageReplyTemplate = "<div style=\" border: 1px solid #edebe9; border-bottom: 0; border-top: 0; padding: 15px; text-align: left;\"><div style=\" color: black; display: flex; align-items: center; \"><span style=\" margin-right: 5px; font-size: 12px; font-weight: 600;\">{{AuthorDisplayName}}</span><span style=\" font-size: 12px;\">{{CreateDate}}</span></div><div style=\" color: black; margin-top: 10px;\">{{Content}}</div></div>";
        private readonly string attachmentTemplate = "<div style=\"height: 18px; width: min-content; max-width: min-content; background: #f1f0ef; padding: 15px; margin: 5px 0; border: 1px solid rgba(0,0,0,.05); border-radius: 3px; box-shadow: 0 1px 2px -1px rgb(0 0 0 / 10%);\"><a href=\"{{AttachmentUrl}}\" style=\" text-decoration: none; color: black;\">{{AttachmentName}}</a></div>";


        public byte[] PrepareDocument(ChatMessage chatMessage, IChatMessageRepliesCollectionPage chatMessageReplies)
        {
            var messageContent = this.chatMessageTemplate;
            messageContent = messageContent.Replace("{{CreateDate}}", chatMessage.LastModifiedDateTime.ToString());
            var messageBodyContent = this.ProcessMessageContent(chatMessage.Body.Content, chatMessage.Attachments);
            messageContent = messageContent.Replace("{{Content}}", messageBodyContent);

            var messageRepliesContent = string.Empty;
            var replies = chatMessageReplies.ToList();
            var sortedReplies = replies.OrderBy(x => x.CreatedDateTime);
            foreach (var reply in sortedReplies)
            {
                var replyContent = this.chatMessageReplyTemplate;
                replyContent = replyContent.Replace("{{CreateDate}}", reply.LastModifiedDateTime.ToString());
                replyContent = replyContent.Replace("{{AuthorDisplayName}}", reply.From.User.DisplayName);
                var replyBodyContent = this.ProcessMessageContent(reply.Body.Content, reply.Attachments);
                replyContent = replyContent.Replace("{{Content}}", replyBodyContent);
                messageRepliesContent += replyContent;
            }

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        PagesCount = true,
                        HtmlContent = messageContent + messageRepliesContent + "<div style=\"border: 1px solid #edebe9;\"></div>",
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                    },
                },
            };

            byte[] pdf = this.pdfConverter.Convert(doc);
            return pdf;
        }

        private string ProcessMessageContent(string messageContent, IEnumerable<ChatMessageAttachment> attachments)
        {
            var content = Regex.Replace(messageContent, "<attachment (id=\".+?\")></attachment>", match =>
            {
                var idString = match.Groups[1].Value;
                var id = idString.Substring(4, idString.Length - 5);

                var attachment = attachments.Where(x => x.Id == id).FirstOrDefault();
                if (string.IsNullOrEmpty(attachment.ContentUrl) || string.IsNullOrEmpty(attachment.Name))
                {
                    return string.Empty;
                }

                var attachmentNode = this.attachmentTemplate;
                attachmentNode = attachmentNode.Replace("{{AttachmentUrl}}", attachment.ContentUrl);
                attachmentNode = attachmentNode.Replace("{{AttachmentName}}", attachment.Name);
                return attachmentNode;
            });
            return content;
        }
    }
}
