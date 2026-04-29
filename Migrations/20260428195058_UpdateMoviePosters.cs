using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_movie_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMoviePosters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterUrl",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "WidePosterUrl",
                table: "Movies");

            migrationBuilder.AddColumn<byte[]>(
                name: "PosterData",
                table: "Movies",
                type: "longblob",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "WidePosterData",
                table: "Movies",
                type: "longblob",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterData",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "WidePosterData",
                table: "Movies");

            migrationBuilder.AddColumn<string>(
                name: "PosterUrl",
                table: "Movies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "WidePosterUrl",
                table: "Movies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
