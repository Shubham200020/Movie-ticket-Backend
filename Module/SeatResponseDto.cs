namespace dotnet_movie_api.Module
{
    public class SeatResponseDto
    {
        public int Id { get; set; }
        public string Row { get; set; } = string.Empty;
        public int Number { get; set; }
        public bool IsAvailable { get; set; }
    }
}
