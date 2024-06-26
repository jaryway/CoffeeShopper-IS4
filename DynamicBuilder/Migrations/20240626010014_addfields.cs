using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicBuilder.Migrations
{
    /// <inheritdoc />
    public partial class addfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "SourceCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "SourceCodes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "SourceCodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SourceCodes");

            migrationBuilder.DropColumn(
                name: "Published",
                table: "SourceCodes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SourceCodes");
        }
    }
}
