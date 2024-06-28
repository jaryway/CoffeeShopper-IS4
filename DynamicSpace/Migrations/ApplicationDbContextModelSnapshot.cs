﻿// <auto-generated />
using System;
using DynamicSpace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DynamicSpace.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("DynamicSpace.Models.DynamicEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EntityProperties")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("JSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<bool>("Published")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TenantId")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.HasKey("Id");

                    b.ToTable("DynamicEntities");
                });

            modelBuilder.Entity("DynamicSpace.Models.MigrationEntry", b =>
                {
                    b.Property<string>("MigrationId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("TenantId")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.HasKey("MigrationId");

                    b.ToTable("MigrationEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
