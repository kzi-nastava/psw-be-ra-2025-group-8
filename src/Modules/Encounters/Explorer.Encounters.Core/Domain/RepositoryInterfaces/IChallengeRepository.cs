using System.Collections.Generic;

namespace Explorer.Encounters.Core.Domain.ReposotoryInterfaces;

public interface IChallengeRepository
{
    Challenge Create(Challenge c);
    Challenge Update(Challenge c);
    Challenge Get(long id);
    IEnumerable<Challenge> GetPending();
}
