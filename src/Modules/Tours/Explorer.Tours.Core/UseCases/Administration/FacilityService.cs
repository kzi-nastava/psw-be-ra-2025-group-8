using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class FacilityService : IFacilityService
    {
        private readonly ICrudRepository<Facility> _crudRepository;
        private readonly IMapper _mapper;

        public FacilityService(ICrudRepository<Facility> repository, IMapper mapper)
        {
            _crudRepository = repository;
            _mapper = mapper;
        }

        public PagedResult<FacilityDto> GetPaged(int page, int pageSize)
        {
            var result = _crudRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<FacilityDto>).ToList();
            return new PagedResult<FacilityDto>(items, result.TotalCount);
        }

        public FacilityDto Create(FacilityDto entity)
        {
            var result = _crudRepository.Create(_mapper.Map<Facility>(entity));
            return _mapper.Map<FacilityDto>(result);
        }

        public FacilityDto Update(FacilityDto entity)
        {
            var result = _crudRepository.Update(_mapper.Map<Facility>(entity));
            return _mapper.Map<FacilityDto>(result);
        }

        public void Delete(long id)
        {
            _crudRepository.Delete(id);
        }
    }
}
