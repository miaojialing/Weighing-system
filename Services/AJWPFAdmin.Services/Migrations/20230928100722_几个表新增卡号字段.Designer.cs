﻿// <auto-generated />
using System;
using AJWPFAdmin.Services.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AJWPFAdmin.Services.Migrations
{
    [DbContext(typeof(DbService))]
    [Migration("20230928100722_几个表新增卡号字段")]
    partial class 几个表新增卡号字段
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Car", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("Attachments")
                        .HasColumnType("longtext")
                        .HasComment("随车清单路径json数组字符串");

                    b.Property<decimal>("CarNetWeight")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)")
                        .HasComment("重量(KG)");

                    b.Property<string>("CarNo")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasComment("车牌号");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("EngineNo")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("发动机号");

                    b.Property<DateTime?>("ExpireDate")
                        .HasColumnType("datetime(6)")
                        .HasComment("有效期");

                    b.Property<string>("IDCardNo")
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasComment("卡号");

                    b.Property<string>("MaterialName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("货物名称");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<string>("PaiFangJieDuan")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("排放阶段");

                    b.Property<DateTime?>("RegDate")
                        .HasColumnType("datetime(6)")
                        .HasComment("注册日期");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<string>("TeamName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("车队名称");

                    b.Property<long>("TypeId")
                        .HasColumnType("bigint")
                        .HasComment("车辆类型Id");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("车辆类型名称");

                    b.Property<string>("VIN")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("VIN");

                    b.Property<string>("VehicleLicense")
                        .HasColumnType("longtext")
                        .HasComment("行驶证图片路径json数组字符串");

                    b.HasKey("Id");

                    b.ToTable("Cars", t =>
                        {
                            t.HasComment("车辆记录表");
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.CarType", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool>("AutoPass")
                        .HasColumnType("tinyint(1)")
                        .HasComment("是否自动开闸");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("EnablePassagewayStatistic")
                        .HasColumnType("tinyint(1)")
                        .HasComment("是否参与车辆统计");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasComment("类型名称");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<bool>("SysRequired")
                        .HasColumnType("tinyint(1)")
                        .HasComment("是否系统必须");

                    b.HasKey("Id");

                    b.ToTable("CarTypes", t =>
                        {
                            t.HasComment("车辆类型记录表");
                        });

                    b.HasData(
                        new
                        {
                            Id = 480306L,
                            AutoPass = true,
                            CreateDate = new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900),
                            EnablePassagewayStatistic = false,
                            Name = "固定车",
                            PId = 480306,
                            SysRequired = true
                        },
                        new
                        {
                            Id = 480307L,
                            AutoPass = false,
                            CreateDate = new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900),
                            EnablePassagewayStatistic = false,
                            Name = "临时车",
                            PId = 480306,
                            SysRequired = true
                        },
                        new
                        {
                            Id = 480308L,
                            AutoPass = true,
                            CreateDate = new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900),
                            EnablePassagewayStatistic = false,
                            Name = "台账车",
                            PId = 480306,
                            SysRequired = true
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.DatabaseBackupRecord", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasComment("备份文件名");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("备份文件所在路径");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint")
                        .HasComment("文件大小");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.HasKey("Id");

                    b.ToTable("DatabaseBackupRecords");
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Device", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("Code")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("编号");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("IP")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("设备IP地址");

                    b.Property<string>("LoginName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("登录账户名");

                    b.Property<string>("LoginPassword")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasComment("登录密码");

                    b.Property<bool>("OnlyMonitor")
                        .HasColumnType("tinyint(1)")
                        .HasComment("是否仅作为监控");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<long>("PassagewayId")
                        .HasColumnType("bigint");

                    b.Property<int>("Port")
                        .HasColumnType("int")
                        .HasComment("设备端口号");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<int>("SerialPort")
                        .HasMaxLength(10)
                        .HasColumnType("int")
                        .HasComment("设备串口号");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.Property<long>("WatchhouseId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PassagewayId");

                    b.ToTable("Devices", t =>
                        {
                            t.HasComment("设备表");
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.MaterialDictionary", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("名称");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<int>("SortNo")
                        .HasColumnType("int")
                        .HasComment("序号");

                    b.Property<short>("Type")
                        .HasColumnType("smallint")
                        .HasComment("物料字典类型");

                    b.HasKey("Id");

                    b.ToTable("MaterialDictionaries");
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Passageway", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("Code")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("编号");

                    b.Property<bool>("CountCarEnable")
                        .HasColumnType("tinyint(1)")
                        .HasComment("是否统计车辆");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DeviceGateway")
                        .HasColumnType("longtext")
                        .HasComment("设备网关");

                    b.Property<short>("Direction")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("名称");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasComment("备注");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<long?>("WarehouseId")
                        .HasColumnType("bigint")
                        .HasComment("关联仓库id");

                    b.Property<string>("WarehouseName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("关联仓库名称");

                    b.Property<long>("WatchhouseId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("WatchhouseId");

                    b.ToTable("Passageways", t =>
                        {
                            t.HasComment("通道表");
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.PlatformCustomer", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Contact")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Director")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("DirectorPhone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<bool>("Enable")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("ExpireDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Logo")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("PlatformCustomers");

                    b.HasData(
                        new
                        {
                            Id = 480306,
                            Address = "",
                            Contact = "15155555551",
                            CreateDate = new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900),
                            Director = "阿吉",
                            DirectorPhone = "15155555551",
                            Enable = true,
                            ExpireDate = new DateTime(2024, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900),
                            Name = "阿吉"
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.ShippingRecord", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ArriveDate")
                        .HasColumnType("datetime(6)")
                        .HasComment("到港时间");

                    b.Property<bool>("AutoPass")
                        .HasColumnType("tinyint(1)")
                        .HasComment("是否是自动开闸");

                    b.Property<string>("Berth")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("装/卸货地点泊位");

                    b.Property<decimal>("CarNetWeight")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)")
                        .HasComment("汽车衡净重");

                    b.Property<string>("CarNo")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasComment("车牌号");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DeviceCode")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("进厂设备编号");

                    b.Property<long>("DeviceId")
                        .HasColumnType("bigint")
                        .HasComment("进厂设备Id");

                    b.Property<short>("Direction")
                        .HasColumnType("smallint")
                        .HasComment("进出方向/状态");

                    b.Property<string>("EngineNo")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("发动机号");

                    b.Property<string>("EntranceCameraCaptureFile")
                        .HasColumnType("longtext")
                        .HasComment("入口监控抓拍图文件路径");

                    b.Property<string>("EntranceIdentifiedCaptureFile")
                        .HasColumnType("longtext")
                        .HasComment("入口车牌识别抓拍图文件路径");

                    b.Property<string>("ExitCameraCaptureFile")
                        .HasColumnType("longtext")
                        .HasComment("出口监控抓拍图文件路径");

                    b.Property<string>("ExitDeviceCode")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("出厂设备编号");

                    b.Property<long?>("ExitDeviceId")
                        .HasColumnType("bigint")
                        .HasComment("出厂设备Id");

                    b.Property<string>("ExitIdentifiedCaptureFile")
                        .HasColumnType("longtext")
                        .HasComment("出口车牌识别抓拍图文件路径");

                    b.Property<long?>("ExitPassagewayId")
                        .HasColumnType("bigint")
                        .HasComment("出厂通道id");

                    b.Property<string>("ExitPassagewayName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("出厂通道名称");

                    b.Property<long?>("ExitWatchhouseId")
                        .HasColumnType("bigint")
                        .HasComment("出厂岗亭id");

                    b.Property<string>("ExitWatchhouseName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("出厂岗亭名称");

                    b.Property<string>("IDCardNo")
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasComment("卡号");

                    b.Property<string>("MaterialCategory")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("物料种类");

                    b.Property<string>("MaterialName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("货物名称");

                    b.Property<decimal>("OrderNetWeight")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)")
                        .HasComment("运单净重");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<string>("PaiFangJieDuan")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("排放阶段");

                    b.Property<long>("PassagewayId")
                        .HasColumnType("bigint")
                        .HasComment("进厂通道id");

                    b.Property<string>("PassagewayName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("进厂通道名称");

                    b.Property<string>("ReceiverName")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasComment("收货单位");

                    b.Property<DateTime?>("RegDate")
                        .HasColumnType("datetime(6)")
                        .HasComment("注册日期");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<string>("SenderName")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasComment("发货单位");

                    b.Property<DateTime?>("ShipEndDate")
                        .HasColumnType("datetime(6)")
                        .HasComment("结束时间");

                    b.Property<DateTime?>("ShipStartDate")
                        .HasColumnType("datetime(6)")
                        .HasComment("装/卸货时间");

                    b.Property<string>("TeamName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("车队名称");

                    b.Property<long>("TypeId")
                        .HasColumnType("bigint")
                        .HasComment("车辆类型Id");

                    b.Property<string>("TypeName")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasComment("车辆类型名称");

                    b.Property<string>("VIN")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("VIN");

                    b.Property<long?>("WarehouseId")
                        .HasColumnType("bigint")
                        .HasComment("通道关联的仓库id");

                    b.Property<string>("WarehouseName")
                        .HasColumnType("longtext")
                        .HasComment("通道关联的仓库名称");

                    b.Property<long>("WatchhouseId")
                        .HasColumnType("bigint")
                        .HasComment("进厂岗亭id");

                    b.Property<string>("WatchhouseName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("进厂岗亭名称");

                    b.HasKey("Id");

                    b.ToTable("ShippingRecords", t =>
                        {
                            t.HasComment("运输台账记录表");
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.SystemConfigDictionary", b =>
                {
                    b.Property<short>("Key")
                        .HasColumnType("smallint")
                        .HasComment("配置Key");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("DecimalValue")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)")
                        .HasComment("数值配置值");

                    b.Property<int>("IntValue")
                        .HasColumnType("int")
                        .HasComment("整数配置值");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<string>("StringValue")
                        .HasColumnType("longtext")
                        .HasComment("字符串配置值");

                    b.HasKey("Key");

                    b.ToTable("SystemConfigDictionaries", t =>
                        {
                            t.HasComment("系统配置字典记录表");
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.SystemRole", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<string>("Permission")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("SystemRoles");

                    b.HasData(
                        new
                        {
                            Id = 480306L,
                            CreateDate = new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900),
                            Name = "管理员",
                            PId = 480306,
                            Permission = "all",
                            UpdateDate = new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900)
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.SystemUser", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Avatar")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Enable")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NickName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<string>("RoleName")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("SystemUsers");

                    b.HasData(
                        new
                        {
                            Id = 480306L,
                            AccountName = "admin",
                            Avatar = "",
                            CreateDate = new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900),
                            Enable = true,
                            NickName = "admin",
                            PId = 480306,
                            Password = "XaLn68yZce8SSDfe6S41Ew==",
                            Phone = "15155555551",
                            RoleId = 480306L,
                            RoleName = "管理员",
                            UpdateDate = new DateTime(2023, 9, 28, 18, 7, 21, 922, DateTimeKind.Local).AddTicks(5900)
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Warehouse", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int>("CarLimit")
                        .HasColumnType("int")
                        .HasComment("车次");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("名称");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<int>("ResidualCarLimit")
                        .HasColumnType("int")
                        .HasComment("剩余车次");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.HasKey("Id");

                    b.ToTable("Warehouses", t =>
                        {
                            t.HasComment("仓库表");
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.WarehousePassageRecord", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("CarNo")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasComment("车牌号");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<short>("Direction")
                        .HasColumnType("smallint")
                        .HasComment("进出方向/状态");

                    b.Property<string>("IDCardNo")
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasComment("车牌号");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<long>("PassagewayId")
                        .HasColumnType("bigint")
                        .HasComment("通道id");

                    b.Property<string>("PassagewayName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("通道名称");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<long>("SippingRecordId")
                        .HasColumnType("bigint")
                        .HasComment("关联通行记录id");

                    b.Property<long>("TypeId")
                        .HasColumnType("bigint")
                        .HasComment("车辆类型Id");

                    b.Property<string>("TypeName")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasComment("车辆类型名称");

                    b.Property<long?>("WarehouseId")
                        .HasColumnType("bigint")
                        .HasComment("通道关联的仓库id");

                    b.Property<string>("WarehouseName")
                        .HasColumnType("longtext")
                        .HasComment("通道关联的仓库名称");

                    b.Property<long>("WatchhouseId")
                        .HasColumnType("bigint")
                        .HasComment("岗亭id");

                    b.Property<string>("WatchhouseName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("岗亭名称");

                    b.HasKey("Id");

                    b.ToTable("WarehousePassageRecords");
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Watchhouse", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("IP")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("IP地址");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("名称");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasComment("备注");

                    b.Property<DateTime?>("RowVersionTimestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.HasKey("Id");

                    b.ToTable("Watchhouses", t =>
                        {
                            t.HasComment("岗亭表");
                        });
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Device", b =>
                {
                    b.HasOne("AJWPFAdmin.Services.EF.Tables.Passageway", "Passageway")
                        .WithMany("Devices")
                        .HasForeignKey("PassagewayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Passageway");
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Passageway", b =>
                {
                    b.HasOne("AJWPFAdmin.Services.EF.Tables.Watchhouse", "Watchhouse")
                        .WithMany("Passageways")
                        .HasForeignKey("WatchhouseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Watchhouse");
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.SystemUser", b =>
                {
                    b.HasOne("AJWPFAdmin.Services.EF.Tables.SystemRole", "Role")
                        .WithMany("SystemUsers")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Passageway", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.SystemRole", b =>
                {
                    b.Navigation("SystemUsers");
                });

            modelBuilder.Entity("AJWPFAdmin.Services.EF.Tables.Watchhouse", b =>
                {
                    b.Navigation("Passageways");
                });
#pragma warning restore 612, 618
        }
    }
}
