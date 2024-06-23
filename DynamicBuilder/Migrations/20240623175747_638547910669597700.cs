using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicDbContextAssembly.Migrations
{
    /// <inheritdoc />
    public partial class _638547910669597700 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name1",
                table: "Tests",
                newName: "Name2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name2",
                table: "Tests",
                newName: "Name1");
        }
    }
}
