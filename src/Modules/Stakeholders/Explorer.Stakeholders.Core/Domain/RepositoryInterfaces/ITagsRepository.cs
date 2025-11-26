using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface ITagsRepository
    {
        Tags GetById(long id);
        Tags GetByName(string tagNormalized);
        Tags Create(Tags tag);
    }
}
