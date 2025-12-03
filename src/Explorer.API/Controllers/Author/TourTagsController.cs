using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Route("api/author/tours/{tourId}/tags")]
    [ApiController]
    [Authorize(Roles = "author")]
    public class TourTagsController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourTagsController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // ============================================================
        //  PUT api/author/tours/{tourId}/tags
        //  Update entire tag list for a tour
        // ============================================================
        [HttpPut]
        public ActionResult<TourDto> UpdateTags(long tourId, [FromBody] UpdateTourTagsDto dto)
        {
            int authorId = int.Parse(User.FindFirst("id").Value);

            var result = _tourService.UpdateTags(tourId, dto.Tags, authorId);
            return Ok(result);
        }

        // ============================================================
        //  POST api/author/tours/{tourId}/tags/{tag}
        //  Add a single tag
        // ============================================================
        [HttpPost("{tag}")]
        public ActionResult<TourDto> AddTag(long tourId, string tag)
        {
            int authorId = int.Parse(User.FindFirst("id").Value);

            var result = _tourService.AddTag(tourId, tag, authorId);
            return Ok(result);
        }

        // ============================================================
        //  DELETE api/author/tours/{tourId}/tags/{tag}
        //  Remove a single tag
        // ============================================================
        [HttpDelete("{tag}")]
        public ActionResult<TourDto> RemoveTag(long tourId, string tag)
        {
            int authorId = int.Parse(User.FindFirst("id").Value);

            var result = _tourService.RemoveTag(tourId, tag, authorId);
            return Ok(result);
        }
    }
}
