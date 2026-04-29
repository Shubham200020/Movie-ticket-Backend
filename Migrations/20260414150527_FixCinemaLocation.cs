using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_movie_api.Migrations
{
    /// <inheritdoc />
    public partial class FixCinemaLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "grade",
                table: "Movies",
                newName: "Grade");

            migrationBuilder.RenameColumn(
                name: "budget",
                table: "Movies",
                newName: "Budget");

            migrationBuilder.RenameColumn(
                name: "boxOffice",
                table: "Movies",
                newName: "BoxOffice");

            migrationBuilder.AlterColumn<decimal>(
                name: "Budget",
                table: "Movies",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoxOffice",
                table: "Movies",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Cinemas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    State = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    location = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Cinemas_LocationId",
                table: "Cinemas",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cinemas_Location_LocationId",
                table: "Cinemas",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cinemas_Location_LocationId",
                table: "Cinemas");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Cinemas_LocationId",
                table: "Cinemas");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Cinemas");

            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "Movies",
                newName: "grade");

            migrationBuilder.RenameColumn(
                name: "Budget",
                table: "Movies",
                newName: "budget");

            migrationBuilder.RenameColumn(
                name: "BoxOffice",
                table: "Movies",
                newName: "boxOffice");

            migrationBuilder.AlterColumn<double>(
                name: "budget",
                table: "Movies",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "boxOffice",
                table: "Movies",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);
        }
    }
}
