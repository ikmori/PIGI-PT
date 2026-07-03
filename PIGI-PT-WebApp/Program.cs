using PIGI_PT_WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar HttpClient para comunicación con la API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5003/") });

// Registrar servicios de autenticación y servicios HTTP conectados a la API real
builder.Services.AddScoped<PIGI_PT_WebApp.Services.IAuthService, PIGI_PT_WebApp.Services.ApiAuthService>();
builder.Services.AddScoped<PIGI_PT_WebApp.Services.ApiAppServices>();
builder.Services.AddScoped<PIGI_PT_WebApp.Services.ITicketService>(sp => sp.GetRequiredService<PIGI_PT_WebApp.Services.ApiAppServices>());
builder.Services.AddScoped<PIGI_PT_WebApp.Services.IDashboardService>(sp => sp.GetRequiredService<PIGI_PT_WebApp.Services.ApiAppServices>());
builder.Services.AddScoped<PIGI_PT_WebApp.Services.IUsuarioService>(sp => sp.GetRequiredService<PIGI_PT_WebApp.Services.ApiAppServices>());
builder.Services.AddScoped<PIGI_PT_WebApp.Services.ICategoriaService>(sp => sp.GetRequiredService<PIGI_PT_WebApp.Services.ApiAppServices>());
builder.Services.AddScoped<PIGI_PT_WebApp.Services.IRiesgoService>(sp => sp.GetRequiredService<PIGI_PT_WebApp.Services.ApiAppServices>());

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