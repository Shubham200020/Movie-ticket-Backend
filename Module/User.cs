namespace dotnet_movie_api.Module
{
    public class User
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }
        public string Role { get; set; } = "user";

        // Relationship
        public List<Booking>? Bookings { get; set; }
    }
}
