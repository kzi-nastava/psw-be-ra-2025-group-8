using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-rating-image")]
    [ApiController]
    public class TourRatingImageController : ControllerBase
    {
        private readonly ITourRatingImageService _imageService;
        private readonly IWebHostEnvironment _environment;

        public TourRatingImageController(ITourRatingImageService imageService, IWebHostEnvironment environment)
        {
            _imageService = imageService;
            _environment = environment;
        }

        [HttpPost("upload/{tourRatingId:long}")]
        public async Task<ActionResult<TourRatingImageDto>> UploadImage(long tourRatingId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Only images are allowed.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tour-ratings");
            
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/images/tour-ratings/{fileName}";

            var imageDto = new TourRatingImageDto
            {
                TourRatingId = tourRatingId,
                Url = imageUrl
            };

            var result = _imageService.Create(imageDto);
            return Ok(result);
        }

        [HttpGet("tour-rating/{tourRatingId:long}")]
        public ActionResult<List<TourRatingImageDto>> GetByTourRatingId(long tourRatingId)
        {
            var images = _imageService.GetByTourRatingId(tourRatingId);
            return Ok(images);
        }

        [HttpGet("{id:long}")]
        public ActionResult<TourRatingImageDto> Get(long id)
        {
            var image = _imageService.Get(id);
            if (image == null) return NotFound();
            return Ok(image);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var image = _imageService.Get(id);
            if (image == null) return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, image.Url.TrimStart('/'));
            
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _imageService.Delete(id);
            return Ok();
        }
    }
}
