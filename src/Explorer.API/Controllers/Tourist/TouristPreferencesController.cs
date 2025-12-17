using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
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
        // PersonId se automatski uzima iz JWT tokena ulogovanog korisnika
        [HttpPut]
        public ActionResult<TouristPreferencesDto> Update([FromBody] UpdateTouristPreferencesDto updateDto)
        {
            long personId = User.PersonId();

            // Šaljemo i ID osobe i DTO. Servis pronalazi postojeće po personId i ažurira ih
            var result = _touristPreferencesService.Update(personId, updateDto);

            if (result == null)
            {
                return NotFound(); // Ako preferencije ne postoje za tog korisnika
            }

            return Ok(result);
        }
    }

}

