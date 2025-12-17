using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourRatingImageService : ITourRatingImageService
    {
        private readonly ITourRatingImageRepository _repository;
        private readonly IMapper _mapper;

        public TourRatingImageService(ITourRatingImageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TourRatingImageDto Create(TourRatingImageDto imageDto)
        {
            var image = new TourRatingImage(imageDto.TourRatingId, imageDto.Url);
            var result = _repository.Create(image);
            return _mapper.Map<TourRatingImageDto>(result);
        }

        public List<TourRatingImageDto> GetByTourRatingId(long tourRatingId)
        {
            var images = _repository.GetByTourRatingId(tourRatingId);
            return images.Select(_mapper.Map<TourRatingImageDto>).ToList();
        }

        public TourRatingImageDto Get(long id)
        {
            var image = _repository.Get(id);
            return image != null ? _mapper.Map<TourRatingImageDto>(image) : null;
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }
    }
}
