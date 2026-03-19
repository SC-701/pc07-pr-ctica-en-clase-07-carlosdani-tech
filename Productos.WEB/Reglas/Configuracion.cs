using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Productos.Abstractions.Interfaces.Reglas;
using Productos.Abstractions.Modelos;
using Microsoft.Extensions.Configuration;

namespace Productos.Reglas
{
    public class Configuracion : IConfiguracion
    {
        private IConfiguration _configuracion;

        public Configuracion(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public string ObtenerMetodo(string seccion,string nombre)
        {
            var apiEndPoint = _configuracion.GetSection(seccion).Get<APIEndPoint>()
                ?? throw new InvalidOperationException($"No se encontró la sección '{seccion}'.");

            var urlBase = apiEndPoint.UrlBase
                ?? throw new InvalidOperationException($"No se encontró UrlBase en '{seccion}'.");

            var metodo = apiEndPoint.Metodos?.FirstOrDefault(m => m.Nombre == nombre)?.Valor
                ?? throw new InvalidOperationException($"No se encontró el método '{nombre}' en '{seccion}'.");

            return $"{urlBase.TrimEnd('/')}/{metodo.TrimStart('/')}";
        }

        private string ObtenerUrlBase(string seccion)
        {
            var apiEndPoint = _configuracion.GetSection(seccion).Get<APIEndPoint>()
                ?? throw new InvalidOperationException($"No se encontró la sección '{seccion}'.");

            return apiEndPoint.UrlBase
                ?? throw new InvalidOperationException($"No se encontró UrlBase en '{seccion}'.");
        }
    }
}
