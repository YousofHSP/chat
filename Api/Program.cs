using Api.Hubs;
using Common;
using Data.Contracts;
using Data.Repositories;
using Data.Reprositories;
using DTO.CustomMapping;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.FileProviders;
using Services.Services;
using Services.DataInitializer;
using WebFramework.Configuration;
using WebFramework.Middlewares;
using WebFramework.Swagger;


var builder = WebApplication.CreateBuilder(args);
var siteSettings = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));

builder.Services.AddControllers(options => { options.Filters.Add(new AuthorizeFilter()); });


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(siteSettings!.Url);
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddCustomIdentity(siteSettings.IdentitySettings);

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IDataInitializer, UserDataInitializer>();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddJwtAuthentication(siteSettings.JwtSettings);

builder.Services.InitializeAutoMapper();
builder.Services.AddCustomApiVersioning();

var app = builder.Build();

app.DataSeeder(app.Environment);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCustomExceptionHandler();

app.UseCors(options =>
{
    options.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "/uploads"
});
app.MapHub<ChatHub>("/chat");


app.Run();
