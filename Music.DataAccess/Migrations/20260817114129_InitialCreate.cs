using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Lyrics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SongAuthors",
                columns: table => new
                {
                    SongId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongAuthors", x => new { x.SongId, x.AuthorId });
                    table.ForeignKey(
                        name: "FK_SongAuthors_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongAuthors_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SongGenres",
                columns: table => new
                {
                    SongId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongGenres", x => new { x.SongId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_SongGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongGenres_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongAuthors_AuthorId",
                table: "SongAuthors",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SongGenres_GenreId",
                table: "SongGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_UserId",
                table: "Songs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO [Roles] ([Name]) VALUES ('Admin'), ('User');

                DECLARE @adminRoleId INT = (SELECT [Id] FROM [Roles] WHERE [Name] = 'Admin');
                DECLARE @userRoleId INT = (SELECT [Id] FROM [Roles] WHERE [Name] = 'User');

                INSERT INTO [Users] ([PasswordHash], [Salt], [Username], [IsApproved], [CreatedAt])
                VALUES ('0YSTpL6IHUyu6267RnH/f+4xnNhr28Cc4prneHUIojU=', 'AQIDBAUGBwgJCgsMDQ4PEBESExQVFhcYGRobHB0eHyA=', 'admin', 1, '2026-01-01T00:00:00');

                INSERT INTO [Users] ([PasswordHash], [Salt], [Username], [IsApproved], [CreatedAt])
                VALUES ('KXVYc4/JJdxYjAlvHvzTtFZK/td5T904TQSZscq6L74=', 'AQIDBAUGBwgJCgsMDQ4PEBESExQVFhcYGRobHB0eHyA=', 'demo', 1, '2026-01-01T00:00:00');

                DECLARE @adminUserId INT = (SELECT [Id] FROM [Users] WHERE [Username] = 'admin');
                DECLARE @demoUserId INT = (SELECT [Id] FROM [Users] WHERE [Username] = 'demo');

                INSERT INTO [UserRoles] ([UserId], [RoleId]) VALUES (@adminUserId, @adminRoleId);
                INSERT INTO [UserRoles] ([UserId], [RoleId]) VALUES (@demoUserId, @userRoleId);

                INSERT INTO [Genres] ([Name], [Description]) VALUES
                    ('Pop', 'Поп-музыка'),
                    ('Rock', 'Рок-музыка'),
                    ('Jazz', 'Джазовая музыка'),
                    ('Electronic', 'Электронная музыка'),
                    ('Hip-Hop', 'Хип-хоп музыка');

                INSERT INTO [Authors] ([Name], [Country], [Description]) VALUES
                    ('John Smith', 'USA', 'Поп-исполнитель'),
                    ('Maria Garcia', 'Spain', 'Джаз и классический вокал'),
                    ('Alex Johnson', 'UK', 'Поп-рок'),
                    ('Sarah Williams', 'Canada', 'Инди-фолк'),
                    ('DJ Shadow', 'USA', 'Электронный диджей');

                DECLARE @popId INT = (SELECT [Id] FROM [Genres] WHERE [Name] = 'Pop');
                DECLARE @rockId INT = (SELECT [Id] FROM [Genres] WHERE [Name] = 'Rock');
                DECLARE @jazzId INT = (SELECT [Id] FROM [Genres] WHERE [Name] = 'Jazz');
                DECLARE @electronicId INT = (SELECT [Id] FROM [Genres] WHERE [Name] = 'Electronic');
                DECLARE @hiphopId INT = (SELECT [Id] FROM [Genres] WHERE [Name] = 'Hip-Hop');

                DECLARE @johnId INT = (SELECT [Id] FROM [Authors] WHERE [Name] = 'John Smith');
                DECLARE @mariaId INT = (SELECT [Id] FROM [Authors] WHERE [Name] = 'Maria Garcia');
                DECLARE @alexId INT = (SELECT [Id] FROM [Authors] WHERE [Name] = 'Alex Johnson');
                DECLARE @sarahId INT = (SELECT [Id] FROM [Authors] WHERE [Name] = 'Sarah Williams');
                DECLARE @djId INT = (SELECT [Id] FROM [Authors] WHERE [Name] = 'DJ Shadow');

                INSERT INTO [Songs] ([Title], [UserId], [FilePath], [Duration], [Lyrics], [PlayCount], [CreatedAt])
                VALUES ('Summer Nights', @demoUserId, 'summer_nights.mp3', 215, 'Summer nights, under the stars...', 1250, '2026-01-01T00:00:00'),
                       ('Electric Dreams', @demoUserId, 'electric_dreams.mp3', 248, 'Electric dreams running through my mind...', 3420, '2026-01-01T00:00:00'),
                       ('Midnight Jazz', @demoUserId, 'midnight_jazz.mp3', 312, 'Midnight jazz, playing slow...', 890, '2026-01-01T00:00:00'),
                       ('Rock Anthem', @demoUserId, 'rock_anthem.mp3', 276, 'We are the heroes of the night...', 2100, '2026-01-01T00:00:00'),
                       ('Street Flow', @demoUserId, 'street_flow.mp3', 195, 'Walking down the street with the flow...', 1670, '2026-01-01T00:00:00');

                DECLARE @song1Id INT = (SELECT [Id] FROM [Songs] WHERE [Title] = 'Summer Nights');
                DECLARE @song2Id INT = (SELECT [Id] FROM [Songs] WHERE [Title] = 'Electric Dreams');
                DECLARE @song3Id INT = (SELECT [Id] FROM [Songs] WHERE [Title] = 'Midnight Jazz');
                DECLARE @song4Id INT = (SELECT [Id] FROM [Songs] WHERE [Title] = 'Rock Anthem');
                DECLARE @song5Id INT = (SELECT [Id] FROM [Songs] WHERE [Title] = 'Street Flow');

                INSERT INTO [SongGenres] ([SongId], [GenreId]) VALUES (@song1Id, @popId), (@song2Id, @electronicId), (@song3Id, @jazzId), (@song4Id, @rockId), (@song5Id, @hiphopId);
                INSERT INTO [SongAuthors] ([SongId], [AuthorId]) VALUES (@song1Id, @johnId), (@song2Id, @djId), (@song3Id, @sarahId), (@song4Id, @alexId), (@song5Id, @mariaId);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongAuthors");

            migrationBuilder.DropTable(
                name: "SongGenres");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
