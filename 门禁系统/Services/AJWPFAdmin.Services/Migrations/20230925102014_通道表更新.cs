using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJWPFAdmin.Services.Migrations
{
    /// <inheritdoc />
    public partial class 通道表更新 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemporaryVehicleAccess",
                table: "Passageways");

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

            migrationBuilder.AddColumn<bool>(
                name: "CountCarEnable",
                table: "Passageways",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                comment: "是否统计车辆");

            migrationBuilder.AddColumn<long>(
                name: "WarehouseId",
                table: "Passageways",
                type: "bigint",
                nullable: true,
                comment: "关联仓库id");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "Passageways",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "关联仓库名称")
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480306L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480307L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480308L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973));

            migrationBuilder.UpdateData(
                table: "PlatformCustomers",
                keyColumn: "Id",
                keyValue: 480306,
                columns: new[] { "CreateDate", "ExpireDate" },
                values: new object[] { new DateTime(2023, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973), new DateTime(2024, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973) });

            migrationBuilder.UpdateData(
                table: "SystemRoles",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973), new DateTime(2023, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973), new DateTime(2023, 9, 25, 18, 20, 14, 758, DateTimeKind.Local).AddTicks(3973) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountCarEnable",
                table: "Passageways");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Passageways");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "Passageways");

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

            migrationBuilder.AddColumn<bool>(
                name: "TemporaryVehicleAccess",
                table: "Passageways",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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
                value: new DateTime(2023, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480307L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480308L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065));

            migrationBuilder.UpdateData(
                table: "PlatformCustomers",
                keyColumn: "Id",
                keyValue: 480306,
                columns: new[] { "CreateDate", "ExpireDate" },
                values: new object[] { new DateTime(2023, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065), new DateTime(2024, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065) });

            migrationBuilder.UpdateData(
                table: "SystemRoles",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065), new DateTime(2023, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065), new DateTime(2023, 9, 25, 16, 46, 39, 574, DateTimeKind.Local).AddTicks(8065) });
        }
    }
}
