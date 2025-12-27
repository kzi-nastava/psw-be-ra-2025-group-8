using System;

namespace Explorer.Encounters.Core.Utils
{
    public static class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371.0;

        /// Calculate distance between two coordinates using Haversine formula

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distanceKm = EarthRadiusKm * c;
            return distanceKm * 1000; // Convert to meters
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// Check if a position is within a certain range of a target
        public static bool IsWithinRange(double lat1, double lon1, double lat2, double lon2, double rangeMeters)
        {
            var distance = CalculateDistance(lat1, lon1, lat2, lon2);
            return distance <= rangeMeters;
        }
    }
}