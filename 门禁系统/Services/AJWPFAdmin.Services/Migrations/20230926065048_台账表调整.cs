using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJWPFAdmin.Services.Migrations
{
    /// <inheritdoc />
    public partial class 台账表调整 : Migration
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
                table: "SystemConfigDictionaries",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<string>(
                name: "WatchhouseName",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "进厂岗亭名称",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "岗亭名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "WatchhouseId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: false,
                comment: "进厂岗亭id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "岗亭id");

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

            migrationBuilder.AlterColumn<string>(
                name: "PassagewayName",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "进厂通道名称",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "通道名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "PassagewayId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: false,
                comment: "进厂通道id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "通道id");

            migrationBuilder.AlterColumn<long>(
                name: "DeviceId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: false,
                comment: "进厂设备Id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCode",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "进厂设备编号",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "设备编号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ExitDeviceCode",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "出厂设备编号")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "ExitDeviceId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: true,
                comment: "出厂设备Id");

            migrationBuilder.AddColumn<long>(
                name: "ExitPassagewayId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: true,
                comment: "出厂通道id");

            migrationBuilder.AddColumn<string>(
                name: "ExitPassagewayName",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "出厂通道名称")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "ExitWatchhouseId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: true,
                comment: "出厂岗亭id");

            migrationBuilder.AddColumn<string>(
                name: "ExitWatchhouseName",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "出厂岗亭名称")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "WarehouseId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: true,
                comment: "通道关联的仓库id");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "ShippingRecords",
                type: "longtext",
                nullable: true,
                comment: "通道关联的仓库名称")
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

            migrationBuilder.CreateTable(
                name: "WarehousePassageRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CarNo = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, comment: "车牌号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeName = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, comment: "车辆类型名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeId = table.Column<long>(type: "bigint", nullable: false, comment: "车辆类型Id"),
                    WatchhouseId = table.Column<long>(type: "bigint", nullable: false, comment: "岗亭id"),
                    WatchhouseName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "岗亭名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PassagewayId = table.Column<long>(type: "bigint", nullable: false, comment: "通道id"),
                    PassagewayName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "通道名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direction = table.Column<short>(type: "smallint", nullable: false, comment: "进出方向/状态"),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true, comment: "通道关联的仓库id"),
                    WarehouseName = table.Column<string>(type: "longtext", nullable: true, comment: "通道关联的仓库名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SippingRecordId = table.Column<long>(type: "bigint", nullable: false, comment: "关联通行记录id"),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehousePassageRecords", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480306L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480307L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027));

            migrationBuilder.UpdateData(
                table: "CarTypes",
                keyColumn: "Id",
                keyValue: 480308L,
                column: "CreateDate",
                value: new DateTime(2023, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027));

            migrationBuilder.UpdateData(
                table: "PlatformCustomers",
                keyColumn: "Id",
                keyValue: 480306,
                columns: new[] { "CreateDate", "ExpireDate" },
                values: new object[] { new DateTime(2023, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027), new DateTime(2024, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027) });

            migrationBuilder.UpdateData(
                table: "SystemRoles",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027), new DateTime(2023, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 480306L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027), new DateTime(2023, 9, 26, 14, 50, 48, 24, DateTimeKind.Local).AddTicks(9027) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehousePassageRecords");

            migrationBuilder.DropColumn(
                name: "ExitDeviceCode",
                table: "ShippingRecords");

            migrationBuilder.DropColumn(
                name: "ExitDeviceId",
                table: "ShippingRecords");

            migrationBuilder.DropColumn(
                name: "ExitPassagewayId",
                table: "ShippingRecords");

            migrationBuilder.DropColumn(
                name: "ExitPassagewayName",
                table: "ShippingRecords");

            migrationBuilder.DropColumn(
                name: "ExitWatchhouseId",
                table: "ShippingRecords");

            migrationBuilder.DropColumn(
                name: "ExitWatchhouseName",
                table: "ShippingRecords");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "ShippingRecords");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "ShippingRecords");

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

            migrationBuilder.AlterColumn<string>(
                name: "WatchhouseName",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "岗亭名称",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "进厂岗亭名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "WatchhouseId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: false,
                comment: "岗亭id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "进厂岗亭id");

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

            migrationBuilder.AlterColumn<string>(
                name: "PassagewayName",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "通道名称",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "进厂通道名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "PassagewayId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: false,
                comment: "通道id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "进厂通道id");

            migrationBuilder.AlterColumn<long>(
                name: "DeviceId",
                table: "ShippingRecords",
                type: "bigint",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "进厂设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCode",
                table: "ShippingRecords",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "进厂设备编号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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
    }
}
