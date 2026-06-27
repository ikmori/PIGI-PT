using PIGI_PT_WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar HttpClient para comunicación con la API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5003/") });

// Registrar servicios (Fase 3: Autenticación real, vistas de tickets/dashboard mockeadas por análisis de diseño)
builder.Services.AddScoped<PIGI_PT_WebApp.Services.ITicketService, PIGI_PT_WebApp.Services.MockTicketService>();
builder.Services.AddScoped<PIGI_PT_WebApp.Services.IDashboardService, PIGI_PT_WebApp.Services.MockDashboardService>();
builder.Services.AddScoped<PIGI_PT_WebApp.Services.IAuthService, PIGI_PT_WebApp.Services.ApiAuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();