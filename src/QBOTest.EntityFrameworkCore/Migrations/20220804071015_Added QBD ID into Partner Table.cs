using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QBOTest.Migrations
{
    public partial class AddedQBDIDintoPartnerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuickBooksDesktopId",
                table: "Partners",
                type: "nvarchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsQBSynced",
                table: "Partners",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuickBooksDesktopId",
                table: "Partners");

            migrationBuilder.DropColumn(
              name: "IsQBSynced",
              table: "Partners");
        }
    }
}
