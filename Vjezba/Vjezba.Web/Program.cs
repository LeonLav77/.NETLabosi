using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Vjezba.DAL;
using Vjezba.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddDbContext<ClientManagerDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("ClientManagerDbContext"),
        opt => opt.MigrationsAssembly("Vjezba.DAL")));

// Configure supported cultures and localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("hr-HR")
    };
    
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add request localization middleware
app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "contact_forma",
    pattern: "kontakt-forma",
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name: "privacy_lang",
    pattern: "o-aplikaciji/{lang}",
    defaults: new { controller = "Home", action = "Privacy" },
    constraints: new { lang = @"[a-zA-Z]{2}" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();