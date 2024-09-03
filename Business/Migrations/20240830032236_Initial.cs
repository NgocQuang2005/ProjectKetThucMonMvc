using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRole);
                });

            migrationBuilder.CreateTable(
                name: "TypeOfArtworks",
                columns: table => new
                {
                    IdTypeOfArtwork = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    NameTypeOfArtwork = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOfArtworks", x => x.IdTypeOfArtwork);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    IdAccount = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdateWhen = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.IdAccount);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount");
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_LastUpdateBy",
                        column: x => x.LastUpdateBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount");
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_IdRole",
                        column: x => x.IdRole,
                        principalTable: "Roles",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountDetails",
                columns: table => new
                {
                    IdAccountDt = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdAccount = table.Column<int>(type: "int", nullable: false),
                    CCCD = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateBy = table.Column<int>(type: "int", nullable: false),
                    LastUpdateWhen = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDetails", x => x.IdAccountDt);
                    table.ForeignKey(
                        name: "FK_AccountDetails_Accounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_Accounts_IdAccount",
                        column: x => x.IdAccount,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_Accounts_LastUpdateBy",
                        column: x => x.LastUpdateBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Artworks",
                columns: table => new
                {
                    IdArtwork = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IdAc = table.Column<int>(type: "int", nullable: false),
                    IdTypeOfArtwork = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Watched = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdateWhen = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artworks", x => x.IdArtwork);
                    table.ForeignKey(
                        name: "FK_Artworks_Accounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Artworks_Accounts_IdAc",
                        column: x => x.IdAc,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Artworks_Accounts_LastUpdateBy",
                        column: x => x.LastUpdateBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Artworks_TypeOfArtworks_IdTypeOfArtwork",
                        column: x => x.IdTypeOfArtwork,
                        principalTable: "TypeOfArtworks",
                        principalColumn: "IdTypeOfArtwork",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Follows",
                columns: table => new
                {
                    IdFollow = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IdFollower = table.Column<int>(type: "int", nullable: false),
                    IdFollowing = table.Column<int>(type: "int", nullable: false),
                    CreatedWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateWhen = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follows", x => x.IdFollow);
                    table.ForeignKey(
                        name: "FK_Follows_Accounts_IdFollower",
                        column: x => x.IdFollower,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Follows_Accounts_IdFollowing",
                        column: x => x.IdFollowing,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    IdProject = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdAc = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastUpdateWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.IdProject);
                    table.ForeignKey(
                        name: "FK_Projects_Accounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Accounts_IdAc",
                        column: x => x.IdAc,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Accounts_LastUpdateBy",
                        column: x => x.LastUpdateBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    IdEvent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdAcDt = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdateWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountIdAccount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.IdEvent);
                    table.ForeignKey(
                        name: "FK_Events_AccountDetails_IdAcDt",
                        column: x => x.IdAcDt,
                        principalTable: "AccountDetails",
                        principalColumn: "IdAccountDt",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Accounts_AccountIdAccount",
                        column: x => x.AccountIdAccount,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount");
                    table.ForeignKey(
                        name: "FK_Events_Accounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Accounts_LastUpdateBy",
                        column: x => x.LastUpdateBy,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    IdComment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IdArtwork = table.Column<int>(type: "int", nullable: false),
                    IdAc = table.Column<int>(type: "int", nullable: false),
                    IdPrevComment = table.Column<int>(type: "int", nullable: true),
                    Reaction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.IdComment);
                    table.ForeignKey(
                        name: "FK_Comments_Accounts_IdAc",
                        column: x => x.IdAc,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Artworks_IdArtwork",
                        column: x => x.IdArtwork,
                        principalTable: "Artworks",
                        principalColumn: "IdArtwork",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    IdReaction = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IdArtwork = table.Column<int>(type: "int", nullable: false),
                    IdAc = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.IdReaction);
                    table.ForeignKey(
                        name: "FK_Reactions_Accounts_IdAc",
                        column: x => x.IdAc,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reactions_Artworks_IdArtwork",
                        column: x => x.IdArtwork,
                        principalTable: "Artworks",
                        principalColumn: "IdArtwork",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectParticipants",
                columns: table => new
                {
                    IdProjectParticipant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IdProject = table.Column<int>(type: "int", nullable: false),
                    IdAc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectParticipants", x => x.IdProjectParticipant);
                    table.ForeignKey(
                        name: "FK_ProjectParticipants_Accounts_IdAc",
                        column: x => x.IdAc,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectParticipants_Projects_IdProject",
                        column: x => x.IdProject,
                        principalTable: "Projects",
                        principalColumn: "IdProject",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentInfos",
                columns: table => new
                {
                    IdDcIf = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IdAcDt = table.Column<int>(type: "int", nullable: true),
                    IdEvent = table.Column<int>(type: "int", nullable: true),
                    IdProject = table.Column<int>(type: "int", nullable: true),
                    IdArtwork = table.Column<int>(type: "int", nullable: true),
                    TypeFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_by = table.Column<int>(type: "int", nullable: true),
                    Created_when = table.Column<DateTime>(type: "datetime", nullable: true),
                    Last_update_by = table.Column<int>(type: "int", nullable: true),
                    Last_update_when = table.Column<DateTime>(type: "datetime", nullable: true),
                    AccountIdAccount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentInfos", x => x.IdDcIf);
                    table.ForeignKey(
                        name: "FK_DocumentInfos_AccountDetails_IdAcDt",
                        column: x => x.IdAcDt,
                        principalTable: "AccountDetails",
                        principalColumn: "IdAccountDt");
                    table.ForeignKey(
                        name: "FK_DocumentInfos_Accounts_AccountIdAccount",
                        column: x => x.AccountIdAccount,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount");
                    table.ForeignKey(
                        name: "FK_DocumentInfos_Accounts_Created_by",
                        column: x => x.Created_by,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentInfos_Accounts_Last_update_by",
                        column: x => x.Last_update_by,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentInfos_Artworks_IdArtwork",
                        column: x => x.IdArtwork,
                        principalTable: "Artworks",
                        principalColumn: "IdArtwork",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentInfos_Events_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Events",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentInfos_Projects_IdProject",
                        column: x => x.IdProject,
                        principalTable: "Projects",
                        principalColumn: "IdProject",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipants",
                columns: table => new
                {
                    IdEventParticipant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IdEvent = table.Column<int>(type: "int", nullable: false),
                    IdAc = table.Column<int>(type: "int", nullable: false),
                    RegistrationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipants", x => x.IdEventParticipant);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Accounts_IdAc",
                        column: x => x.IdAc,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Events_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Events",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRole", "Active", "RoleName" },
                values: new object[,]
                {
                    { 1, false, "Admin" },
                    { 2, false, "User" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "IdAccount", "CreatedBy", "CreatedWhen", "Email", "IdRole", "LastUpdateBy", "LastUpdateWhen", "Password", "Phone" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1154), "Quang111420@gmail.com", 1, null, new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1155), "quang111420", null },
                    { 2, null, new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1158), "khang2007@gmail.com", 2, null, new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1159), "khang2007", null }
                });

            migrationBuilder.InsertData(
                table: "AccountDetails",
                columns: new[] { "IdAccountDt", "Active", "Address", "Birthday", "CCCD", "CreatedBy", "CreatedWhen", "Description", "Fullname", "Gender", "IdAccount", "LastUpdateBy", "LastUpdateWhen", "Nationality" },
                values: new object[,]
                {
                    { 1, true, null, null, 123456789, 1, new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1193), "Admin account", "Nguyễn Ngọc Quang", "Nam", 1, 1, new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1194), "Vietnam" },
                    { 2, true, null, null, 987654321, 2, new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1198), "User account", "Minh Khang", "Nam", 2, 1, new DateTime(2024, 8, 30, 10, 22, 35, 930, DateTimeKind.Local).AddTicks(1199), "Vietnam" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_CreatedBy",
                table: "AccountDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_IdAccount",
                table: "AccountDetails",
                column: "IdAccount",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_LastUpdateBy",
                table: "AccountDetails",
                column: "LastUpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CreatedBy",
                table: "Accounts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_IdRole",
                table: "Accounts",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LastUpdateBy",
                table: "Accounts",
                column: "LastUpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Artworks_CreatedBy",
                table: "Artworks",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Artworks_IdAc",
                table: "Artworks",
                column: "IdAc");

            migrationBuilder.CreateIndex(
                name: "IX_Artworks_IdTypeOfArtwork",
                table: "Artworks",
                column: "IdTypeOfArtwork");

            migrationBuilder.CreateIndex(
                name: "IX_Artworks_LastUpdateBy",
                table: "Artworks",
                column: "LastUpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IdAc",
                table: "Comments",
                column: "IdAc");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IdArtwork",
                table: "Comments",
                column: "IdArtwork");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfos_AccountIdAccount",
                table: "DocumentInfos",
                column: "AccountIdAccount");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfos_Created_by",
                table: "DocumentInfos",
                column: "Created_by");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfos_IdAcDt",
                table: "DocumentInfos",
                column: "IdAcDt");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfos_IdArtwork",
                table: "DocumentInfos",
                column: "IdArtwork");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfos_IdEvent",
                table: "DocumentInfos",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfos_IdProject",
                table: "DocumentInfos",
                column: "IdProject");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfos_Last_update_by",
                table: "DocumentInfos",
                column: "Last_update_by");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_IdAc",
                table: "EventParticipants",
                column: "IdAc");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_IdEvent",
                table: "EventParticipants",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AccountIdAccount",
                table: "Events",
                column: "AccountIdAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedBy",
                table: "Events",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Events_IdAcDt",
                table: "Events",
                column: "IdAcDt");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LastUpdateBy",
                table: "Events",
                column: "LastUpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_IdFollower",
                table: "Follows",
                column: "IdFollower");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_IdFollowing",
                table: "Follows",
                column: "IdFollowing");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectParticipants_IdAc",
                table: "ProjectParticipants",
                column: "IdAc");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectParticipants_IdProject",
                table: "ProjectParticipants",
                column: "IdProject");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedBy",
                table: "Projects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IdAc",
                table: "Projects",
                column: "IdAc");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastUpdateBy",
                table: "Projects",
                column: "LastUpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_IdAc",
                table: "Reactions",
                column: "IdAc");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_IdArtwork",
                table: "Reactions",
                column: "IdArtwork");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "DocumentInfos");

            migrationBuilder.DropTable(
                name: "EventParticipants");

            migrationBuilder.DropTable(
                name: "Follows");

            migrationBuilder.DropTable(
                name: "ProjectParticipants");

            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Artworks");

            migrationBuilder.DropTable(
                name: "AccountDetails");

            migrationBuilder.DropTable(
                name: "TypeOfArtworks");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
