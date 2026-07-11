using PIGI_PT_Infraestructure.Configuration;
using PIGI_PT_Application.Commands.Ticket;
using Microsoft.OpenApi;
using FluentValidation;
using PIGI_PT_Infraestructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using PIGI_PT_Infraestructure.BackgroundJobs.Maintenance;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios de Infraestructura (DbContext, Repositorios, UnitOfWork) ────────
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── MediatR: registra todos los handlers de Application ──────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateTicketCommand).Assembly));

// ── FluentValidation: registra todos los validadores de Application ───────────
builder.Services.AddValidatorsFromAssembly(typeof(CreateTicketCommand).Assembly);

// ── Autenticación y Autorización JWT ──────────────────────────────────────────
var secretKey = builder.Configuration["JwtSettings:Secret"] ?? "ClaveSecretaSuperProtegidaDeDesarrollo1234567890!";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "pigi-pt-api",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "pigi-pt-app",
        ClockSkew = TimeSpan.Zero
    };
});

// ── Hangfire: Configurar Motor de Tareas en Segundo Plano ──────────────────────
builder.Services.AddHangfire(config =>
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connStr))
    {
        config.UseInMemoryStorage();
    }
    else
    {
        config.UseSqlServerStorage(connStr);
    }
});
builder.Services.AddHangfireServer();

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PIGI-PT API",
        Version = "v1",
        Description = "Plataforma Inteligente de Gestión y Predicción de Incidentes Tecnológicos"
    });

    // Configuración de Seguridad para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de un espacio y su token JWT."
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PIGI-PT API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// ── Dashboard de Hangfire ────────────────────────────────────────────────────
app.UseHangfireDashboard("/hangfire");

app.MapControllers();

// Health check básico
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithTags("System");

// Seeding y Migración en Desarrollo
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PigiPtDbContext>();
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
        await PigiPtDbSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al migrar o sembrar la base de datos.");
    }
}

// Programar Tareas Recurrentes de Hangfire
RecurringJob.AddOrUpdate<RiskReviewJob>(
    "risk-review-90days",
    job => job.ExecuteAsync(),
    Cron.Daily
);

app.Run();