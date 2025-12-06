using System.ComponentModel.DataAnnotations;

namespace Explorer.Blog.API.Dtos
{
    public class CommentCreationDto
    {
        [Required]
        public string Text { get; set; }
    }
}