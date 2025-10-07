using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifierTestProject.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NotifierName = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoticeUser",
                columns: table => new
                {
                    NoticesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeUser", x => new { x.NoticesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_NoticeUser_notices_NoticesId",
                        column: x => x.NoticesId,
                        principalTable: "notices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoticeUser_users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notices_Id",
                table: "notices",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeUser_UsersId",
                table: "NoticeUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Id",
                table: "users",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NoticeUser");

            migrationBuilder.DropTable(
                name: "notices");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
