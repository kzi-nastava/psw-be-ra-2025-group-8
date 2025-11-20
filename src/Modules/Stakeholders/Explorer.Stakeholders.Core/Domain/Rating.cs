using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Rating : Entity
    {
        public long UserId {  get; set; } 
        public int Grade { get; private set; }
        public string? Comment {  get; private set; }
        public DateTime CreationDate {  get; set; }

        public Rating()
        {
        }
        public Rating(long userId, int grade, string? comment)
        {
            Validate(userId, grade);

            UserId = userId;
            Grade = grade;
            Comment = comment;
            CreationDate = DateTime.Now;
        }

        public void Validate(long userId, int grade)
        {
            //if (userId < 0)
            //{
              //  throw new ArgumentException("Invalid User ID.");
            //}

            if (grade < 1 || grade > 5)
            {
                throw new ArgumentException("Invalid Grade. Grade must be between 1 and 5.");
            }
        }
    }
}
