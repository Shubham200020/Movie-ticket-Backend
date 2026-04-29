namespace dotnet_movie_api.Module
{
    public class Screen
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., "Screen 1" or "IMAX"
        public int Capacity { get; set; }

        public int TheaterId { get; set; }
        public Theater Theater { get; set; } = null!;

        // Navigation properties
        public List<Seat> Seats { get; set; } = new();
        public List<Showtime> Showtimes { get; set; } = new();
    }
}
