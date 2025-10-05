using Microsoft.EntityFrameworkCore;
using CarRentalApi.Data;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CarRentalApi.Services;
using CarRentalApi.Services.Interfaces;
using CareRentalApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

var DevelopmentCorsPolicy = "DevelopmentCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<CarRentalContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: DevelopmentCorsPolicy,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200").WithMethods("POST", "PUT", "DELETE", "GET").AllowAnyHeader();
                      });
});

var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";

// Auth0

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
// .AddJwtBearer(options =>
// {
//     options.Authority = domain;
//     options.Audience = builder.Configuration["Auth0:Audience"];
// });

// External DB Connection

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

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<ICarService, CarService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
      {
          c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyProject", Version = "v1.0.0" });

          string securityDefinitionName = "Bearer";

          var securitySchema = new OpenApiSecurityScheme
          {
              Description = "Using the Authorization header with the Bearer scheme.",
              Name = "Authorization",
              In = ParameterLocation.Header,
              Type = SecuritySchemeType.Http,
              Scheme = "bearer",
              Reference = new OpenApiReference
              {
                  Type = ReferenceType.SecurityScheme,
                  Id = "Bearer"
              }
          };

          OpenApiSecurityRequirement securityRequirement = new OpenApiSecurityRequirement
          {
              { securitySchema, new[] { "Bearer" } }
          };

          c.AddSecurityDefinition(securityDefinitionName, securitySchema);

          c.AddSecurityRequirement(securityRequirement);
      });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
});


var app = builder.Build();

app.MapIdentityApi<IdentityUser>();

// Create scope and seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarRentalContext>();
    SeedData.Initialize(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(DevelopmentCorsPolicy);
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();