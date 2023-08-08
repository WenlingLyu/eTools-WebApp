#region Additional Namespaces
using SalesReturnsSystem;
using PurchasingSystem;
using SecurityDependencies;
#endregion
using eToolsWebFramework.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// given
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// added
var SalesReturnsConnectionString = builder.Configuration.GetConnectionString("SalesReturnsConnectionString");
var PurchasingDBConnectionString = builder.Configuration.GetConnectionString("PurchasingDB"); 

//given
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// added
builder.Services.SalesReturnsSystemBackendDependencies(options => options.UseSqlServer(SalesReturnsConnectionString));
builder.Services.SecuritySystemBackendDependencies(options => options.UseSqlServer(SalesReturnsConnectionString));
builder.Services.PurchasingSystemBackendDependencies(options => options.UseSqlServer(PurchasingDBConnectionString));
builder.Services.SecuritySystemBackendDependencies(options => options.UseSqlServer(PurchasingDBConnectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
