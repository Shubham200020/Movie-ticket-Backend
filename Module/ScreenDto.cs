namespace dotnet_movie_api.Module
{
    public class ScreenDto
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string ScreenType { get; set; } = "2D";
        public int TheaterId { get; set; }
    }
}
