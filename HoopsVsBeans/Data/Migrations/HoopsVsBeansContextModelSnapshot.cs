﻿// <auto-generated />
using System;
using HoopsVsBeans.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HoopsVsBeans.Data.Migrations
{
    [DbContext(typeof(HoopsVsBeansContext))]
    partial class HoopsVsBeansContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("HoopsVsBeans.Data.Models.Vote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("OptionVoted")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("VoteTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("HoopsVsBeans.Data.Models.VoteOptions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Beans")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Hoops")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("VoteOptions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Beans = 0,
                            Hoops = 0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
