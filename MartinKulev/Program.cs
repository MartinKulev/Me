using MartinKulev.Data;
using MartinKulev.Services.Projects;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<IProjectsService, ProjectsService>();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Logging.ClearProviders(); // remove default providers
builder.Logging.AddSerilog();

builder.Configuration.AddJsonFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GitSecrets.json"), optional: true);
string? connectionString = builder.Configuration.GetConnectionString("MartinKulev") ?? builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<MartinKulevDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Log.Logger.Warning($"Hosting_Environemnt: {app.Environment.EnvironmentName}");
Log.Logger.Warning($"fhgfhkhjkgede34646");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
