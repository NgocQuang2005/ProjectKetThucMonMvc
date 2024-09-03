using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class Initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AccountDetails_IdAcDt",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_AccountIdAccount",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_AccountIdAccount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "AccountIdAccount",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "IdAcDt",
                table: "Events",
                newName: "IdAc");

            migrationBuilder.RenameIndex(
                name: "IX_Events_IdAcDt",
                table: "Events",
                newName: "IX_Events_IdAc");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdateBy",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AccountDetails",
                keyColumn: "IdAccountDt",
                keyValue: 1,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3441), new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3441) });

            migrationBuilder.UpdateData(
                table: "AccountDetails",
                keyColumn: "IdAccountDt",
                keyValue: 2,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3445), new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3446) });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "IdAccount",
                keyValue: 1,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3411), new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3412) });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "IdAccount",
                keyValue: 2,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3415), new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3415) });

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Accounts_IdAc",
                table: "Events",
                column: "IdAc",
                principalTable: "Accounts",
                principalColumn: "IdAccount",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_IdAc",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "IdAc",
                table: "Events",
                newName: "IdAcDt");

            migrationBuilder.RenameIndex(
                name: "IX_Events_IdAc",
                table: "Events",
                newName: "IX_Events_IdAcDt");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdateBy",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AccountIdAccount",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AccountDetails",
                keyColumn: "IdAccountDt",
                keyValue: 1,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1193), new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1194) });

            migrationBuilder.UpdateData(
                table: "AccountDetails",
                keyColumn: "IdAccountDt",
                keyValue: 2,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1198), new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1199) });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "IdAccount",
                keyValue: 1,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1154), new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1155) });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "IdAccount",
                keyValue: 2,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1158), new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1159) });

            migrationBuilder.CreateIndex(
                name: "IX_Events_AccountIdAccount",
                table: "Events",
                column: "AccountIdAccount");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AccountDetails_IdAcDt",
                table: "Events",
                column: "IdAcDt",
                principalTable: "AccountDetails",
                principalColumn: "IdAccountDt",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Accounts_AccountIdAccount",
                table: "Events",
                column: "AccountIdAccount",
                principalTable: "Accounts",
                principalColumn: "IdAccount");
        }
    }
}
