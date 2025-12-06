using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IPreferenceTagsService
    {
        IEnumerable<TagDto> GetTagsForPerson(long personId);
        TagDto AddTagForPerson(long personId, TagDto dto);
        void RemoveTagFromPerson(long personId, long tagId);
    }
}

