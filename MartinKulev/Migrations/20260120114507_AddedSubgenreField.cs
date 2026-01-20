using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartinKulev.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubgenreField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subgenre",
                table: "ListenedSongs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subgenre",
                table: "ListenedSongs");
        }
    }
}
