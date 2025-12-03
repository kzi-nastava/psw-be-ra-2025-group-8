namespace Explorer.Blog.API.Dtos
{
    public class CommentDto
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastEditTime { get; set; }
    }
}