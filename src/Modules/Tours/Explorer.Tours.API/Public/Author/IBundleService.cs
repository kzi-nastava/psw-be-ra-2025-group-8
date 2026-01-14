using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Author;

public interface IBundleService
{
    List<BundleDto> GetByAuthor(int authorId);
    BundleDto GetById(long id, int authorId);

    BundleDto Create(CreateBundleDto dto, int authorId);
    BundleDto Update(long id, UpdateBundleDto dto, int authorId);

    void Delete(long id, int authorId);

    BundleDto Publish(long id, int authorId);
    BundleDto Archive(long id, int authorId);
}

