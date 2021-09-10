﻿// <auto-generated />
using System;
using DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DB.Migrations
{
    [DbContext(typeof(TIAE6Context))]
    [Migration("20210910155947_SeedDataTaxDeclaration")]
    partial class SeedDataTaxDeclaration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Shared.Models.Municipality", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("municipalities");

                    b.HasData(
                        new
                        {
                            id = 1,
                            createdAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            modifiedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            name = "Sitten"
                        });
                });

            modelBuilder.Entity("Shared.Models.Person", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("streetId")
                        .HasColumnType("int");

                    b.Property<string>("streetNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("streetId");

                    b.ToTable("persons");

                    b.HasData(
                        new
                        {
                            id = 1,
                            createdAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            firstName = "André",
                            lastName = "Glatzl",
                            modifiedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            streetId = 1,
                            streetNumber = "1A"
                        },
                        new
                        {
                            id = 2,
                            createdAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            firstName = "Sven",
                            lastName = "Gehring",
                            modifiedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            streetId = 1,
                            streetNumber = "1B"
                        });
                });

            modelBuilder.Entity("Shared.Models.Rule", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("condition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("parentId")
                        .HasColumnType("int");

                    b.Property<string>("rule")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("transformation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("parentId");

                    b.ToTable("Rule");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Rule");
                });

            modelBuilder.Entity("Shared.Models.Street", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("municipalityId")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("postalCode")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("municipalityId");

                    b.ToTable("streets");

                    b.HasData(
                        new
                        {
                            id = 1,
                            createdAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            modifiedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            municipalityId = 1,
                            name = "Bahnhofstrasse",
                            postalCode = 1950
                        });
                });

            modelBuilder.Entity("Shared.Models.TaxDeclaration", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("isSent")
                        .HasColumnType("bit");

                    b.Property<DateTime>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("personId")
                        .HasColumnType("int");

                    b.Property<int>("year")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("personId");

                    b.ToTable("taxDeclarations");
                });

            modelBuilder.Entity("Shared.Models.TaxDeclarationAttribute", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("taxDeclarationAttributes");
                });

            modelBuilder.Entity("Shared.Models.TaxDeclarationEntry", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("createdByRuleId")
                        .HasColumnType("int");

                    b.Property<DateTime>("modifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("taxDeclarationAttributeId")
                        .HasColumnType("int");

                    b.Property<int>("taxDeclarationId")
                        .HasColumnType("int");

                    b.Property<decimal>("value")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("id");

                    b.HasIndex("createdByRuleId");

                    b.HasIndex("taxDeclarationAttributeId");

                    b.HasIndex("taxDeclarationId");

                    b.ToTable("taxDeclarationEntries");
                });

            modelBuilder.Entity("Shared.Models.EvaluationRule", b =>
                {
                    b.HasBaseType("Shared.Models.Rule");

                    b.HasDiscriminator().HasValue("EvaluationRule");
                });

            modelBuilder.Entity("Shared.Models.InferenceRule", b =>
                {
                    b.HasBaseType("Shared.Models.Rule");

                    b.HasDiscriminator().HasValue("InferenceRule");
                });

            modelBuilder.Entity("Shared.Models.Person", b =>
                {
                    b.HasOne("Shared.Models.Street", "street")
                        .WithMany()
                        .HasForeignKey("streetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("street");
                });

            modelBuilder.Entity("Shared.Models.Rule", b =>
                {
                    b.HasOne("Shared.Models.Rule", "parent")
                        .WithMany()
                        .HasForeignKey("parentId");

                    b.Navigation("parent");
                });

            modelBuilder.Entity("Shared.Models.Street", b =>
                {
                    b.HasOne("Shared.Models.Municipality", "municipality")
                        .WithMany()
                        .HasForeignKey("municipalityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("municipality");
                });

            modelBuilder.Entity("Shared.Models.TaxDeclaration", b =>
                {
                    b.HasOne("Shared.Models.Person", "person")
                        .WithMany()
                        .HasForeignKey("personId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("person");
                });

            modelBuilder.Entity("Shared.Models.TaxDeclarationEntry", b =>
                {
                    b.HasOne("Shared.Models.Rule", "createdByRule")
                        .WithMany()
                        .HasForeignKey("createdByRuleId");

                    b.HasOne("Shared.Models.TaxDeclarationAttribute", "attribute")
                        .WithMany()
                        .HasForeignKey("taxDeclarationAttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.TaxDeclaration", "taxDeclaration")
                        .WithMany()
                        .HasForeignKey("taxDeclarationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("attribute");

                    b.Navigation("createdByRule");

                    b.Navigation("taxDeclaration");
                });
#pragma warning restore 612, 618
        }
    }
}
