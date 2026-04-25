using dotnet_movie_api.Module;
using Microsoft.EntityFrameworkCore;
namespace dotnet_movie_api.Databace
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
       // public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Show> Shows { get;  set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Admin> Admins { get; set; }
      
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; internal set; }
        public DbSet<Location> Locations { get; internal set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This forces EF to look for "Theators" table instead of "theaters"
            //  modelBuilder.Entity<Theator>().ToTable("Theators");
            modelBuilder.Entity<Theater>().ToTable("Theaters");
            // It is good practice to do the same for Screens
            modelBuilder.Entity<Screen>().ToTable("Screens");

            base.OnModelCreating(modelBuilder);
        }
    }
}

