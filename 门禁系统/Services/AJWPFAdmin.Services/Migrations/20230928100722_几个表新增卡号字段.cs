using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJWPFAdmin.Services.Migrations
{
    /// <inheritdoc />
    public partial class 几个表新增卡号字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Watchhouses",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Warehouses",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "WarehousePassageRecords",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<string>(
                name: "IDCardNo",
                table: "WarehousePassageRecords",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true,
                comment: "车牌号")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "SystemConfigDictionaries",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "ShippingRecords",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<string>(
                name: "IDCardNo",
                table: "ShippingRecords",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true,
                comment: "卡号")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Passageways",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "MaterialDictionaries",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Devices",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "DatabaseBackupRecords",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "CarTypes",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Cars",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<string>(
                name: "IDCardNo",
                table: "Cars",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true,
                comment: "卡号")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480306L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480307L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480308L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900));

            migrationBuilder.UpdateData(
                table: "PlatformCustomers",
                keyColumn: "Id",
                keyValue: 480306,
                columns: new[] { "CreateDate", "ExpireDate" },
                values: new object[] { new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900), new DateTime(2024, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900) });

            migrationBuilder.UpdateData(
                table: "SystemRoles",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900), new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900), new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDCardNo",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "IDCardNo",
                table: "ShippingRecords");

            migrationBuilder.DropColumn(
                name: "IDCardNo",
                table: "Cars");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Watchhouses",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Warehouses",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "WarehousePassageRecords",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "SystemConfigDictionaries",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "ShippingRecords",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Passageways",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "MaterialDictionaries",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Devices",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "DatabaseBackupRecords",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "CarTypes",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersionTimestamp",
                table: "Cars",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480306L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480307L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480308L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312));

            migrationBuilder.UpdateData(
                table: "PlatformCustomers",
                keyColumn: "Id",
                keyValue: 480306,
                columns: new[] { "CreateDate", "ExpireDate" },
                values: new object[] { new DateTime(2023, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312), new DateTime(2024, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312) });

            migrationBuilder.UpdateData(
                table: "SystemRoles",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312), new DateTime(2023, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312), new DateTime(2023, 9, 27, 8, 55, 38, 941, DateTimeKind.Local).AddTicks(7312) });
        }
    }
}
