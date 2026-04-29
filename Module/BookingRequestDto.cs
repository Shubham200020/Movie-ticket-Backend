namespace dotnet_movie_api.Module
{
    public class BookingRequestDto
    {
        public int ShowtimeId { get; set; }
        public int UserId { get; set; }
        public List<int> SeatIds { get; set; } = new();
    }
}
