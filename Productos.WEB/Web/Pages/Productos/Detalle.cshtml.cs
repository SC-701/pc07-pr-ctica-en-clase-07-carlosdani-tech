using Productos.Abstractions.Interfaces.Reglas;
using Productos.Abstractions.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;

namespace Productos.Web.Pages.Productos
{
    public class DetalleModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public ProductoResponse Producto { get; set; } = default!;

        public DetalleModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProducto");
            using var cliente = new HttpClient();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));

            using var respuesta = await cliente.SendAsync(solicitud);
            if (respuesta.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var producto = JsonSerializer.Deserialize<ProductoResponse>(resultado, opciones);
            if (producto == null)
            {
                return NotFound();
            }

            Producto = producto;

            return Page();
        }
    }
}
