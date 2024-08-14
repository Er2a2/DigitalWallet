using DigitalWallet.Application.CQRS.Command;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Commmon;
using DigitalWallet.Infrastructure;
using DigitalWallet.Infrastructure.Services;
using DigitalWallet.Middlewares;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DB>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IWalletService, WalletService>();
// استفاده از Configuration از طریق builder
builder.Services.AddCommonInjections(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Digitall Wallet",
        Version = "v1"
    });
    c.EnableAnnotations();
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DepositCommand).Assembly));
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// استفاده از Middleware سفارشی
app.UseCustomExceptionMiddleware();


app.MapControllers();

app.Run();
