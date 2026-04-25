using AuthService.Application.Intrfaces;
using AuthService.Application.Services;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using FluentValidation.AspNetCore;
using FluentValidation;
using AuthService.Application.Validators;
using MassTransit;
using Microsoft.OpenApi.Models;
using AuthService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
	.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
	.CreateLogger();

builder.Host.UseSerilog();

// Database
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbContext")));


builder.Services.AddScoped<IUserRepository, UserRepositories>();


builder.Services.AddScoped<IAuthAppService, AuthAppServices>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddControllers();

var jwtSecret = builder.Configuration["Jwt:Secret"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
	};
});

builder.Services.AddAuthorization();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddMassTransit(x =>
{
	x.UsingRabbitMq((context, cfg) =>
	{
		cfg.Host("localhost", 5672, "/", h => 
		{
			h.Username("guest");
			h.Password("guest");
		});
	});
});

Console.WriteLine(builder.Configuration["RabbitMq:Username"]);
Console.WriteLine(builder.Configuration["RabbitMq:Password"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthService API",
        Version = "v1",
        Description = "API for user authentication and registration"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter `Bearer {token}`",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
    });

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                }
			},
			Array.Empty<string>()
		}
	});
        
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();


// Configure the HTTP request pipeline

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();

app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

