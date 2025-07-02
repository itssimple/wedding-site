using Microsoft.EntityFrameworkCore;
using WeddingSite;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(builder.Configuration["ConfigFile"] ?? "wedding-site.report.environment.json");

var connectionString = builder.Configuration["Database:ConnectionString"];

builder.Services.AddDbContext<WeddingContext>(options =>
    options
        .UseLazyLoadingProxies()
        .UseSqlServer(connectionString)
        .LogTo(Console.WriteLine));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".WeddingSite.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.MapControllers();

app.Run();
