﻿// <auto-generated />
using System;
using ExchangeAdvisor.DB.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ExchangeAdvisor.DB.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("001_InitialMigration")]
    partial class _001_InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ExchangeAdvisor.DB.Entities.ForecastedRateEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Day")
                        .HasColumnType("datetime2");

                    b.Property<long?>("ForecastId")
                        .HasColumnType("bigint");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ForecastId");

                    b.ToTable("ForecastedRate");
                });

            modelBuilder.Entity("ExchangeAdvisor.DB.Entities.HistoricalRateEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Day")
                        .HasColumnType("datetime2");

                    b.Property<long?>("HistoryId")
                        .HasColumnType("bigint");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("HistoryId");

                    b.ToTable("HistoricalRate");
                });

            modelBuilder.Entity("ExchangeAdvisor.DB.Entities.RateForecastEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BaseCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(4)");

                    b.Property<string>("ComparingCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(4)");

                    b.Property<DateTime>("CreationDay")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RateForecast");
                });

            modelBuilder.Entity("ExchangeAdvisor.DB.Entities.RateHistoryEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BaseCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(4)");

                    b.Property<string>("ComparingCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(4)");

                    b.HasKey("Id");

                    b.ToTable("RateHistory");
                });

            modelBuilder.Entity("ExchangeAdvisor.DB.Entities.ForecastedRateEntity", b =>
                {
                    b.HasOne("ExchangeAdvisor.DB.Entities.RateForecastEntity", "Forecast")
                        .WithMany("Rates")
                        .HasForeignKey("ForecastId");
                });

            modelBuilder.Entity("ExchangeAdvisor.DB.Entities.HistoricalRateEntity", b =>
                {
                    b.HasOne("ExchangeAdvisor.DB.Entities.RateHistoryEntity", "History")
                        .WithMany("Rates")
                        .HasForeignKey("HistoryId");
                });
#pragma warning restore 612, 618
        }
    }
}
