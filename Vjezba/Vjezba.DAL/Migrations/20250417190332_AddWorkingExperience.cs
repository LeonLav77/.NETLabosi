using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vjezba.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Cities_CityID",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "CityID",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingExperience",
                table: "Clients",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "ID",
                keyValue: 1,
                column: "WorkingExperience",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Cities_CityID",
                table: "Clients",
                column: "CityID",
                principalTable: "Cities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Cities_CityID",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "WorkingExperience",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "CityID",
                table: "Clients",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Cities_CityID",
                table: "Clients",
                column: "CityID",
                principalTable: "Cities",
                principalColumn: "ID");
        }
    }
}
