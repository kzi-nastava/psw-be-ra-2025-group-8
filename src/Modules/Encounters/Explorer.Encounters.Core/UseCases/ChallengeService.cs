using System.Collections.Generic;
using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Encounters.Core.UseCases;

public class ChallengeService : IChallengeService
{
    private readonly IChallengeRepository _repo;
    private readonly IInternalPersonService _personService; // to check level
    private readonly IMapper _mapper;

    public ChallengeService(IChallengeRepository repo, IInternalPersonService personService, IMapper mapper)
    {
        _repo = repo;
        _personService = personService;
        _mapper = mapper;
    }

    public ChallengeDto CreateChallenge(CreateChallengeDto dto, long creatorPersonId)
    {
        // Note: _personService.GetByUserId expects userId; if caller passes personId adapt accordingly.
        var person = _personService.GetByUserId(creatorPersonId);
        if (person.Level < 10)
            throw new System.UnauthorizedAccessException("You must be level 10 to create a challenge.");

        var challenge = new Challenge
        {
            Name = dto.Name,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            CreatorPersonId = creatorPersonId,
            Status = ChallengeStatus.Pending,
            XPReward = dto.XPReward
        };

        var created = _repo.Create(challenge);
        return _mapper.Map<ChallengeDto>(created);
    }

    public List<ChallengeDto> GetPendingChallenges()
    {
        var list = _repo.GetPending();
        return _mapper.Map<List<ChallengeDto>>(list);
    }

    public ChallengeDto ApproveChallenge(long challengeId, long adminUserId)
    {
        var ch = _repo.Get(challengeId) ?? throw new System.Collections.Generic.KeyNotFoundException("Challenge not found");
        ch.Status = ChallengeStatus.Approved;
        ch.ApprovedAt = System.DateTime.UtcNow;
        var updated = _repo.Update(ch);
        return _mapper.Map<ChallengeDto>(updated);
    }

    public ChallengeDto GetById(long id)
    {
        var ch = _repo.Get(id);
        return ch == null ? null : _mapper.Map<ChallengeDto>(ch);
    }
}
