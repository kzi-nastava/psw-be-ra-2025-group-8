using System.Collections.Generic;
using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository repo, IMapper mapper)
        {
            _clubRepository = repo;
            _mapper = mapper;
        }

        public PagedResult<ClubDto> GetPaged(int page, int pageSize)
        {
            var result = _clubRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<ClubDto>).ToList();
            return new PagedResult<ClubDto>(items, result.TotalCount);
        }
        public ClubDto Create(CreateClubDto dto, long ownerId)
        {
            var club = new Club(ownerId, dto.Name, dto.Description, dto.ImageUrls);
            var created = _clubRepository.Create(club);
            return _mapper.Map<ClubDto>(created);
        }
        public ClubDto Get(long id)
        {
            var club = _clubRepository.Get(id);
            return _mapper.Map<ClubDto>(club);
        }
        public IEnumerable<ClubDto> GetAll()
        {
            var clubs = _clubRepository.GetAll();
            return _mapper.Map<IEnumerable<ClubDto>>(clubs);
        }
        public void Join(long id, long touristId)
        {
            var club = _clubRepository.Get(id);
            club.AddMember(touristId);
            _clubRepository.Update(club);
        }
        public ClubDto Update(long id, long current_owner_id, ClubDto dto, long user_id)
        {
            if (user_id != current_owner_id)
                throw new UnauthorizedAccessException("Only the owner can update the club");

            // 1. fetch postojeći klub
            var existing = _clubRepository.Get(id);

            // 2. update polja
            existing.Update(dto.Name, dto.Description, dto.ImageUrls);

            // 3. sačuvaj
            var updated = _clubRepository.Update(existing);

            return _mapper.Map<ClubDto>(updated);
        }


        public void Delete(long userId, long id)
        {
            var club = _clubRepository.Get(id);
            if (club.OwnerId != userId)
                throw new UnauthorizedAccessException("Only the owner can delete the club");
            _clubRepository.Delete(id);
        }
    }
}
