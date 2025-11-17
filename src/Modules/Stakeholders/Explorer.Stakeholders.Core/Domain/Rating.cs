using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    internal class Rating : Entity
    {
        public long UserId {  get; init; } 
        public int Grade { get; private set; }
        public string? Comment {  get; private set; }
        public DateTime CreationDate {  get; private set; }
        public Rating(long userId, int grade, string? comment)
        {
            Validate(userId, grade);

            UserId = userId;
            Grade = grade;
            Comment = comment;
            CreationDate = DateTime.Now;
        }

        private void Validate(long userId, int grade)
        {
            if (userId < 0)
            {
                throw new ArgumentException("Invalid User ID.");
            }

            if (grade < 1 || grade > 5)
            {
                throw new ArgumentException("Invalid Grade. Grade must be between 1 and 5.");
            }
        }
    }
}
