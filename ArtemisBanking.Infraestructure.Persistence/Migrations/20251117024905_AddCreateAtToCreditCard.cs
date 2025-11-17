using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtemisBanking.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateAtToCreditCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Beneficiaries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "Beneficiaries",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "Beneficiaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "Beneficiaries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "Beneficiaries");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Beneficiaries",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "Beneficiaries",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);
        }
    }
}
