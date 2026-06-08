# Presentation Layer: Blazor Web App (PIGI-PT-WebApp)

**Estado:** ⏳ FASE 5 - Pendiente de implementación  
**Última actualización:** Enero 2024  
**Dependencias:** Requiere PIGI-PT-API (⏳ Fase 3) completada

## Propósito
La capa de **Presentación - Blazor** proporciona la interfaz de usuario web moderna e interactiva.

## 📋 Status de Implementación

| Componente | Estado | Notas |
|-----------|--------|-------|
| **Layout Components** | ⏳ Pendiente | Se crean en Fase 5 (tarea 5.1) |
| **Page Components** | ⏳ Pendiente | Se crean en Fase 5 (tarea 5.1) |
| **Shared Components** | ⏳ Pendiente | Se crean en Fase 5 (tarea 5.1) |
| **Forms & Validation** | ⏳ Pendiente | Se crean en Fase 5 (tarea 5.2) |
| **API Client Services** | ⏳ Pendiente | Se crean en Fase 5 (tarea 5.2) |
| **State Management** | ⏳ Pendiente | Se implementa en Fase 5 (tarea 5.3) |
| **SignalR Real-time** | ⏳ Pendiente | Se implementa post-MVP en Fase 5 |

## ¿Por Qué Blazor?

- **C# en el frontend**: Reutilizar conocimiento de backend
- **Component-based**: Reutilizable y testeable
- **Real-time**: SignalR para actualizaciones en vivo
- **Responsive**: Bootstrap para diseño adaptable
- **Manejo de estado**: Servicios inyectables

## 🗺️ Guía para Fase 5

Para instrucciones detalladas, consulta:
- **[ROADMAP_FUTURO.md - Fase 5](../ROADMAP_FUTURO.md#fase-5-websapp---blazor-wasm-estimado-30-40-horas)** - Componentes, Features, State Management

## Estructura de Carpetas

```
PIGI-PT-WebApp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor          # Layout principal
│   │   ├── MainLayout.razor.css
│   │   ├── NavMenu.razor             # Navegación
│   │   └── README.md
│   │
│   ├── Shared/
│   │   ├── ErrorBoundary.razor       # Manejo de errores
│   │   ├── LoadingSpinner.razor
│   │   ├── ConfirmationDialog.razor
│   │   ├── AlertComponent.razor
│   │   ├── PaginationComponent.razor
│   │   └── README.md
│   │
│   ├── Tickets/
│   │   ├── TicketList.razor          # Listar tickets
│   │   ├── TicketDetail.razor        # Ver detalle
│   │   ├── TicketForm.razor          # Crear/editar
│   │   ├── TicketClassifyModal.razor
│   │   ├── TicketResolveModal.razor
│   │   └── README.md
│   │
│   ├── Inquilinos/
│   │   ├── InquilinoSettings.razor
│   │   ├── IAPrivacyToggle.razor
│   │   └── README.md
│   │
│   ├── Usuarios/
│   │   ├── UserList.razor
│   │   ├── UserForm.razor
│   │   └── README.md
│   │
│   ├── Categorias/
│   │   ├── CategoryList.razor
│   │   ├── CategoryForm.razor
│   │   └── README.md
│   │
│   ├── RiesgosOperacionales/
│   │   ├── RiskList.razor
│   │   ├── RiskForm.razor
│   │   └── README.md
│   │
│   └── README.md
│
├── Pages/
│   ├── Dashboard.razor               # Página principal
│   ├── Tickets.razor                 # Listado de tickets
│   ├── CreateTicket.razor            # Crear ticket
│   ├── TicketDetails.razor           # Detalle de ticket
│   ├── Users.razor                   # Gestión de usuarios
│   ├── Categories.razor              # Gestión de categorías
│   ├── Settings.razor                # Configuración del inquilino
│   ├── Admin.razor                   # Panel de administración
│   ├── Login.razor                   # Autenticación
│   ├── NotFound.razor
│   ├── Error.razor
│   └── README.md
│
├── Services/
│   ├── AuthService.cs                # Autenticación y JWT
│   ├── TicketService.cs              # Llamadas API de Tickets
│   ├── InquilinoService.cs           # Llamadas API de Inquilinos
│   ├── UsuarioService.cs             # Llamadas API de Usuarios
│   ├── CategoriaService.cs           # Llamadas API de Categorías
│   ├── HttpClientService.cs          # HTTP client base
│   ├── NotificationService.cs        # Notificaciones/toasts
│   ├── CacheService.cs               # Caché local
│   ├── PermissionService.cs          # Validación de permisos
│   ├── StateContainers/
│   │   ├── AppState.cs               # Estado global de la app
│   │   ├── UserState.cs
│   │   ├── TenantState.cs
│   │   └── README.md
│   └── README.md
│
├── Models/
│   ├── DTOs/
│   │   ├── TicketDto.cs
│   │   ├── CreateTicketDto.cs
│   │   ├── UsuarioDto.cs
│   │   ├── CategoriaDto.cs
│   │   └── README.md
│   ├── ViewModels/
│   │   ├── TicketViewModel.cs
│   │   ├── TicketFilterViewModel.cs
│   │   └── README.md
│   └── README.md
│
├── wwwroot/
│   ├── css/
│   │   ├── bootstrap.min.css         # Bootstrap
│   │   ├── custom.css                # Estilos personalizados
│   │   └── README.md
│   ├── js/
│   │   ├── bootstrap.bundle.min.js
│   │   ├── interop.js                # JS interop
│   │   └── README.md
│   └── images/
│       └── logo.png
│
├── Shared/
│   ├── Constants.cs
│   ├── Extensions.cs
│   └── README.md
│
├── App.razor                         # Componente raíz
├── Program.cs                        # Configuración
├── appsettings.json
├── appsettings.Development.json
├── _Imports.razor                    # Directivas globales
├── _Host.cshtml                      # Host HTML
└── README.md                         (Este archivo)
```

## Patrón de Componentes

### Componente Simple

```razor
@* Tickets/TicketList.razor *@
@page "/tickets"
@attribute [Authorize]
@inject TicketService TicketService
@inject NavigationManager Navigation
@inject NotificationService Notification

<div class="container-fluid mt-4">
	<div class="d-flex justify-content-between mb-3">
		<h1>Mis Tickets</h1>
		<button class="btn btn-primary" @onclick="GoToCreateTicket">
			Crear Nuevo Ticket
		</button>
	</div>

	@if (isLoading)
	{
		<LoadingSpinner />
	}
	else if (tickets == null || !tickets.Any())
	{
		<div class="alert alert-info">
			No hay tickets registrados.
		</div>
	}
	else
	{
		<div class="table-responsive">
			<table class="table table-hover">
				<thead class="table-dark">
					<tr>
						<th>ID</th>
						<th>Título</th>
						<th>Estado</th>
						<th>Prioridad</th>
						<th>Fecha Creación</th>
						<th>Acciones</th>
					</tr>
				</thead>
				<tbody>
					@foreach (var ticket in tickets)
					{
						<tr>
							<td>@ticket.Id</td>
							<td>@ticket.Titulo</td>
							<td>
								<GetStateBadge State="ticket.Estado" />
							</td>
							<td>
								<GetPriorityBadge Priority="ticket.Prioridad" />
							</td>
							<td>@ticket.CreatedAt.ToString("dd/MM/yyyy HH:mm")</td>
							<td>
								<button class="btn btn-sm btn-info" 
									@onclick="() => GoToDetailTicket(ticket.Id)">
									Ver
								</button>
							</td>
						</tr>
					}
				</tbody>
			</table>
		</div>
	}
</div>

@code {
	private List<TicketDto> tickets;
	private bool isLoading = true;

	protected override async Task OnInitializedAsync()
	{
		try
		{
			tickets = await TicketService.GetTicketsAsync();
		}
		catch (Exception ex)
		{
			Notification.ShowError($"Error al cargar tickets: {ex.Message}");
		}
		finally
		{
			isLoading = false;
		}
	}

	private void GoToCreateTicket()
	{
		Navigation.NavigateTo("/create-ticket");
	}

	private void GoToDetailTicket(Guid ticketId)
	{
		Navigation.NavigateTo($"/tickets/{ticketId}");
	}
}
```

### Componente con Formulario

```razor
@* Tickets/TicketForm.razor *@
@page "/create-ticket"
@page "/edit-ticket/{TicketId:guid}"
@attribute [Authorize]
@inject TicketService TicketService
@inject NavigationManager Navigation
@inject NotificationService Notification

<div class="container mt-4">
	<h1>@(TicketId == Guid.Empty ? "Crear" : "Editar") Ticket</h1>

	<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
		<DataAnnotationsValidator />
		<ValidationSummary />

		<div class="form-group">
			<label for="titulo">Título</label>
			<InputText id="titulo" class="form-control" 
				@bind-Value="model.Titulo" />
			<ValidationMessage For="@(() => model.Titulo)" />
		</div>

		<div class="form-group">
			<label for="descripcion">Descripción</label>
			<InputTextArea id="descripcion" class="form-control" rows="5"
				@bind-Value="model.Descripcion" />
			<ValidationMessage For="@(() => model.Descripcion)" />
		</div>

		<div class="form-group">
			<button type="submit" class="btn btn-primary" disabled="@isSaving">
				@if (isSaving)
				{
					<span class="spinner-border spinner-border-sm me-2" 
						role="status" aria-hidden="true"></span>
				}
				Guardar
			</button>
			<button type="button" class="btn btn-secondary" 
				@onclick="@(() => Navigation.NavigateTo("/tickets"))">
				Cancelar
			</button>
		</div>
	</EditForm>
</div>

@code {
	[Parameter]
	public Guid TicketId { get; set; }

	private CreateTicketDto model = new();
	private bool isSaving = false;

	protected override async Task OnInitializedAsync()
	{
		if (TicketId != Guid.Empty)
		{
			// Cargar ticket para edición
			var ticket = await TicketService.GetTicketAsync(TicketId);
			model.Titulo = ticket.Titulo;
			model.Descripcion = ticket.DescripcionOriginal;
		}
	}

	private async Task HandleSubmit()
	{
		isSaving = true;
		try
		{
			if (TicketId == Guid.Empty)
			{
				// Crear
				var result = await TicketService.CreateTicketAsync(model);
				Notification.ShowSuccess("Ticket creado exitosamente");
				Navigation.NavigateTo($"/tickets/{result.Id}");
			}
			else
			{
				// Editar (si aplica)
				Notification.ShowSuccess("Ticket actualizado exitosamente");
				Navigation.NavigateTo("/tickets");
			}
		}
		catch (Exception ex)
		{
			Notification.ShowError($"Error: {ex.Message}");
		}
		finally
		{
			isSaving = false;
		}
	}
}
```

## Services

### TicketService

```csharp
@* Services/TicketService.cs *@
public class TicketService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<TicketService> _logger;

	public TicketService(HttpClient httpClient, ILogger<TicketService> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<List<TicketDto>> GetTicketsAsync()
	{
		try
		{
			var response = await _httpClient.GetAsync("api/v1/tickets");
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsAsync<List<TicketDto>>();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting tickets");
			throw;
		}
	}

	public async Task<TicketDto> GetTicketAsync(Guid ticketId)
	{
		var response = await _httpClient.GetAsync($"api/v1/tickets/{ticketId}");
		response.EnsureSuccessStatusCode();

		return await response.Content.ReadAsAsync<TicketDto>();
	}

	public async Task<TicketDto> CreateTicketAsync(CreateTicketDto dto)
	{
		var response = await _httpClient.PostAsJsonAsync("api/v1/tickets", dto);
		response.EnsureSuccessStatusCode();

		return await response.Content.ReadAsAsync<TicketDto>();
	}
}
```

### AuthService

```csharp
@* Services/AuthService.cs *@
public class AuthService
{
	private readonly HttpClient _httpClient;
	private readonly LocalStorageService _localStorage;

	public async Task<LoginResponse> LoginAsync(string email, string password)
	{
		var request = new { email, password };
		var response = await _httpClient.PostAsJsonAsync("api/v1/auth/login", request);

		if (response.IsSuccessStatusCode)
		{
			var result = await response.Content.ReadAsAsync<LoginResponse>();

			// Guardar JWT
			await _localStorage.SetItemAsync("authToken", result.Token);

			// Actualizar header
			_httpClient.DefaultRequestHeaders.Authorization = 
				new AuthenticationHeaderValue("Bearer", result.Token);

			return result;
		}

		throw new UnauthorizedAccessException("Credenciales inválidas");
	}
}
```

## Program.cs

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient
builder.Services.AddScoped(sp =>
	new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "") });

// Servicios
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<InquilinoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<NotificationService>();

// Autenticación
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Bootstrap
builder.Services.AddBootstrapBlazor();

await builder.Build().RunAsync();
```

## Autenticación

```razor
@* Components/Layout/NavMenu.razor *@
@inject AuthService AuthService
@inject NavigationManager Navigation

<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
	<div class="container-fluid">
		<a class="navbar-brand" href="/">PIGI-PT</a>

		<AuthorizeView>
			<Authorized>
				<button class="navbar-toggler" type="button">
					<span class="navbar-toggler-icon"></span>
				</button>
				<div class="collapse navbar-collapse">
					<ul class="navbar-nav ms-auto">
						<li class="nav-item">
							<a class="nav-link" href="/tickets">Tickets</a>
						</li>
						<li class="nav-item">
							<a class="nav-link" href="/settings">Configuración</a>
						</li>
						<AuthorizeView Roles="Admin">
							<li class="nav-item">
								<a class="nav-link" href="/admin">Administración</a>
							</li>
						</AuthorizeView>
						<li class="nav-item">
							<button class="btn btn-link nav-link" 
								@onclick="HandleLogout">Salir</button>
						</li>
					</ul>
				</div>
			</Authorized>
			<NotAuthorized>
				<a href="/login" class="btn btn-primary">Iniciar Sesión</a>
			</NotAuthorized>
		</AuthorizeView>
	</div>
</nav>

@code {
	private async Task HandleLogout()
	{
		await AuthService.LogoutAsync();
		Navigation.NavigateTo("/login", forceLoad: true);
	}
}
```

## Mejores Prácticas

1. **Componentes Pequeños**: Reutilizables y testeables
2. **Services para Lógica**: No en componentes
3. **DTOs**: Usar objetos transferibles, no entities
4. **Autenticación**: JWT con LocalStorage
5. **Validación**: EditForm + Validators
6. **Error Handling**: Try/catch con notificaciones
7. **Loading States**: Mostrar spinners durante operaciones async

## Próximas Mejoras

- [ ] Implementar state management (Redux-like)
- [ ] Agregar offline support
- [ ] Implementar pwa
- [ ] Agregar temas (light/dark)
- [ ] Crear storybook para componentes
- [ ] Implementar virtualization para listas grandes
