using Microsoft.EntityFrameworkCore;
using CarRentalApi.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// var connection = String.Empty;
// if (builder.Environment.IsDevelopment())
// {
//     connection = builder.Configuration["DbConnectionString"];
// }
// else
// {
//     connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
// }

// builder.Services.AddDbContext<CarRentalContext>(options =>
//     options.UseSqlServer(connection));

builder.Services.AddDbContext<CarRentalContext>(options =>
   options.UseInMemoryDatabase("AppDb"));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
