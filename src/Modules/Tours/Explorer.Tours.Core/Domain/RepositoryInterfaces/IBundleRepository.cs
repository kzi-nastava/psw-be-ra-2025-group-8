using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IBundleRepository
{
    Bundle Get(long id);
    List<Bundle> GetByAuthor(int authorId);
    List<Bundle> GetPublished();

    Bundle Create(Bundle bundle);
    Bundle Update(Bundle bundle);
    void Delete(long id);
}

