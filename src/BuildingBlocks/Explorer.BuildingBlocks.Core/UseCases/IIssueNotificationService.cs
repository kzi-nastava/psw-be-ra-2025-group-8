namespace Explorer.BuildingBlocks.Core.UseCases
{
    public interface IIssueNotificationService
    {
        void NotifyAboutNewMessage(long touristId, long tourAuthorId, long reportProblemId, string messageContent, int messageSenderId);
    }
}
