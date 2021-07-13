// <copyright file="PdfGenerator.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Graph;
    using WkHtmlToPdfDotNet;

    public static class PdfGenerator
    {
        private static readonly BasicConverter converter = new BasicConverter(new PdfTools());
        private static readonly string chatMessageTemplate = "<div style=\"border: 1px solid #edebe9; border-radius: .3rem .3rem 0 0; padding: 15px; text-align: left;\"><div style=\" color: black; display: flex; align-items: center; \"><span style=\" margin-right: 5px; font-size: 12px; font-weight: 600;\">Police Incidents Bot</span><span style=\" font-size: 12px;\">{{CreateDate}}</span></div><div style=\" color: black; margin-top: 10px;\">{{Content}}</div></div>";
        private static readonly string chatMessageReplyTemplate = "<div style=\" border: 1px solid #edebe9; border-bottom: 0; border-top: 0; padding: 15px; text-align: left;\"><div style=\" color: black; display: flex; align-items: center; \"><span style=\" margin-right: 5px; font-size: 12px; font-weight: 600;\">{{AuthorDisplayName}}</span><span style=\" font-size: 12px;\">{{CreateDate}}</span></div><div style=\" color: black; margin-top: 10px;\">{{Content}}</div></div>";
        private static readonly string attachmentTemplate = "<div style=\"height: 18px; width: min-content; max-width: min-content; background: #f1f0ef; padding: 15px; margin: 5px 0; border: 1px solid rgba(0,0,0,.05); border-radius: 3px; box-shadow: 0 1px 2px -1px rgb(0 0 0 / 10%);\"><a href=\"{{AttachmentUrl}}\" style=\" text-decoration: none; color: black;\">{{AttachmentName}}</a></div>";


        public static byte[] PrepareDocument(ChatMessage chatMessage, IChatMessageRepliesCollectionPage chatMessageReplies)
        {
            var messageContent = chatMessageTemplate;
            messageContent = messageContent.Replace("{{CreateDate}}", chatMessage.LastModifiedDateTime.ToString());
            var messageBodyContent = ProcessMessageContent(chatMessage.Body.Content, chatMessage.Attachments);
            messageContent = messageContent.Replace("{{Content}}", messageBodyContent);

            var messageRepliesContent = string.Empty;
            var replies = chatMessageReplies.ToList();
            var sortedReplies = replies.OrderBy(x => x.CreatedDateTime);
            foreach (var reply in sortedReplies)
            {
                var replyContent = chatMessageReplyTemplate;
                replyContent = replyContent.Replace("{{CreateDate}}", reply.LastModifiedDateTime.ToString());
                replyContent = replyContent.Replace("{{AuthorDisplayName}}", reply.From.User.DisplayName);
                var replyBodyContent = ProcessMessageContent(reply.Body.Content, reply.Attachments);
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

            byte[] pdf = converter.Convert(doc);
            return pdf;
        }

        private static string ProcessMessageContent(string messageContent, IEnumerable<ChatMessageAttachment> attachments)
        {
            var content = Regex.Replace(messageContent, "<attachment (id=\".+?\")></attachment>",  match =>
            {
                var idString = match.Groups[1].Value;
                var id = idString.Substring(4, idString.Length - 5);

                var attachment = attachments.Where(x => x.Id == id).FirstOrDefault();
                if (string.IsNullOrEmpty(attachment.ContentUrl) || string.IsNullOrEmpty(attachment.Name))
                {
                    return string.Empty;
                }

                var attachmentNode = attachmentTemplate;
                attachmentNode = attachmentNode.Replace("{{AttachmentUrl}}", attachment.ContentUrl);
                attachmentNode = attachmentNode.Replace("{{AttachmentName}}", attachment.Name);
                return attachmentNode;
            });
            return content;
        }
    }
}
