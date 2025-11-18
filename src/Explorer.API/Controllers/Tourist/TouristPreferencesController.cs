using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tourist-preferences")]
    [ApiController]
    public class TouristPreferencesController : ControllerBase
    {
        private readonly ITouristPreferencesService _touristPreferencesService;

        public TouristPreferencesController(ITouristPreferencesService touristPreferencesService)
        {
            _touristPreferencesService = touristPreferencesService;
        }

        [HttpGet]
        public ActionResult<TouristPreferencesDto> Get()
        {
            // ID osobe se izvlači iz JWT tokena prijavljenog korisnika
            long personId = User.PersonId();

            var result = _touristPreferencesService.Get(personId);

            // Provera da li preferencije postoje
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // URL: PUT /api/tourist/tourist-preferences
        [HttpPut]
        public ActionResult<TouristPreferencesDto> Update([FromBody] TouristPreferencesDto touristPreferencesDto)
        {
            long personId = User.PersonId();

            // Šaljemo i ID osobe i DTO. Servis pronalazi postojeće po personId i ažurira ih
            var result = _touristPreferencesService.Update(personId, touristPreferencesDto);

            if (result == null)
            {
                return NotFound(); // Ako preferencije ne postoje za tog korisnika
            }

            return Ok(result);
        }
    }

}

