using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicDbContextAssembly.Migrations
{
    /// <inheritdoc />
    public partial class _638547907504007740 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tests",
                newName: "Name1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name1",
                table: "Tests",
                newName: "Name");
        }
    }
}
