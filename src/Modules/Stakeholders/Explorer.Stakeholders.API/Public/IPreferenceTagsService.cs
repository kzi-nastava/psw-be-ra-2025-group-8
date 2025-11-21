using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IPreferenceTagsService
    {
        IEnumerable<TagDto> GetTagsForPerson(long personId);
        TagDto AddTagForPerson(long personId, TagDto dto);
        void RemoveTagFromPerson(long personId, long tagId);
    }
}

