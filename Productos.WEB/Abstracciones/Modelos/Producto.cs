using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Productos.Abstractions.Modelos
{
    public class ProductoBase
    {
        [Required(ErrorMessage = "La propiedad nombre es requerida")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "La propiedad nombre debe tener entre 10 y 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La propiedad descripcion es requerida")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "La propiedad descripcion debe tener entre 10 y 200 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La propiedad precio es requerida")]
        [Range(typeof(decimal), "0,01", "9999999999", ErrorMessage = "La propiedad precio debe ser mayor a 0")]
        [DisplayName("Precio CRC")]
        [JsonPropertyOrder(1)]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La propiedad stock es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La propiedad stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La propiedad codigoBarras es requerida")]
        [RegularExpression(@"^\d{8,14}$", ErrorMessage = "El Codigo de Barras debe contener solo digitos y tener entre 8 y 14 caracteres")]
        [DisplayName("Codigo de barras")]
        public string CodigoBarras { get; set; } = string.Empty;
    }

    public class ProductoRequest : ProductoBase
    {
        public Guid IdSubCategoria { get; set; }
    }

    public class ProductoDetalle : ProductoBase
    {
        [DisplayName("Precio USD")]
        [JsonPropertyOrder(2)]
        public decimal PrecioUSD { get; set; }
    }

    public class ProductoResponse : ProductoDetalle
    {
        public Guid Id { get; set; }
        public Guid IdSubCategoria { get; set; }
        public Guid IdCategoria { get; set; }

        [DisplayName("Subcategoria")]
        public string SubCategoria { get; set; } = string.Empty;

        [DisplayName("Categoria")]
        public string Categoria { get; set; } = string.Empty;
    }

    public class ProductoFormulario : ProductoBase
    {
        [DisplayName("Categoria")]
        public Guid? IdCategoria { get; set; }

        [DisplayName("Subcategoria")]
        public Guid? IdSubCategoria { get; set; }
    }
}
