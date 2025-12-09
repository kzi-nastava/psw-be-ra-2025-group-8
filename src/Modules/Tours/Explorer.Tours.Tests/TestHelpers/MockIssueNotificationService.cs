using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Tests.TestHelpers
{
    public class MockIssueNotificationService : IIssueNotificationService
    {
        public void NotifyAboutNewMessage(long touristId, long tourAuthorId, long reportProblemId, string messageContent, int messageSenderId)
        {
            // Mock implementation - ne radi ništa u testovima
        }
    }
}
