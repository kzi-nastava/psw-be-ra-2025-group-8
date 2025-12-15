using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
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
        private readonly IReportProblemRepository _reportProblemRepository;
        private readonly ICrudRepository<Tour> _tourRepository;
        private readonly IIssueNotificationService _notificationService;
        private readonly IMapper _mapper;

        public ReportProblemService(
            ICrudRepository<ReportProblem> repository, 
            IReportProblemRepository reportProblemRepository,
            ICrudRepository<Tour> tourRepository, 
            IIssueNotificationService notificationService,
            IMapper mapper)
        {
            _crudRepository = repository;
            _reportProblemRepository = reportProblemRepository;
            _tourRepository = tourRepository;
            _notificationService = notificationService;
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
            // Since ReportProblem properties are mutable now, we can map and update.
            var domainEntity = _mapper.Map<ReportProblem>(entity);
            var result = _crudRepository.Update(domainEntity);
            return _mapper.Map<ReportProblemDto>(result);
        }

        public void Delete(long id)
        {
            _crudRepository.Delete(id);
        }

        // Novi metod: Author odgovara na prijavu (BEZ provere autorstva - samo za test)
        public ReportProblemDto AuthorRespond(int reportId, int authorId, string response)
        {
            var report = _crudRepository.Get(reportId);
            report.RespondByAuthor(authorId, response);
            var updated = _crudRepository.Update(report);
            return _mapper.Map<ReportProblemDto>(updated);
        }

        // Novi metod: Tourist mark resolved/unresolved
        public ReportProblemDto MarkResolved(int reportId, bool resolved, string? comment)
        {
            var report = _crudRepository.Get(reportId);
            report.MarkResolved(resolved, comment);
            var updated = _crudRepository.Update(report);
            return _mapper.Map<ReportProblemDto>(updated);
        }

        // Dodavanje poruke u prijavu problema
        public IssueMessageDto AddMessage(int reportId, int authorId, string content)
        {
            var report = _reportProblemRepository.GetWithMessages(reportId);
            report.AddMessage(authorId, content);
            var updated = _crudRepository.Update(report);

            var addedMessage = updated.Messages.LastOrDefault();
            
            // Kreiranje notifikacija preko callback interfejsa
            var tour = _tourRepository.Get(updated.TourId);
            _notificationService.NotifyAboutNewMessage(
                updated.TouristId,
                tour.AuthorId,
                reportId,
                content,
                authorId
            );
            
            return _mapper.Map<IssueMessageDto>(addedMessage);
        }

        // Dobijanje svih poruka za prijavu
        public List<IssueMessageDto> GetMessages(int reportId)
        {
            var report = _reportProblemRepository.GetWithMessages(reportId);
            return _mapper.Map<List<IssueMessageDto>>(report.Messages);
        }

        // Dobijanje pojedinačne prijave po Id-u
        public ReportProblemDto GetById(int reportId)
        {
            var report = _reportProblemRepository.GetWithMessages(reportId);
            return _mapper.Map<ReportProblemDto>(report);
        }
        // Administrator postavlja rok za rešavanje
        public ReportProblemDto SetDeadline(int reportId, DateTime deadline)
        {
            var report = _crudRepository.Get(reportId);
            report.SetDeadline(deadline);
            var updated = _crudRepository.Update(report);

            // Pošalji notifikaciju autoru ture
            var tour = _tourRepository.Get(updated.TourId);
            _notificationService.NotifyAboutNewMessage(
                updated.TouristId,           // Pošiljalac može biti turistički ID, ali ovde je OK koristiti turistu jer sistem zna oba korisnika
                tour.AuthorId,               // Autor dobija obaveštenje
                reportId,
                $"Administrator je postavio rok za rešavanje problema do {deadline:dd.MM.yyyy. HH:mm}.",
                0                            // 0 = sistem / administrator
            );

            return _mapper.Map<ReportProblemDto>(updated);
        }

        // Administrator zatvara problem bez kazne
        public ReportProblemDto CloseIssueByAdmin(int reportId)
        {
            var report = _crudRepository.Get(reportId);
            report.CloseOrPenalize(false);
            var updated = _crudRepository.Update(report);

            return _mapper.Map<ReportProblemDto>(updated);
        }

        // Administrator penalizuje autora (npr. zatvara turu)
        public ReportProblemDto PenalizeAuthor(int reportId)
        {
            var report = _crudRepository.Get(reportId);
            report.CloseOrPenalize(true);
            var updated = _crudRepository.Update(report);

            // Moguće: deaktivacija ture (ugasi turu)
            var tour = _tourRepository.Get(updated.TourId);
            if (tour != null)
            {
                tour.Status = TourStatus.Archived; // ili 0 ako je enum int
                _tourRepository.Update(tour);
            }

            return _mapper.Map<ReportProblemDto>(updated);
        }

    }
}