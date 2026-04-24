namespace dotnet_movie_api.Module
{
    public class BookingRequestDto
    {
        public int ShowtimeId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<int> SeatIds { get; set; } = new();
    }
}
