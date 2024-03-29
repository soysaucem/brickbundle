﻿// <auto-generated />
using System;
using BrickBundle.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BrickBundle.Model.Migrations
{
    [DbContext(typeof(BrickBundleContext))]
    [Migration("20190915083648_add_set_theme_inventory")]
    partial class add_set_theme_inventory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BrickBundle.Model.Category", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("BrickBundle.Model.Inventory", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("SetID");

                    b.Property<long>("Version");

                    b.HasKey("ID");

                    b.HasIndex("SetID");

                    b.HasIndex("Version", "SetID")
                        .IsUnique();

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("BrickBundle.Model.InventoryPart", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ColorID");

                    b.Property<long>("InventoryID");

                    b.Property<bool>("IsSpare");

                    b.Property<long>("PartID");

                    b.Property<int>("Quantity");

                    b.HasKey("ID");

                    b.HasIndex("ColorID");

                    b.HasIndex("InventoryID");

                    b.HasIndex("PartID");

                    b.HasIndex("InventoryID", "PartID", "ColorID", "IsSpare")
                        .IsUnique();

                    b.ToTable("InventoryParts");
                });

            modelBuilder.Entity("BrickBundle.Model.InventorySet", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("InventoryID");

                    b.Property<int>("Quantity");

                    b.Property<long>("SetID");

                    b.HasKey("ID");

                    b.HasIndex("InventoryID");

                    b.HasIndex("SetID");

                    b.HasIndex("InventoryID", "SetID")
                        .IsUnique();

                    b.ToTable("InventorySets");
                });

            modelBuilder.Entity("BrickBundle.Model.LegoColor", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsTransparent");

                    b.Property<string>("Name");

                    b.Property<string>("RGB");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("LegoColors");
                });

            modelBuilder.Entity("BrickBundle.Model.Part", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CategoryID");

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.HasIndex("Name");

                    b.ToTable("Parts");
                });

            modelBuilder.Entity("BrickBundle.Model.Set", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("NumParts");

                    b.Property<string>("SetNum");

                    b.Property<long>("ThemeID");

                    b.Property<int>("Year");

                    b.HasKey("ID");

                    b.HasIndex("SetNum")
                        .IsUnique()
                        .HasFilter("[SetNum] IS NOT NULL");

                    b.HasIndex("ThemeID");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("BrickBundle.Model.Theme", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<long?>("ParentID");

                    b.HasKey("ID");

                    b.HasIndex("Name");

                    b.HasIndex("ParentID");

                    b.ToTable("Themes");
                });

            modelBuilder.Entity("BrickBundle.Model.User", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EmailAddress");

                    b.Property<string>("EmailVerificationCode");

                    b.Property<DateTime?>("EmailVerificationCodeCreated");

                    b.Property<bool>("IsEmailVerified");

                    b.Property<byte[]>("Password")
                        .IsRequired();

                    b.Property<int>("ResetPasswordAttempts");

                    b.Property<string>("ResetPasswordCode");

                    b.Property<DateTime?>("ResetPasswordCodeCreated");

                    b.Property<int>("ResetPasswordCodesSent");

                    b.Property<string>("Username");

                    b.HasKey("ID");

                    b.HasIndex("EmailAddress")
                        .IsUnique()
                        .HasFilter("[EmailAddress] IS NOT NULL");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasFilter("[Username] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BrickBundle.Model.UserPart", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ColorID");

                    b.Property<long>("PartID");

                    b.Property<int>("Quantity");

                    b.Property<long>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("ColorID");

                    b.HasIndex("PartID");

                    b.HasIndex("Quantity");

                    b.HasIndex("UserID");

                    b.HasIndex("UserID", "PartID", "ColorID")
                        .IsUnique();

                    b.ToTable("UserParts");
                });

            modelBuilder.Entity("BrickBundle.Model.Inventory", b =>
                {
                    b.HasOne("BrickBundle.Model.Set", "Set")
                        .WithMany("Inventories")
                        .HasForeignKey("SetID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BrickBundle.Model.InventoryPart", b =>
                {
                    b.HasOne("BrickBundle.Model.LegoColor", "Color")
                        .WithMany("InventoryParts")
                        .HasForeignKey("ColorID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BrickBundle.Model.Inventory", "Inventory")
                        .WithMany("InventoryParts")
                        .HasForeignKey("InventoryID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BrickBundle.Model.Part", "Part")
                        .WithMany("InventoryParts")
                        .HasForeignKey("PartID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BrickBundle.Model.InventorySet", b =>
                {
                    b.HasOne("BrickBundle.Model.Inventory", "Inventory")
                        .WithMany("InventorySets")
                        .HasForeignKey("InventoryID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BrickBundle.Model.Set", "Set")
                        .WithMany("InventorySets")
                        .HasForeignKey("SetID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("BrickBundle.Model.Part", b =>
                {
                    b.HasOne("BrickBundle.Model.Category", "Category")
                        .WithMany("Parts")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BrickBundle.Model.Set", b =>
                {
                    b.HasOne("BrickBundle.Model.Theme", "Theme")
                        .WithMany("Sets")
                        .HasForeignKey("ThemeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BrickBundle.Model.Theme", b =>
                {
                    b.HasOne("BrickBundle.Model.Theme", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentID");
                });

            modelBuilder.Entity("BrickBundle.Model.UserPart", b =>
                {
                    b.HasOne("BrickBundle.Model.LegoColor", "Color")
                        .WithMany("UserParts")
                        .HasForeignKey("ColorID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BrickBundle.Model.Part", "Part")
                        .WithMany("UserParts")
                        .HasForeignKey("PartID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BrickBundle.Model.User", "User")
                        .WithMany("UserParts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
