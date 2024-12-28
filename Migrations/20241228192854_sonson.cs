using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace berber.Migrations
{
    /// <inheritdoc />
    public partial class sonson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calisanlar",
                columns: table => new
                {
                    CalisanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UzmanlikAlanlari = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UygunlukSaatleri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calisanlar", x => x.CalisanID);
                });

            migrationBuilder.CreateTable(
                name: "Islemler",
                columns: table => new
                {
                    IslemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IslemAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sure = table.Column<int>(type: "int", nullable: false),
                    Ucret = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Islemler", x => x.IslemID);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UyelikTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.KullaniciID);
                });

            migrationBuilder.CreateTable(
                name: "CalisanIslemler",
                columns: table => new
                {
                    CalisanIslemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanID = table.Column<int>(type: "int", nullable: false),
                    IslemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalisanIslemler", x => x.CalisanIslemID);
                    table.ForeignKey(
                        name: "FK_CalisanIslemler_Calisanlar_CalisanID",
                        column: x => x.CalisanID,
                        principalTable: "Calisanlar",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalisanIslemler_Islemler_IslemID",
                        column: x => x.IslemID,
                        principalTable: "Islemler",
                        principalColumn: "IslemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Randevular",
                columns: table => new
                {
                    RandevuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<bool>(type: "bit", nullable: true),
                    KullaniciID = table.Column<int>(type: "int", nullable: true),
                    CalisanID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Randevular", x => x.RandevuID);
                    table.ForeignKey(
                        name: "FK_Randevular_Calisanlar_CalisanID",
                        column: x => x.CalisanID,
                        principalTable: "Calisanlar",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Randevular_Kullanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RandevuIslemler",
                columns: table => new
                {
                    RandevuIslemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RandevuID = table.Column<int>(type: "int", nullable: false),
                    IslemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RandevuIslemler", x => x.RandevuIslemID);
                    table.ForeignKey(
                        name: "FK_RandevuIslemler_Islemler_IslemID",
                        column: x => x.IslemID,
                        principalTable: "Islemler",
                        principalColumn: "IslemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RandevuIslemler_Randevular_RandevuID",
                        column: x => x.RandevuID,
                        principalTable: "Randevular",
                        principalColumn: "RandevuID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalisanIslemler_CalisanID",
                table: "CalisanIslemler",
                column: "CalisanID");

            migrationBuilder.CreateIndex(
                name: "IX_CalisanIslemler_IslemID",
                table: "CalisanIslemler",
                column: "IslemID");

            migrationBuilder.CreateIndex(
                name: "IX_RandevuIslemler_IslemID",
                table: "RandevuIslemler",
                column: "IslemID");

            migrationBuilder.CreateIndex(
                name: "IX_RandevuIslemler_RandevuID",
                table: "RandevuIslemler",
                column: "RandevuID");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_CalisanID",
                table: "Randevular",
                column: "CalisanID");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_KullaniciID",
                table: "Randevular",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalisanIslemler");

            migrationBuilder.DropTable(
                name: "RandevuIslemler");

            migrationBuilder.DropTable(
                name: "Islemler");

            migrationBuilder.DropTable(
                name: "Randevular");

            migrationBuilder.DropTable(
                name: "Calisanlar");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
