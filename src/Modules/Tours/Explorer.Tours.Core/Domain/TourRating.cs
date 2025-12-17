using Explorer.BuildingBlocks.Core.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class TourRating : Entity
    {
        public int IdTour { get; set; }
        public int IdTourist { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public double TourCompletionPercentage { get; set; }

        public TourRating(int idTour, int idTourist, int rating, string? comment, double tourCompletionPercentage)
        {
            IdTour = idTour;
            IdTourist = idTourist;
            Rating = rating;
            Comment = comment;
            CreatedAt = DateTime.UtcNow;
            TourCompletionPercentage = tourCompletionPercentage;
            Validate();
        }

        public void UpdateRating(int rating)
        {
            Rating = rating;
            Validate();
        }

        public void UpdateComment(string? comment)
        {
            Comment = comment;
        }

        public void UpdateTourCompletionPercentage(double tourCompletionPercentage)
        {
            TourCompletionPercentage = tourCompletionPercentage;
            Validate();
        }

        private void Validate() {
            //if (IdTour <= 0)
                //throw new ArgumentException("IdTour must be greater than zero.");
            //if (IdTourist <= 0)
               // throw new ArgumentException("IdTourist must be greater than zero.");
            if (Rating < 1 || Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");
            if (TourCompletionPercentage < 0 || TourCompletionPercentage > 100)
                throw new ArgumentException("TourCompletionPercentage must be between 0 and 100.");
        }
    }
}
