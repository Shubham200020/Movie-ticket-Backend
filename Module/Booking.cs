namespace dotnet_movie_api.Module
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        // Foreign Keys
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigation: One booking can have multiple seats (e.g., Row A-1, A-2)
        public List<BookingSeat> SelectedSeats { get; set; } = new();
    }
}
