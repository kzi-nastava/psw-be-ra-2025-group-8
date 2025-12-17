using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Tests.TestHelpers
{
    public class MockIssueNotificationService : IIssueNotificationService
    {
        public void NotifyAboutNewMessage(long touristUserId, long tourAuthorUserId, long reportProblemId, string messageContent, long messageSenderUserId)
        {
            // Mock implementation - ne radi ništa u testovima
        }

        public void NotifyAuthorAboutNewProblem(long tourAuthorUserId, long touristUserId, long reportProblemId, string problemDescription)
        {
            // Mock implementation - ne radi ništa u testovima
        }

        public void NotifyTouristAboutAuthorResponse(long touristUserId, long tourAuthorUserId, long reportProblemId, string response)
        {
            // Mock implementation - ne radi ništa u testovima
        }
    }
}
