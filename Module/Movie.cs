using System.ComponentModel.DataAnnotations;

namespace dotnet_movie_api.Module
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Genre { get; set; } // ✅ FIXED

        [MaxLength(4)]
        public string? Grade { get; set; }

        public int Hour { get; set; }
        public int Min { get; set; }

        public decimal? BoxOffice { get; set; }
        public decimal? Budget { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string? PosterUrl { get; set; }
        public string? WidePosterUrl { get; set; }

        public bool Recomended { get; set; }
        public bool Running { get; set; } = true;
    }
}
