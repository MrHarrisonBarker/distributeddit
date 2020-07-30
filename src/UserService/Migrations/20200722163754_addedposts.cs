using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Migrations
{
    public partial class addedposts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostIds",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostIds",
                table: "Users");
        }
    }
}
