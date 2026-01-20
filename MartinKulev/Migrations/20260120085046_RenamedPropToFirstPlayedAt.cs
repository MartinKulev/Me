using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartinKulev.Migrations
{
    /// <inheritdoc />
    public partial class RenamedPropToFirstPlayedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastPlayedAt",
                table: "ListenedSongs",
                newName: "FirstPlayedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstPlayedAt",
                table: "ListenedSongs",
                newName: "LastPlayedAt");
        }
    }
}
