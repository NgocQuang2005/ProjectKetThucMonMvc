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
            migrationBuilder.AlterColumn<string>(
                name: "UrlDocument",
                table: "DocumentInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdateBy",
                table: "AccountDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "AccountDetails",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<long>(
                name: "CCCD",
                table: "AccountDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AccountDetails",
                keyColumn: "IdAccountDt",
                keyValue: 1,
                columns: new[] { "CCCD", "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { 123456789L, new DateTime(2024, 9, 21, 16, 51, 44, 572, DateTimeKind.Local).AddTicks(1857), new DateTime(2024, 9, 21, 16, 51, 44, 572, DateTimeKind.Local).AddTicks(1858) });

            migrationBuilder.UpdateData(
                table: "AccountDetails",
                keyColumn: "IdAccountDt",
                keyValue: 2,
                columns: new[] { "CCCD", "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { 987654321L, new DateTime(2024, 9, 21, 16, 51, 44, 572, DateTimeKind.Local).AddTicks(1861), new DateTime(2024, 9, 21, 16, 51, 44, 572, DateTimeKind.Local).AddTicks(1862) });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "IdAccount",
                keyValue: 1,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 9, 21, 16, 51, 44, 572, DateTimeKind.Local).AddTicks(1817), new DateTime(2024, 9, 21, 16, 51, 44, 572, DateTimeKind.Local).AddTicks(1818) });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "IdAccount",
                keyValue: 2,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 9, 21, 16, 51, 44, 572, DateTimeKind.Local).AddTicks(1821), new DateTime(2024, 9, 21, 16, 51, 44, 572, DateTimeKind.Local).AddTicks(1821) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UrlDocument",
                table: "DocumentInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdateBy",
                table: "AccountDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "AccountDetails",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CCCD",
                table: "AccountDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.UpdateData(
                table: "AccountDetails",
                keyColumn: "IdAccountDt",
                keyValue: 1,
                columns: new[] { "CCCD", "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { 123456789, new DateTime(2024, 9, 13, 16, 50, 22, 125, DateTimeKind.Local).AddTicks(7424), new DateTime(2024, 9, 13, 16, 50, 22, 125, DateTimeKind.Local).AddTicks(7425) });

            migrationBuilder.UpdateData(
                table: "AccountDetails",
                keyColumn: "IdAccountDt",
                keyValue: 2,
                columns: new[] { "CCCD", "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { 987654321, new DateTime(2024, 9, 13, 16, 50, 22, 125, DateTimeKind.Local).AddTicks(7430), new DateTime(2024, 9, 13, 16, 50, 22, 125, DateTimeKind.Local).AddTicks(7431) });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "IdAccount",
                keyValue: 1,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 9, 13, 16, 50, 22, 125, DateTimeKind.Local).AddTicks(7376), new DateTime(2024, 9, 13, 16, 50, 22, 125, DateTimeKind.Local).AddTicks(7377) });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "IdAccount",
                keyValue: 2,
                columns: new[] { "CreatedWhen", "LastUpdateWhen" },
                values: new object[] { new DateTime(2024, 9, 13, 16, 50, 22, 125, DateTimeKind.Local).AddTicks(7381), new DateTime(2024, 9, 13, 16, 50, 22, 125, DateTimeKind.Local).AddTicks(7382) });
        }
    }
}
