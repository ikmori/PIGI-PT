using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PIGI_PT_WebApp.Services
{
    /// <summary>
    /// Manejador HTTP que intercepta todas las peticiones salientes y adjunta 
    /// de manera automática el Token JWT del usuario autenticado en la cabecera Authorization.
    /// </summary>
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public JwtAuthorizationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Resolver IAuthService bajo demanda para romper dependencias circulares (HttpClient -> Handler -> AuthService -> HttpClient)
            var authService = _serviceProvider.GetRequiredService<IAuthService>();
            var token = authService.GetJwtToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
