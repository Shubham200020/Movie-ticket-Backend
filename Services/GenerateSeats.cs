using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.Services
{
    public class GenerateSeatsService
    {
        private readonly AppDbContext _context; // ✅ inject DB

        public GenerateSeatsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerateSeats(int screenId, int totalRows, int seatsPerRow)
        {
            var seats = new List<Seat>();

            for (int i = 0; i < totalRows; i++)
            {
                char row = (char)('A' + i);

                for (int j = 1; j <= seatsPerRow; j++)
                {
                    seats.Add(new Seat
                    {
                        Row = row.ToString(),
                        Number = j,
                        SeatType = "Regular",
                        ScreenId = screenId
                    });
                }
            }

            await _context.Seats.AddRangeAsync(seats); // ✅ async version
            await _context.SaveChangesAsync();
        }
    }
}