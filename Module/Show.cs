namespace dotnet_movie_api.Module
{
    public class Show
    {
        public int Id { get; set; }

        public DateTime ShowTime { get; set; }

        // Foreign Keys
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int CinemaId { get; set; }
      
        // Relationship
        public List<Seat> Seats { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
