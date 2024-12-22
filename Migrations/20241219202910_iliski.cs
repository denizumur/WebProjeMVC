using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace berber.Migrations
{
	/// <inheritdoc />
	public partial class iliski : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "KullaniciID",
				table: "Randevular",
				type: "int",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_Randevular_KullaniciID",
				table: "Randevular",
				column: "KullaniciID");

			migrationBuilder.AddForeignKey(
				name: "FK_Randevular_Kullanicilar_KullaniciID",
				table: "Randevular",
				column: "KullaniciID",
				principalTable: "Kullanicilar",
				principalColumn: "KullaniciID");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Randevular_Kullanicilar_KullaniciID",
				table: "Randevular");

			migrationBuilder.DropIndex(
				name: "IX_Randevular_KullaniciID",
				table: "Randevular");

			migrationBuilder.DropColumn(
				name: "KullaniciID",
				table: "Randevular");
		}
	}
}
