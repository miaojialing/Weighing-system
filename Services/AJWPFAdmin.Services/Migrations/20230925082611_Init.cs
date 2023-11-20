using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AJWPFAdmin.Services.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CarNo = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, comment: "车牌号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeId = table.Column<long>(type: "bigint", nullable: false, comment: "车辆类型Id"),
                    TypeName = table.Column<string>(type: "longtext", nullable: false, comment: "车辆类型名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpireDate = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "有效期"),
                    PaiFangJieDuan = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "排放阶段")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EngineNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "发动机号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VIN = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "VIN")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegDate = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "注册日期"),
                    TeamName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "车队名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "货物名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CarNetWeight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "重量(KG)"),
                    VehicleLicense = table.Column<string>(type: "longtext", nullable: true, comment: "行驶证图片路径json数组字符串")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Attachments = table.Column<string>(type: "longtext", nullable: true, comment: "随车清单路径json数组字符串")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                },
                comment: "车辆记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CarTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, comment: "类型名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SysRequired = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否系统必须"),
                    AutoPass = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否自动开闸"),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarTypes", x => x.Id);
                },
                comment: "车辆类型记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DatabaseBackupRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "备份文件名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilePath = table.Column<string>(type: "longtext", nullable: false, comment: "备份文件所在路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSize = table.Column<long>(type: "bigint", nullable: false, comment: "文件大小"),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseBackupRecords", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MaterialDictionaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortNo = table.Column<int>(type: "int", nullable: false, comment: "序号"),
                    Type = table.Column<short>(type: "smallint", nullable: false, comment: "物料字典类型"),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDictionaries", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlatformCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Director = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DirectorPhone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contact = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Logo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Enable = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformCustomers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShippingRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CarNo = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, comment: "车牌号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialCategory = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "物料种类")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "货物名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArriveDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "到港时间"),
                    ShipStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "装/卸货时间"),
                    ShipEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "结束时间"),
                    OrderNetWeight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "运单净重"),
                    CarNetWeight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "汽车衡净重"),
                    SenderName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "发货单位")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReceiverName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "收货单位")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Berth = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "装/卸货地点泊位")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WatchhouseId = table.Column<long>(type: "bigint", nullable: false, comment: "岗亭id"),
                    WatchhouseName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "岗亭名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PassagewayId = table.Column<long>(type: "bigint", nullable: false, comment: "通道id"),
                    PassagewayName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "通道名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false, comment: "设备Id"),
                    DeviceCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "设备编号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntranceIdentifiedCaptureFile = table.Column<string>(type: "longtext", nullable: true, comment: "入口车牌识别抓拍图文件路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntranceCameraCaptureFile = table.Column<string>(type: "longtext", nullable: true, comment: "入口监控抓拍图文件路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExitIdentifiedCaptureFile = table.Column<string>(type: "longtext", nullable: true, comment: "出口车牌识别抓拍图文件路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExitCameraCaptureFile = table.Column<string>(type: "longtext", nullable: true, comment: "出口监控抓拍图文件路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direction = table.Column<short>(type: "smallint", nullable: false, comment: "进出方向/状态"),
                    PaiFangJieDuan = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "排放阶段")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EngineNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "发动机号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VIN = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "VIN")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegDate = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "注册日期"),
                    TeamName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "车队名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeName = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, comment: "车辆类型名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeId = table.Column<long>(type: "bigint", nullable: false, comment: "车辆类型Id"),
                    AutoPass = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否是自动开闸"),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRecords", x => x.Id);
                },
                comment: "运输台账记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemConfigDictionaries",
                columns: table => new
                {
                    Key = table.Column<short>(type: "smallint", nullable: false, comment: "配置Key"),
                    IntValue = table.Column<int>(type: "int", nullable: false, comment: "整数配置值"),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, comment: "数值配置值"),
                    StringValue = table.Column<string>(type: "longtext", nullable: true, comment: "字符串配置值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigDictionaries", x => x.Key);
                },
                comment: "系统配置字典记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    PId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Permission = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Watchhouses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IP = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "IP地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Remark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watchhouses", x => x.Id);
                },
                comment: "岗亭表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    PId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NickName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Avatar = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Enable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    RoleName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUsers_SystemRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SystemRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Passageways",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    WatchhouseId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Remark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direction = table.Column<short>(type: "smallint", nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "编号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceGateway = table.Column<string>(type: "longtext", nullable: true, comment: "设备网关")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TemporaryVehicleAccess = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passageways", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passageways_Watchhouses_WatchhouseId",
                        column: x => x.WatchhouseId,
                        principalTable: "Watchhouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "通道表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    WatchhouseId = table.Column<long>(type: "bigint", nullable: false),
                    PassagewayId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "编号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IP = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "设备IP地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SerialPort = table.Column<int>(type: "int", maxLength: 10, nullable: false, comment: "设备串口号"),
                    Port = table.Column<int>(type: "int", nullable: false, comment: "设备端口号"),
                    LoginName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "登录账户名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginPassword = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "登录密码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OnlyMonitor = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否仅作为监控"),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersionTimestamp = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Passageways_PassagewayId",
                        column: x => x.PassagewayId,
                        principalTable: "Passageways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "设备表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "CarTypes",
                columns: new[] { "Id", "AutoPass", "CreateDate", "Name", "PId", "SysRequired" },
                values: new object[,]
                {
                    { 480306L, true, new DateTime(2023, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850), "固定车", 480306, true },
                    { 480307L, false, new DateTime(2023, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850), "临时车", 480306, true },
                    { 480308L, true, new DateTime(2023, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850), "台账车", 480306, true }
                });

            migrationBuilder.InsertData(
                table: "PlatformCustomers",
                columns: new[] { "Id", "Address", "Contact", "CreateDate", "Director", "DirectorPhone", "Enable", "ExpireDate", "Logo", "Name" },
                values: new object[] { 480306, "", "15155555551", new DateTime(2023, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850), "阿吉", "15155555551", true, new DateTime(2024, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850), null, "阿吉" });

            migrationBuilder.InsertData(
                table: "SystemRoles",
                columns: new[] { "Id", "CreateDate", "Description", "Name", "PId", "Permission", "UpdateDate" },
                values: new object[] { 480306L, new DateTime(2023, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850), null, "管理员", 480306, "all", new DateTime(2023, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850) });

            migrationBuilder.InsertData(
                table: "SystemUsers",
                columns: new[] { "Id", "AccountName", "Avatar", "CreateDate", "Enable", "NickName", "PId", "Password", "Phone", "RoleId", "RoleName", "UpdateDate" },
                values: new object[] { 480306L, "admin", "", new DateTime(2023, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850), true, "admin", 480306, "XaLn68yZce8SSDfe6S41Ew==", "15155555551", 480306L, "管理员", new DateTime(2023, 9, 25, 16, 26, 11, 832, DateTimeKind.Local).AddTicks(850) });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_PassagewayId",
                table: "Devices",
                column: "PassagewayId");

            migrationBuilder.CreateIndex(
                name: "IX_Passageways_WatchhouseId",
                table: "Passageways",
                column: "WatchhouseId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsers_RoleId",
                table: "SystemUsers",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "CarTypes");

            migrationBuilder.DropTable(
                name: "DatabaseBackupRecords");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "MaterialDictionaries");

            migrationBuilder.DropTable(
                name: "PlatformCustomers");

            migrationBuilder.DropTable(
                name: "ShippingRecords");

            migrationBuilder.DropTable(
                name: "SystemConfigDictionaries");

            migrationBuilder.DropTable(
                name: "SystemUsers");

            migrationBuilder.DropTable(
                name: "Passageways");

            migrationBuilder.DropTable(
                name: "SystemRoles");

            migrationBuilder.DropTable(
                name: "Watchhouses");
        }
    }
}
