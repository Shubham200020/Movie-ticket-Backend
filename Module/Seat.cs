namespace dotnet_movie_api.Module
{
    public class Seat
    {
        public int Id { get; set; }
        public string Row { get; set; } = string.Empty; // e.g., "A", "B"
        public int Number { get; set; } // e.g., 1, 2, 3
        public string SeatType { get; set; } = "Regular"; // e.g., "Gold", "Premium"

        public int ScreenId { get; set; }
        public Screen Screen { get; set; } = null!;
    }
}
