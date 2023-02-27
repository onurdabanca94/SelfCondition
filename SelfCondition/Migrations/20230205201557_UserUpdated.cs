using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfCondition.Migrations
{
    public partial class UserUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "user"); //Normalde migration oluştuğunda boş string "" -> gelmişti db'deki role kısmına boş string gelmesin varsayılan olarak user eklesin.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}
