using Microsoft.EntityFrameworkCore;
using Pustok.Data.DAL;
using Pustok.Data.Repositories.Implementations;
using Pustok.Core.Repositories.Interfaces;
using Pustok.Business.Services.Implementations;
using Pustok.Business.Services.Interfaces;
using static Pustok.Core.Repositories.Interfaces.IAuthorIRepository;
using Pustok.Core.Models;
using Microsoft.AspNetCore.Identity;
using static Pustok.Core.Repositories.Interfaces.IBookImageRepository;
using Pustok.MVC.ViewServices;
using Pustok.Business.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IAuthorRepository,AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAuthorService, AuhtorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IBookTagsRepository, BookTagRepository>();
builder.Services.AddScoped<IBookImagesRepository, BookImagesRepository>();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(20);
});
builder.Services.AddDbContext<PustokContext>(opt =>
{
    opt.UseSqlServer("Server=LAPTOP-N2MJ83JU\\SQLEXPRESS;Database=Pustoknew0;Trusted_Connection=True");
});
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequiredUniqueChars = 0;
    opt.Password.RequireNonAlphanumeric= true;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireDigit= true;
    opt.Password.RequireLowercase= true;
    opt.Password.RequireUppercase= true;
    opt.User.RequireUniqueEmail = false;

}
).AddEntityFrameworkStores<PustokContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddSignalR();


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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chaturl");

app.Run();