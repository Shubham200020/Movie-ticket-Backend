using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_movie_api.Module
{
    public class Showtime
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal BasePrice { get; set; }

        // Foreign Keys
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        
        public int ScreenId { get; set; }
        public Screen Screen { get; set; } = null!;

        // Navigation property: One showtime has many specific seat bookings
        public List<Booking> Bookings { get; set; } = new();
    }
}
