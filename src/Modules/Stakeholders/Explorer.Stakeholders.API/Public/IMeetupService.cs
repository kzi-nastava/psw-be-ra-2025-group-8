using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMeetupService
    {
        MeetupDto Create(MeetupDto dto);
        MeetupDto Update(MeetupDto dto);
        void Delete(int id);
        MeetupDto Get(int id);
        IEnumerable<MeetupDto> GetByCreator(int creatorId);
    }
}
