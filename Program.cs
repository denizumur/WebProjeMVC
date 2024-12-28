using Microsoft.EntityFrameworkCore;
using berber.Models;  // Context sýnýfýnýzýn bulunduðu namespace

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsý ve DbContext'i ekleyin
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// MVC ve API controller'larýný ekleyin
builder.Services.AddControllersWithViews();

// Session yapýlandýrmasý
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); // Session zaman aþýmý süresi
    options.Cookie.HttpOnly = true; // Güvenlik amacýyla sadece HTTP isteklerinde kullanýlabilir
    options.Cookie.IsEssential = true; // Session'ýn temel bir çerez olmasý
});

var app = builder.Build();

// HTTP istekleri için middleware pipeline'ý yapýlandýrma
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS için önerilen ayarlar
    app.UseHsts();
}

app.UseSession(); // Session'ý aktif hale getirme

app.UseHttpsRedirection(); // HTTPS yönlendirmeleri
app.UseStaticFiles(); // Statik dosyalarýn servis edilmesi

app.UseRouting(); // Routing (yönlendirme) iþlemi

app.UseAuthorization(); // Authorization iþlemi

// API controller'larý için yönlendirmeler
app.MapControllers();

// MVC controller'larý için varsayýlan yönlendirme
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

// Uygulamanýn çalýþtýrýlmasý
app.Run();
