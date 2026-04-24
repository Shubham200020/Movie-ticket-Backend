namespace dotnet_movie_api.Controllers
{
    public class CreateBookingDto
    {
        public int UserId { get; set; }
        public int ShowId { get; set; }
        public List<int> SeatIds { get; set; }
    }
}