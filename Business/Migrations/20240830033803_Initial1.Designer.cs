﻿// <auto-generated />
using System;
using Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Business.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240830033803_Initial1")]
    partial class Initial1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AccountDetail", b =>
                {
                    b.Property<int>("IdAccountDt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdAccountDt"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Address")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<int>("CCCD")
                        .HasColumnType("int");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedWhen")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("IdAccount")
                        .HasColumnType("int");

                    b.Property<int?>("LastUpdateBy")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdateWhen")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nationality")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("IdAccountDt");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("IdAccount")
                        .IsUnique();

                    b.HasIndex("LastUpdateBy");

                    b.ToTable("AccountDetails");

                    b.HasData(
                        new
                        {
                            IdAccountDt = 1,
                            Active = true,
                            CCCD = 123456789,
                            CreatedBy = 1,
                            CreatedWhen = new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3441),
                            Description = "Admin account",
                            Fullname = "Nguyễn Ngọc Quang",
                            Gender = "Nam",
                            IdAccount = 1,
                            LastUpdateBy = 1,
                            LastUpdateWhen = new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3441),
                            Nationality = "Vietnam"
                        },
                        new
                        {
                            IdAccountDt = 2,
                            Active = true,
                            CCCD = 987654321,
                            CreatedBy = 2,
                            CreatedWhen = new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3445),
                            Description = "User account",
                            Fullname = "Minh Khang",
                            Gender = "Nam",
                            IdAccount = 2,
                            LastUpdateBy = 1,
                            LastUpdateWhen = new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3446),
                            Nationality = "Vietnam"
                        });
                });

            modelBuilder.Entity("Business.Account", b =>
                {
                    b.Property<int>("IdAccount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdAccount"));

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedWhen")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("IdRole")
                        .HasColumnType("int");

                    b.Property<int?>("LastUpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdateWhen")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Phone")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IdAccount");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("IdRole");

                    b.HasIndex("LastUpdateBy");

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            IdAccount = 1,
                            CreatedWhen = new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3411),
                            Email = "Quang111420@gmail.com",
                            IdRole = 1,
                            LastUpdateWhen = new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3412),
                            Password = "quang111420"
                        },
                        new
                        {
                            IdAccount = 2,
                            CreatedWhen = new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3415),
                            Email = "khang2007@gmail.com",
                            IdRole = 2,
                            LastUpdateWhen = new DateTime(2024, 8, 30, 10, 38, 3, 668, DateTimeKind.Local).AddTicks(3415),
                            Password = "khang2007"
                        });
                });

            modelBuilder.Entity("Business.Artwork", b =>
                {
                    b.Property<int>("IdArtwork")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdArtwork"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedWhen")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdAc")
                        .HasColumnType("int");

                    b.Property<int>("IdTypeOfArtwork")
                        .HasColumnType("int");

                    b.Property<int?>("LastUpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdateWhen")
                        .HasColumnType("datetime2");

                    b.Property<string>("MediaType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MediaUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("Watched")
                        .HasColumnType("int");

                    b.HasKey("IdArtwork");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("IdAc");

                    b.HasIndex("IdTypeOfArtwork");

                    b.HasIndex("LastUpdateBy");

                    b.ToTable("Artworks");
                });

            modelBuilder.Entity("Business.Comment", b =>
                {
                    b.Property<int>("IdComment")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdComment"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdAc")
                        .HasColumnType("int");

                    b.Property<int>("IdArtwork")
                        .HasColumnType("int");

                    b.Property<int?>("IdPrevComment")
                        .HasColumnType("int");

                    b.Property<string>("Reaction")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdComment");

                    b.HasIndex("IdAc");

                    b.HasIndex("IdArtwork");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Business.DocumentInfo", b =>
                {
                    b.Property<int>("IdDcIf")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDcIf"));

                    b.Property<int?>("AccountIdAccount")
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("Created_by")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Created_when")
                        .HasColumnType("datetime");

                    b.Property<int?>("IdAcDt")
                        .HasColumnType("int");

                    b.Property<int?>("IdArtwork")
                        .HasColumnType("int");

                    b.Property<int?>("IdEvent")
                        .HasColumnType("int");

                    b.Property<int?>("IdProject")
                        .HasColumnType("int");

                    b.Property<int?>("Last_update_by")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Last_update_when")
                        .HasColumnType("datetime");

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeFile")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlDocument")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdDcIf");

                    b.HasIndex("AccountIdAccount");

                    b.HasIndex("Created_by");

                    b.HasIndex("IdAcDt");

                    b.HasIndex("IdArtwork");

                    b.HasIndex("IdEvent");

                    b.HasIndex("IdProject");

                    b.HasIndex("Last_update_by");

                    b.ToTable("DocumentInfos");
                });

            modelBuilder.Entity("Business.Event", b =>
                {
                    b.Property<int>("IdEvent")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdEvent"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedWhen")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdAc")
                        .HasColumnType("int");

                    b.Property<int?>("LastUpdateBy")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdateWhen")
                        .HasColumnType("datetime2");

                    b.Property<int>("NumberOfPeople")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("IdEvent");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("IdAc");

                    b.HasIndex("LastUpdateBy");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Business.EventParticipants", b =>
                {
                    b.Property<int>("IdEventParticipant")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdEventParticipant"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int>("IdAc")
                        .HasColumnType("int");

                    b.Property<int>("IdEvent")
                        .HasColumnType("int");

                    b.Property<DateTime>("RegistrationTime")
                        .HasColumnType("datetime2");

                    b.HasKey("IdEventParticipant");

                    b.HasIndex("IdAc");

                    b.HasIndex("IdEvent");

                    b.ToTable("EventParticipants");
                });

            modelBuilder.Entity("Business.Follow", b =>
                {
                    b.Property<int>("IdFollow")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdFollow"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CreatedWhen")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdFollower")
                        .HasColumnType("int");

                    b.Property<int>("IdFollowing")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdateWhen")
                        .HasColumnType("datetime2");

                    b.HasKey("IdFollow");

                    b.HasIndex("IdFollower");

                    b.HasIndex("IdFollowing");

                    b.ToTable("Follows");
                });

            modelBuilder.Entity("Business.ProjectParticipant", b =>
                {
                    b.Property<int>("IdProjectParticipant")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdProjectParticipant"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int>("IdAc")
                        .HasColumnType("int");

                    b.Property<int>("IdProject")
                        .HasColumnType("int");

                    b.HasKey("IdProjectParticipant");

                    b.HasIndex("IdAc");

                    b.HasIndex("IdProject");

                    b.ToTable("ProjectParticipants");
                });

            modelBuilder.Entity("Business.Reaction", b =>
                {
                    b.Property<int>("IdReaction")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdReaction"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdAc")
                        .HasColumnType("int");

                    b.Property<int>("IdArtwork")
                        .HasColumnType("int");

                    b.HasKey("IdReaction");

                    b.HasIndex("IdAc");

                    b.HasIndex("IdArtwork");

                    b.ToTable("Reactions");
                });

            modelBuilder.Entity("Business.Role", b =>
                {
                    b.Property<int>("IdRole")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdRole"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("IdRole");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            IdRole = 1,
                            Active = false,
                            RoleName = "Admin"
                        },
                        new
                        {
                            IdRole = 2,
                            Active = false,
                            RoleName = "User"
                        });
                });

            modelBuilder.Entity("Business.TypeOfArtwork", b =>
                {
                    b.Property<int>("IdTypeOfArtwork")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdTypeOfArtwork"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameTypeOfArtwork")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdTypeOfArtwork");

                    b.ToTable("TypeOfArtworks");
                });

            modelBuilder.Entity("Project", b =>
                {
                    b.Property<int>("IdProject")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdProject"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedWhen")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdAc")
                        .HasColumnType("int");

                    b.Property<int?>("LastUpdateBy")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdateWhen")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("IdProject");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("IdAc");

                    b.HasIndex("LastUpdateBy");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("AccountDetail", b =>
                {
                    b.HasOne("Business.Account", "Creator")
                        .WithMany("CreatedAd")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Business.Account", "account")
                        .WithOne("AccountDetail")
                        .HasForeignKey("AccountDetail", "IdAccount")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Account", "Updator")
                        .WithMany("UpdatedAd")
                        .HasForeignKey("LastUpdateBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Updator");

                    b.Navigation("account");
                });

            modelBuilder.Entity("Business.Account", b =>
                {
                    b.HasOne("Business.Account", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("Business.Role", "AccountRole")
                        .WithMany("RoleAccount")
                        .HasForeignKey("IdRole")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Account", "Updater")
                        .WithMany()
                        .HasForeignKey("LastUpdateBy");

                    b.Navigation("AccountRole");

                    b.Navigation("Creator");

                    b.Navigation("Updater");
                });

            modelBuilder.Entity("Business.Artwork", b =>
                {
                    b.HasOne("Business.Account", "Creator")
                        .WithMany("CreatedArt")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Business.Account", "Account")
                        .WithMany("Artworks")
                        .HasForeignKey("IdAc")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.TypeOfArtwork", "TypeOfArtwork")
                        .WithMany("Artworks")
                        .HasForeignKey("IdTypeOfArtwork")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Account", "Updater")
                        .WithMany("UpdatedArt")
                        .HasForeignKey("LastUpdateBy")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Account");

                    b.Navigation("Creator");

                    b.Navigation("TypeOfArtwork");

                    b.Navigation("Updater");
                });

            modelBuilder.Entity("Business.Comment", b =>
                {
                    b.HasOne("Business.Account", "Account")
                        .WithMany("Comments")
                        .HasForeignKey("IdAc")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Artwork", "Artwork")
                        .WithMany("Comments")
                        .HasForeignKey("IdArtwork")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Artwork");
                });

            modelBuilder.Entity("Business.DocumentInfo", b =>
                {
                    b.HasOne("Business.Account", null)
                        .WithMany("DocumentInfos")
                        .HasForeignKey("AccountIdAccount");

                    b.HasOne("Business.Account", "CreatedBy")
                        .WithMany("CreatedDocuments")
                        .HasForeignKey("Created_by")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AccountDetail", "AccountDetail")
                        .WithMany()
                        .HasForeignKey("IdAcDt");

                    b.HasOne("Business.Artwork", "IdArtworkNavigation")
                        .WithMany("DocumentInfos")
                        .HasForeignKey("IdArtwork")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Business.Event", "IdEventNavigation")
                        .WithMany("DocumentInfos")
                        .HasForeignKey("IdEvent")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Project", "IdProjectNavigation")
                        .WithMany("DocumentInfos")
                        .HasForeignKey("IdProject")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Business.Account", "LastUpdatedBy")
                        .WithMany("UpdatedDocuments")
                        .HasForeignKey("Last_update_by")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("AccountDetail");

                    b.Navigation("CreatedBy");

                    b.Navigation("IdArtworkNavigation");

                    b.Navigation("IdEventNavigation");

                    b.Navigation("IdProjectNavigation");

                    b.Navigation("LastUpdatedBy");
                });

            modelBuilder.Entity("Business.Event", b =>
                {
                    b.HasOne("Business.Account", "Creator")
                        .WithMany("CreatedE")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Business.Account", "Account")
                        .WithMany("Events")
                        .HasForeignKey("IdAc")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Account", "Updater")
                        .WithMany("UpdatedE")
                        .HasForeignKey("LastUpdateBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Creator");

                    b.Navigation("Updater");
                });

            modelBuilder.Entity("Business.EventParticipants", b =>
                {
                    b.HasOne("Business.Account", "Account")
                        .WithMany("EPAccDt")
                        .HasForeignKey("IdAc")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Event", "Event")
                        .WithMany("EventParticipants")
                        .HasForeignKey("IdEvent")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("Business.Follow", b =>
                {
                    b.HasOne("Business.Account", "Follower")
                        .WithMany("Followers")
                        .HasForeignKey("IdFollower")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Account", "Following")
                        .WithMany("Following")
                        .HasForeignKey("IdFollowing")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Follower");

                    b.Navigation("Following");
                });

            modelBuilder.Entity("Business.ProjectParticipant", b =>
                {
                    b.HasOne("Business.Account", "Account")
                        .WithMany("PjAccDt")
                        .HasForeignKey("IdAc")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Project", "Project")
                        .WithMany("ProjectParticipants")
                        .HasForeignKey("IdProject")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Business.Reaction", b =>
                {
                    b.HasOne("Business.Account", "Account")
                        .WithMany("Reactions")
                        .HasForeignKey("IdAc")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Artwork", "Artwork")
                        .WithMany("Reactions")
                        .HasForeignKey("IdArtwork")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Artwork");
                });

            modelBuilder.Entity("Project", b =>
                {
                    b.HasOne("Business.Account", "Creator")
                        .WithMany("CreatedPj")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Account", "Account")
                        .WithMany("Projects")
                        .HasForeignKey("IdAc")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Business.Account", "Updater")
                        .WithMany("UpdatedPj")
                        .HasForeignKey("LastUpdateBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Creator");

                    b.Navigation("Updater");
                });

            modelBuilder.Entity("Business.Account", b =>
                {
                    b.Navigation("AccountDetail")
                        .IsRequired();

                    b.Navigation("Artworks");

                    b.Navigation("Comments");

                    b.Navigation("CreatedAd");

                    b.Navigation("CreatedArt");

                    b.Navigation("CreatedDocuments");

                    b.Navigation("CreatedE");

                    b.Navigation("CreatedPj");

                    b.Navigation("DocumentInfos");

                    b.Navigation("EPAccDt");

                    b.Navigation("Events");

                    b.Navigation("Followers");

                    b.Navigation("Following");

                    b.Navigation("PjAccDt");

                    b.Navigation("Projects");

                    b.Navigation("Reactions");

                    b.Navigation("UpdatedAd");

                    b.Navigation("UpdatedArt");

                    b.Navigation("UpdatedDocuments");

                    b.Navigation("UpdatedE");

                    b.Navigation("UpdatedPj");
                });

            modelBuilder.Entity("Business.Artwork", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("DocumentInfos");

                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("Business.Event", b =>
                {
                    b.Navigation("DocumentInfos");

                    b.Navigation("EventParticipants");
                });

            modelBuilder.Entity("Business.Role", b =>
                {
                    b.Navigation("RoleAccount");
                });

            modelBuilder.Entity("Business.TypeOfArtwork", b =>
                {
                    b.Navigation("Artworks");
                });

            modelBuilder.Entity("Project", b =>
                {
                    b.Navigation("DocumentInfos");

                    b.Navigation("ProjectParticipants");
                });
#pragma warning restore 612, 618
        }
    }
}