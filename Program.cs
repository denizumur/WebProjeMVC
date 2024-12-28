using Microsoft.EntityFrameworkCore;
using berber.Models;  // Context s�n�f�n�z�n bulundu�u namespace

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s� ve DbContext'i ekleyin
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// MVC ve API controller'lar�n� ekleyin
builder.Services.AddControllersWithViews();

// Session yap�land�rmas�
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); // Session zaman a��m� s�resi
    options.Cookie.HttpOnly = true; // G�venlik amac�yla sadece HTTP isteklerinde kullan�labilir
    options.Cookie.IsEssential = true; // Session'�n temel bir �erez olmas�
});

var app = builder.Build();

// HTTP istekleri i�in middleware pipeline'� yap�land�rma
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS i�in �nerilen ayarlar
    app.UseHsts();
}

app.UseSession(); // Session'� aktif hale getirme

app.UseHttpsRedirection(); // HTTPS y�nlendirmeleri
app.UseStaticFiles(); // Statik dosyalar�n servis edilmesi

app.UseRouting(); // Routing (y�nlendirme) i�lemi

app.UseAuthorization(); // Authorization i�lemi

// API controller'lar� i�in y�nlendirmeler
app.MapControllers();

// MVC controller'lar� i�in varsay�lan y�nlendirme
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

// Uygulaman�n �al��t�r�lmas�
app.Run();
