using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Infrastructure.Repositories;
using TestProduct.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Global exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Application layer
builder.Services.AddScoped<IProductService, ProductService>();

// Infrastructure layer
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// Exception handler should be early in the middleware pipeline.
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
