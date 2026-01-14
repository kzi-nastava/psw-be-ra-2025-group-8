using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Infrastructure
{
    public class IssueNotificationServiceAdapter : IIssueNotificationService
    {
     private readonly INotificationService _notificationService;
        private readonly ICrudRepository<Person> _personRepository;

        public IssueNotificationServiceAdapter(INotificationService notificationService, ICrudRepository<Person> personRepository)
        {
    _notificationService = notificationService;
            _personRepository = personRepository;
        }

        public void NotifyAboutNewMessage(long touristUserId, long tourAuthorUserId, long reportProblemId, string messageContent, long messageSenderUserId)
        {
            // Notification for tourist (if message was not sent by tourist)
    if (messageSenderUserId != touristUserId)
            {
       var touristNotification = new NotificationDto
      {
      UserId = touristUserId,
          Type = (int)NotificationType.IssueMessage,
               Title = "New message on reported problem",
          Content = $"You have received a new message on a reported problem: {messageContent.Substring(0, Math.Min(50, messageContent.Length))}...",
         RelatedEntityId = reportProblemId,
        RelatedEntityType = "ReportProblem"
        };
          _notificationService.Create(touristNotification);
   }

            // Notification for tour author (if message was not sent by author)
            if (messageSenderUserId != tourAuthorUserId)
            {
       var authorNotification = new NotificationDto
                {
          UserId = tourAuthorUserId,
 Type = (int)NotificationType.IssueMessage,
 Title = "New message on reported problem",
   Content = $"You have received a new message on a reported problem: {messageContent.Substring(0, Math.Min(50, messageContent.Length))}...",
 RelatedEntityId = reportProblemId,
         RelatedEntityType = "ReportProblem"
       };
            _notificationService.Create(authorNotification);
}
     }

        public void NotifyAuthorAboutNewProblem(long tourAuthorUserId, long touristUserId, long reportProblemId, string problemDescription)
        {
          var authorNotification = new NotificationDto
            {
            UserId = tourAuthorUserId,
       Type = (int)NotificationType.IssueMessage,
  Title = "New problem reported on your tour",
      Content = $"A tourist has reported a problem on your tour: {problemDescription.Substring(0, Math.Min(50, problemDescription.Length))}...",
          RelatedEntityId = reportProblemId,
                RelatedEntityType = "ReportProblem"
         };
        _notificationService.Create(authorNotification);
        }

        public void NotifyTouristAboutAuthorResponse(long touristUserId, long tourAuthorUserId, long reportProblemId, string response)
        {
 var touristNotification = new NotificationDto
    {
  UserId = touristUserId,
       Type = (int)NotificationType.IssueMessage,
   Title = "Author responded to your problem report",
         Content = $"The tour author has responded to your problem report: {response.Substring(0, Math.Min(50, response.Length))}...",
            RelatedEntityId = reportProblemId,
            RelatedEntityType = "ReportProblem"
            };
            _notificationService.Create(touristNotification);
        }
    }
}
