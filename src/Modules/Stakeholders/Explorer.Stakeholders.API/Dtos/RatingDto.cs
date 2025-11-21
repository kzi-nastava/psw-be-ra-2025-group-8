namespace Explorer.Stakeholders.API.Dtos
{
    public class RatingDto
    {
        public int Id { get; set; }
        //public long UserId { get; set; }
        public int Grade { get; set; }
        public string? Comment { get; set; }
        public DateTime CreationDate { get; set; }
    }
}