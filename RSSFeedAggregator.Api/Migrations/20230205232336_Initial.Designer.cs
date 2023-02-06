﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RSSFeedAggregator.Api.Db;

#nullable disable

namespace RSSFeedAggregator.Api.Migrations
{
    [DbContext(typeof(RSSFeedAggregatorDbContext))]
    [Migration("20230205232336_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RSSFeedAggregator.Api.Db.Entities.CategoryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("FeedItemEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FeedItemEntityId");

                    b.ToTable("CategoryEntity");
                });

            modelBuilder.Entity("RSSFeedAggregator.Api.Db.Entities.FeedItemEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FeedItems");
                });

            modelBuilder.Entity("RSSFeedAggregator.Api.Db.Entities.CategoryEntity", b =>
                {
                    b.HasOne("RSSFeedAggregator.Api.Db.Entities.FeedItemEntity", null)
                        .WithMany("Categories")
                        .HasForeignKey("FeedItemEntityId");
                });

            modelBuilder.Entity("RSSFeedAggregator.Api.Db.Entities.FeedItemEntity", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
