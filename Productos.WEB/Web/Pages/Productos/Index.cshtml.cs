using Productos.Abstractions.Interfaces.Reglas;
using Productos.Abstractions.Modelos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;

namespace Productos.Web.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public IList<ProductoResponse> Productos { get; set; } = new List<ProductoResponse>();

        public IndexModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task OnGetAsync()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProductos");
            using var cliente = new HttpClient();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            using var respuesta = await cliente.SendAsync(solicitud);
            if (respuesta.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Productos = JsonSerializer.Deserialize<List<ProductoResponse>>(resultado, opciones) ?? new List<ProductoResponse>();
        }
    }
}
