namespace dotnet_movie_api.Module
{
    public class Theator
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
       
        public List<Screen> Screens { get; set; } = new();
    }
}
