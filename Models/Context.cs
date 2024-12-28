using Microsoft.EntityFrameworkCore;



namespace berber.Models
{
    public class Context:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-DESKTOP-KRGVO7T\\SQLEXPRESS; database=berber32 ; integrated security=true; TrustServerCertificate=True;");
        }
        public Context(DbContextOptions<Context> options) : base(options) { }

        public Context()
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
		public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<Islem> Islemler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

		public DbSet<RandevuIslem> RandevuIslemler { get; set; }
		public DbSet<CalisanIslem> CalisanIslemler { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Çalışan-Hizmet (Çok-Çok) ilişkisi
			modelBuilder.Entity<CalisanIslem>()
				.HasKey(ci => ci.CalisanIslemID);

			modelBuilder.Entity<CalisanIslem>()
				.HasOne(ci => ci.Calisan)
				.WithMany(c => c.CalisanIslemler)
				.HasForeignKey(ci => ci.CalisanID);

			modelBuilder.Entity<CalisanIslem>()
				.HasOne(ci => ci.Islem)
				.WithMany(i => i.CalisanIslemler)
				.HasForeignKey(ci => ci.IslemID);

			// Randevu-Hizmet (Çok-Çok) ilişkisi
			modelBuilder.Entity<RandevuIslem>()
				.HasKey(ri => ri.RandevuIslemID);

			modelBuilder.Entity<RandevuIslem>()
				.HasOne(ri => ri.Randevu)
				.WithMany(r => r.RandevuIslemler)
				.HasForeignKey(ri => ri.RandevuID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<RandevuIslem>()
				.HasOne(ri => ri.Islem)
				.WithMany(i => i.RandevuIslemler)
				.HasForeignKey(ri => ri.IslemID);

			// Kullanıcı-Randevu (Bir-Karşı-Çok) ilişkisi
			modelBuilder.Entity<Randevu>()
				.HasOne(r => r.Kullanici)
				.WithMany(k => k.Randevular)
				.HasForeignKey(r => r.KullaniciID)
				.OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse, randevular da silinir

			// Çalışan-Randevu (Bir-Karşı-Çok) ilişkisi
			modelBuilder.Entity<Randevu>()
				.HasOne(r => r.Calisan)
				.WithMany(c => c.Randevular)
				.HasForeignKey(r => r.CalisanID)
				.OnDelete(DeleteBehavior.Cascade); // Çalışan silinirse, randevular da silinir

			modelBuilder.Entity<CalisanIslem>()
		.HasOne(ci => ci.Islem)
		.WithMany(i => i.CalisanIslemler)
		.HasForeignKey(ci => ci.IslemID)
		.OnDelete(DeleteBehavior.Cascade); // Hizmet silindiğinde çalışan ilişkisi silinsin

			// CalisanIslemler ile Calisan arasındaki ilişki
			modelBuilder.Entity<CalisanIslem>()
				.HasOne(ci => ci.Calisan)
				.WithMany(c => c.CalisanIslemler)
				.HasForeignKey(ci => ci.CalisanID)
				.OnDelete(DeleteBehavior.Cascade);

		}


	}
}
