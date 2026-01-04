using Explorer.Encounters.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Encounters.API.Public;

public interface IChallengeService
{
    ChallengeDto CreateChallenge(CreateChallengeDto dto, long creatorPersonId);
    List<ChallengeDto> GetPendingChallenges();
    ChallengeDto ApproveChallenge(long challengeId, long adminUserId);
    ChallengeDto GetById(long id);
}
