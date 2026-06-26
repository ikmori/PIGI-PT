using PIGI_PT_Infraestructure.Configuration;
using PIGI_PT_Application.Commands.Ticket;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios de Infraestructura (DbContext, Repositorios, UnitOfWork) ────────
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── MediatR: registra todos los handlers de Application ──────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateTicketCommand).Assembly));

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PIGI-PT API",
        Version = "v1",
        Description = "Plataforma Inteligente de Gestión y Predicción de Incidentes Tecnológicos"
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
app.MapControllers();

// Health check básico
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithTags("System");

app.Run();