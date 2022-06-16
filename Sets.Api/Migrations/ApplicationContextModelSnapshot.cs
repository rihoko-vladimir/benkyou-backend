﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sets.Api.Models.DbContext;

#nullable disable

namespace Sets.Api.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Sets.Api.Models.Entities.Kanji", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("KanjiChar")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("nvarchar(1)");

                    b.Property<Guid>("SetId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SetId");

                    b.ToTable("Kanji");
                });

            modelBuilder.Entity("Sets.Api.Models.Entities.Kunyomi", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("KanjiId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Reading")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.HasIndex("KanjiId");

                    b.ToTable("Kunyomis");
                });

            modelBuilder.Entity("Sets.Api.Models.Entities.Onyomi", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("KanjiId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Reading")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.HasIndex("KanjiId");

                    b.ToTable("Onyomis");
                });

            modelBuilder.Entity("Sets.Api.Models.Entities.Set", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(90)
                        .HasColumnType("nvarchar(90)");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("author_id");

                    b.HasKey("Id");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("Sets.Api.Models.Entities.Kanji", b =>
                {
                    b.HasOne("Sets.Api.Models.Entities.Set", "Set")
                        .WithMany("KanjiList")
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Set");
                });

            modelBuilder.Entity("Sets.Api.Models.Entities.Kunyomi", b =>
                {
                    b.HasOne("Sets.Api.Models.Entities.Kanji", "Kanji")
                        .WithMany("KunyomiReadings")
                        .HasForeignKey("KanjiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kanji");
                });

            modelBuilder.Entity("Sets.Api.Models.Entities.Onyomi", b =>
                {
                    b.HasOne("Sets.Api.Models.Entities.Kanji", "Kanji")
                        .WithMany("OnyomiReadings")
                        .HasForeignKey("KanjiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kanji");
                });

            modelBuilder.Entity("Sets.Api.Models.Entities.Kanji", b =>
                {
                    b.Navigation("KunyomiReadings");

                    b.Navigation("OnyomiReadings");
                });

            modelBuilder.Entity("Sets.Api.Models.Entities.Set", b =>
                {
                    b.Navigation("KanjiList");
                });
#pragma warning restore 612, 618
        }
    }
}
