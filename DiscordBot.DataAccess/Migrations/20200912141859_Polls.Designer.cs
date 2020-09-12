﻿// <auto-generated />
using System;
using DiscordBot.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBot.DataAccess.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20200912141859_Polls")]
    partial class Polls
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-preview.8.20407.4");

            modelBuilder.Entity("DiscordBot.DataAccess.Entities.Poll", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "MessageId" }, "Idx_Poll_MessageId")
                        .IsUnique();

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Entities.Pollvote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PollId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("VoteOption")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PollId");

                    b.HasIndex(new[] { "UserId" }, "Idx_Pollvote_UserId");

                    b.ToTable("Pollvotes");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Entities.Pollvote", b =>
                {
                    b.HasOne("DiscordBot.DataAccess.Entities.Poll", "Poll")
                        .WithMany()
                        .HasForeignKey("PollId");
                });
#pragma warning restore 612, 618
        }
    }
}
