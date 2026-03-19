using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Productos.Abstractions.Interfaces.Reglas;
using Productos.Abstractions.Modelos;
using System.Net;
using System.Text.Json;

namespace Productos.Web.Pages.Productos
{
    public class AgregarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        [BindProperty]
        public ProductoFormulario Producto { get; set; } = new();

        public List<SelectListItem> Categorias { get; set; } = new();
        public List<SelectListItem> SubCategorias { get; set; } = new();

        public AgregarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task OnGetAsync()
        {
            await CargarCategoriasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarCombosAsync();
                return Page();
            }

            if (!TryConstruirRequest(out var request))
            {
                await CargarCombosAsync();
                return Page();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "AgregarProducto");
            using var cliente = new HttpClient();

            var respuesta = await cliente.PostAsJsonAsync(endpoint, request);
            respuesta.EnsureSuccessStatusCode();

            return RedirectToPage("./Index");
        }

        public async Task<JsonResult> OnGetObtenerSubCategoriasAsync(Guid categoriaId)
        {
            var subCategorias = await ObtenerSubCategoriasAsync(categoriaId);
            return new JsonResult(subCategorias);
        }

        private bool TryConstruirRequest(out ProductoRequest request)
        {
            request = default!;

            if (!Producto.IdCategoria.HasValue || Producto.IdCategoria.Value == Guid.Empty)
            {
                ModelState.AddModelError("Producto.IdCategoria", "Debes seleccionar una categoria.");
                return false;
            }

            if (!Producto.IdSubCategoria.HasValue || Producto.IdSubCategoria.Value == Guid.Empty)
            {
                ModelState.AddModelError("Producto.IdSubCategoria", "Debes seleccionar una subcategoria.");
                return false;
            }

            request = new ProductoRequest
            {
                IdSubCategoria = Producto.IdSubCategoria.Value,
                Nombre = Producto.Nombre,
                Descripcion = Producto.Descripcion,
                Precio = Producto.Precio,
                Stock = Producto.Stock,
                CodigoBarras = Producto.CodigoBarras
            };

            return true;
        }

        private async Task CargarCombosAsync()
        {
            await CargarCategoriasAsync();

            if (Producto.IdCategoria.HasValue && Producto.IdCategoria.Value != Guid.Empty)
            {
                await CargarSubCategoriasAsync(Producto.IdCategoria.Value);
            }
        }

        private async Task CargarCategoriasAsync()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerCategorias");
            using var cliente = new HttpClient();
            using var respuesta = await cliente.GetAsync(endpoint);

            if (respuesta.StatusCode == HttpStatusCode.NoContent)
            {
                Categorias = new List<SelectListItem>();
                return;
            }

            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var categorias = JsonSerializer.Deserialize<List<Categoria>>(resultado, opciones) ?? new List<Categoria>();

            Categorias = categorias.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre,
                Selected = Producto.IdCategoria == c.Id
            }).ToList();
        }

        private async Task CargarSubCategoriasAsync(Guid idCategoria)
        {
            var subCategorias = await ObtenerSubCategoriasAsync(idCategoria);

            SubCategorias = subCategorias.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Nombre,
                Selected = Producto.IdSubCategoria == s.Id
            }).ToList();
        }

        private async Task<List<SubCategoria>> ObtenerSubCategoriasAsync(Guid idCategoria)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerSubCategorias");
            using var cliente = new HttpClient();
            using var respuesta = await cliente.GetAsync(string.Format(endpoint, idCategoria));

            if (respuesta.StatusCode == HttpStatusCode.NoContent)
            {
                return new List<SubCategoria>();
            }

            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<SubCategoria>>(resultado, opciones) ?? new List<SubCategoria>();
        }
    }
}
