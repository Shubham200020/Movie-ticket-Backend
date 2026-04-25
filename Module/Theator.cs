using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_movie_api.Module
{
    [Table("Theaters")]
    public class Theater
    {
        public int Id { get; set; }
        public int? LocationId { get; set; }
        public string Name { get; set; } = string.Empty;
        [ForeignKey("LocationId")]
        public Location? Location { get; set; } 
       
        public List<Screen> Screens { get; set; } = new();
    }
}
