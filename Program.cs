using Microsoft.EntityFrameworkCore;
using NotifierTestProject.Data;
using NotifierTestProject.Interfaces;
using NotifierTestProject.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddLogging();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite("Data Source=application.db");
});
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<INoticeRepository, NoticeRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (dbContext != null)
        {
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                await dbContext.Database.MigrateAsync();
            }
        }
    }

    await next.Invoke();
});

app.UseRouting();
app.UseStaticFiles();
app.UseSession();
app.MapControllers();

app.MapGet("/mainpage", () =>
{
    var filePath = Path.Combine(builder.Environment.WebRootPath, "mainpage", "index.html");

    return Results.File(filePath, "text/html");
});

app.Run();
