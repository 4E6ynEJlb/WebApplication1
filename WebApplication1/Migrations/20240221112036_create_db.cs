using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMakler.Migrations
{
    /// <inheritdoc />
    public partial class create_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    _user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    _user_name = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    is_admin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x._user_id);
                });

            migrationBuilder.CreateTable(
                name: "ads",
                columns: table => new
                {
                    ad_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    phone_number = table.Column<int>(type: "int", nullable: false),
                    _user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ad_text = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    pic_link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    creation_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deletion_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ads", x => x.ad_id);
                    table.ForeignKey(
                        name: "FK_ads_users__user_id",
                        column: x => x._user_id,
                        principalTable: "users",
                        principalColumn: "_user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ads__user_id",
                table: "ads",
                column: "_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ads");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
