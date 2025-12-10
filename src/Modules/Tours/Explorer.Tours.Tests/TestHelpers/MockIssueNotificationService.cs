using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Tests.TestHelpers
{
    public class MockIssueNotificationService : IIssueNotificationService
    {
        public void NotifyAboutNewMessage(long touristId, long tourAuthorId, long reportProblemId, string messageContent, int messageSenderId)
        {
            // Mock implementation - ne radi ništa u testovima
        }

        public void NotifyAuthorAboutNewProblem(long tourAuthorId, long touristId, long reportProblemId, string problemDescription)
        {
            // Mock implementation - ne radi ništa u testovima
        }

        public void NotifyTouristAboutAuthorResponse(long touristId, long tourAuthorId, long reportProblemId, string response)
        {
            // Mock implementation - ne radi ništa u testovima
        }
    }
}
