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
    public class ReportProblemService : IReportProblemService
    {
        private readonly ICrudRepository<ReportProblem> _crudRepository;
        private readonly IMapper _mapper;

        public ReportProblemService(ICrudRepository<ReportProblem> repository, IMapper mapper)
        {
            _crudRepository = repository;
            _mapper = mapper;
        }

        public PagedResult<ReportProblemDto> GetPaged(int page, int pageSize)
        {
            var result = _crudRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<ReportProblemDto>).ToList();
            return new PagedResult<ReportProblemDto>(items, result.TotalCount);
        }

        public ReportProblemDto Create(ReportProblemDto entity)
        {
            var domainEntity = new ReportProblem(
                entity.TourId,
                entity.TouristId,
                (ReportCategory)entity.Category,
                (ReportPriority)entity.Priority,
                entity.Description
            );

            var result = _crudRepository.Create(domainEntity);
            return _mapper.Map<ReportProblemDto>(result);
        }

        public ReportProblemDto Update(ReportProblemDto entity)
        {
            // Since ReportProblem properties are immutable (init-only), 
            // we need to use the mapper which can handle the construction properly
            var domainEntity = _mapper.Map<ReportProblem>(entity);
            var result = _crudRepository.Update(domainEntity);
            return _mapper.Map<ReportProblemDto>(result);
        }

        public void Delete(long id)
        {
            _crudRepository.Delete(id);
        }
    }
}