using Microsoft.EntityFrameworkCore;
using TP2.Context;
using TP2.Models.Repositories;
using TP2.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Ajouter AppDbContext avec la chaîne de connexion "ProductDBConnection"
builder.Services.AddDbContextPool<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDBConnection"
)));


// Injection de l'interface IRepository et de son implémentation SqlProductRepository
builder.Services.AddScoped<IRepository<Product>, SqlProductRepository>();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
