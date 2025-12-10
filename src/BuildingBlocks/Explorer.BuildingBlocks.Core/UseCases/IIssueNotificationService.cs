namespace Explorer.BuildingBlocks.Core.UseCases
{
    /// <summary>
    /// Servis za slanje notifikacija vezanih za prijavljene probleme na turama.
    /// Svi parametri su UserId (direktno iz Users tabele).
    /// </summary>
    public interface IIssueNotificationService
    {
        void NotifyAboutNewMessage(long touristUserId, long tourAuthorUserId, long reportProblemId, string messageContent, long messageSenderUserId);
        void NotifyAuthorAboutNewProblem(long tourAuthorUserId, long touristUserId, long reportProblemId, string problemDescription);
        void NotifyTouristAboutAuthorResponse(long touristUserId, long tourAuthorUserId, long reportProblemId, string response);
    }
}
