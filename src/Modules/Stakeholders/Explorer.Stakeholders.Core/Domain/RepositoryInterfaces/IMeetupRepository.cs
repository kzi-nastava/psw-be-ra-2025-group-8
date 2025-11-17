using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IMeetupRepository
    {
        Meetup Create(Meetup meetup);
        Meetup Update(Meetup meetup);
        void Delete(int id);
        Meetup Get(int id);
        IEnumerable<Meetup> GetByCreator(int creatorId);
    }
}
