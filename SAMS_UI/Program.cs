using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SAMS_UI.Authorization;
using SAMS_UI.Data;
using SAMS_UI.Services.Implementations;
using SAMS_UI.Services.Interfaces;
using Serilog;
using WhatsAppUI.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("BackendApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
});
builder.Services.AddSingleton<AuthTokenManager>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUpdateUserActivityStatus, UpdateUserActivityStatus>();
builder.Services.AddScoped<IRegisterUserService, RegisterUserService>();
builder.Services.AddScoped<IGetUserById, GetUserById>();
builder.Services.AddScoped<IRolePolicyQueryService, RolePolicyQueryService>();
builder.Services.AddScoped<IRolePolicyService, RolePolicyService>();
builder.Services.AddScoped<IViewPolicyService, ViewPolicyService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PolicyHandler>();


builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<DataSeeder>();

builder.Services.AddAuthorization();
builder.Services.AddAuthorization();



builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Index";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var tokenManager = scope.ServiceProvider.GetRequiredService<AuthTokenManager>();
    await tokenManager.GetTokenAsync(); // Force initial login
}


using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}


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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}");

app.Run();
