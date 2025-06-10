using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiApp01.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoItensToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
