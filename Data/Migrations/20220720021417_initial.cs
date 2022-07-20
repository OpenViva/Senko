using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
	public partial class initial : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterDatabase()
				.Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "Components",
				columns: table => new
				{
					CustomId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
					Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
					OwnerId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
					ChannelId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
					MessageId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
					KyaruComponentType = table.Column<string>(type: "longtext", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Components", x => x.CustomId);
				})
				.Annotation("MySql:CharSet", "utf8mb4");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Components");
		}
	}
}
