using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using SistemaCitasAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Gestión de Citas API",
        Version = "v1",
        Description = "API REST para gestión de citas y atención al cliente. Soporta múltiples sectores como salud, belleza, educación y más.",
        Contact = new OpenApiContact
        {
            Name = "Sistema de Citas",
            Email = "contacto@sistemacitas.com"
        }
    });

    c.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "SistemaCitas.Auth",
        Description = "Autenticación basada en cookies"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Cookie"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

// Configure Authentication with Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SistemaCitas.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Citas API v1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "Sistema de Citas API - Documentación";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
