﻿// <auto-generated />
using System;
using IBISWorldTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IBISWorldTest.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20230531064613_AddDatesToTermTable")]
    partial class AddDatesToTermTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("IBISWorldTest.Models.Term", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Definition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Terms");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Definition = "Practice of writing program",
                            Name = "Coding"
                        },
                        new
                        {
                            Id = 2,
                            Definition = "A great company to work",
                            Name = "Ibis"
                        },
                        new
                        {
                            Id = 3,
                            Definition = "Good way of making web apps",
                            Name = "Web API application"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
