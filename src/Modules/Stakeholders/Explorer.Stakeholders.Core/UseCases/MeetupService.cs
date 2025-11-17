using System.Collections.Generic;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class MeetupService : IMeetupService
    {
        private readonly IMeetupRepository _repo;
        private readonly IMapper _mapper;

        public MeetupService(IMeetupRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public MeetupDto Create(MeetupDto dto)
        {
            var entity = _mapper.Map<Meetup>(dto);
            var result = _repo.Create(entity);
            return _mapper.Map<MeetupDto>(result);
        }

        public MeetupDto Update(MeetupDto dto)
        {
            var entity = _mapper.Map<Meetup>(dto);
            var result = _repo.Update(entity);
            return _mapper.Map<MeetupDto>(result);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public MeetupDto Get(int id)
        {
            var entity = _repo.Get(id);
            return _mapper.Map<MeetupDto>(entity);
        }

        public IEnumerable<MeetupDto> GetByCreator(int creatorId)
        {
            var list = _repo.GetByCreator(creatorId);
            return _mapper.Map<IEnumerable<MeetupDto>>(list);
        }
    }
}
