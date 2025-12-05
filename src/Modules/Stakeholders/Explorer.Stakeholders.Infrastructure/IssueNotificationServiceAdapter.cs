using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Infrastructure
{
    public class IssueNotificationServiceAdapter : IIssueNotificationService
    {
        private readonly INotificationService _notificationService;

        public IssueNotificationServiceAdapter(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void NotifyAboutNewMessage(long touristId, long tourAuthorId, long reportProblemId, string messageContent, int messageSenderId)
        {
            // Notification for tourist (if message was not sent by tourist)
            if (messageSenderId != touristId)
            {
                var touristNotification = new NotificationDto
                {
                    UserId = touristId,
                    Type = (int)NotificationType.IssueMessage,
                    Title = "New message on reported problem",
                    Content = $"You have received a new message on a reported problem: {messageContent.Substring(0, Math.Min(50, messageContent.Length))}...",
                    RelatedEntityId = reportProblemId,
                    RelatedEntityType = "ReportProblem"
                };
                _notificationService.Create(touristNotification);
            }

            // Notification for tour author (if message was not sent by author)
            if (messageSenderId != tourAuthorId)
            {
                var authorNotification = new NotificationDto
                {
                    UserId = tourAuthorId,
                    Type = (int)NotificationType.IssueMessage,
                    Title = "New message on reported problem",
                    Content = $"You have received a new message on a reported problem: {messageContent.Substring(0, Math.Min(50, messageContent.Length))}...",
                    RelatedEntityId = reportProblemId,
                    RelatedEntityType = "ReportProblem"
                };
                _notificationService.Create(authorNotification);
            }
        }
    }
}
