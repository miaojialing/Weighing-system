using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJWPFAdmin.Services.Migrations
{
    /// <inheritdoc />
    public partial class 统计端记录表调整 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SippingRecordId",
                table: "WarehousePassageRecords");

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

            migrationBuilder.AlterColumn<string>(
                name: "IDCardNo",
                table: "WarehousePassageRecords",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true,
                comment: "卡号",
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldMaxLength: 120,
                oldNullable: true,
                oldComment: "车牌号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CameraCaptureFile",
                table: "WarehousePassageRecords",
                type: "longtext",
                nullable: true,
                comment: "监控抓拍图文件路径")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DeviceCode",
                table: "WarehousePassageRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "进厂设备编号")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "DeviceId",
                table: "WarehousePassageRecords",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "进厂设备Id");

            migrationBuilder.AddColumn<string>(
                name: "ExitDeviceCode",
                table: "WarehousePassageRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "出厂设备编号")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "ExitDeviceId",
                table: "WarehousePassageRecords",
                type: "bigint",
                nullable: true,
                comment: "出厂设备Id");

            migrationBuilder.AddColumn<long>(
                name: "ExitPassagewayId",
                table: "WarehousePassageRecords",
                type: "bigint",
                nullable: true,
                comment: "出厂通道id");

            migrationBuilder.AddColumn<string>(
                name: "ExitPassagewayName",
                table: "WarehousePassageRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "出厂通道名称")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "ExitWatchhouseId",
                table: "WarehousePassageRecords",
                type: "bigint",
                nullable: true,
                comment: "出厂岗亭id");

            migrationBuilder.AddColumn<string>(
                name: "ExitWatchhouseName",
                table: "WarehousePassageRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "出厂岗亭名称")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IdentifiedCaptureFullFile",
                table: "WarehousePassageRecords",
                type: "longtext",
                nullable: true,
                comment: "车牌识别抓拍全图文件路径")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IdentifiedCaptureSmallFile",
                table: "WarehousePassageRecords",
                type: "longtext",
                nullable: true,
                comment: "车牌识别抓拍图文件路径")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ShipEndDate",
                table: "WarehousePassageRecords",
                type: "datetime(6)",
                nullable: true,
                comment: "出厂时间 ");

            migrationBuilder.AddColumn<DateTime>(
                name: "ShipStartDate",
                table: "WarehousePassageRecords",
                type: "datetime(6)",
                nullable: true,
                comment: "进厂时间");

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
                name: "ShipStartDate",
                table: "ShippingRecords",
                type: "datetime(6)",
                nullable: true,
                comment: "进厂时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "装/卸货时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShipEndDate",
                table: "ShippingRecords",
                type: "datetime(6)",
                nullable: true,
                comment: "出厂时间 ",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "结束时间");

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
                value: new DateTime(2024, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480307L,
                column: "CreateDate",
                value: new DateTime(2024, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480308L,
                column: "CreateDate",
                value: new DateTime(2024, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675));

            migrationBuilder.UpdateData(
                table: "PlatformCustomers",
                keyColumn: "Id",
                keyValue: 480306,
                columns: new[] { "CreateDate", "ExpireDate" },
                values: new object[] { new DateTime(2024, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675), new DateTime(2025, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675) });

            migrationBuilder.UpdateData(
                table: "SystemRoles",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2024, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675), new DateTime(2024, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2024, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675), new DateTime(2024, 4, 2, 19, 18, 46, 634, DateTimeKind.Local).AddTicks(675) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CameraCaptureFile",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "DeviceCode",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ExitDeviceCode",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ExitDeviceId",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ExitPassagewayId",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ExitPassagewayName",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ExitWatchhouseId",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ExitWatchhouseName",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "IdentifiedCaptureFullFile",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "IdentifiedCaptureSmallFile",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ShipEndDate",
                table: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ShipStartDate",
                table: "WarehousePassageRecords");

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

            migrationBuilder.AlterColumn<string>(
                name: "IDCardNo",
                table: "WarehousePassageRecords",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true,
                comment: "车牌号",
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldMaxLength: 120,
                oldNullable: true,
                oldComment: "卡号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "SippingRecordId",
                table: "WarehousePassageRecords",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "关联通行记录id");

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
                name: "ShipStartDate",
                table: "ShippingRecords",
                type: "datetime(6)",
                nullable: true,
                comment: "装/卸货时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "进厂时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShipEndDate",
                table: "ShippingRecords",
                type: "datetime(6)",
                nullable: true,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "出厂时间 ");

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
    }
}
