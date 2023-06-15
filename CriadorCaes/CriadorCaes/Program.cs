using CriadorCaes.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ***************************************************
// Add services to the container.
// ***************************************************


// esta vari�vel cont�m a localiza��o do servidor
// a localiza��o do Servidor est� escrita no ficheiro 'appSettings.json'
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")??throw new InvalidOperationException("A BD referenciada pela Connection string 'DefaultConnection' n�o foi encontrada.");
// inicializar o servi�o de acesso � BD (Sql Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();




builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount=true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
   app.UseMigrationsEndPoint();
}
else {
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
app.MapRazorPages();

app.Run();
