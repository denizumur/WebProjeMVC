using Microsoft.EntityFrameworkCore;



namespace berber.Models
{
    public class Context:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-KRGVO7T\\SQLEXPRESS; database=berber; integrated security=true; TrustServerCertificate=True;");
        }

        public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<Islem> Islemler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
    }
}
