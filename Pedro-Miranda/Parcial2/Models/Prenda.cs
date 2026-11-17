namespace Parcial2.Models
{
    // Una prenda o servicio de la lista de precios del lavadero. Se lee de Datos/prendas.json.
    public class Prenda : IEntidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Categoria { get; set; } = "";
        public decimal Precio { get; set; }

        public override string ToString()
        {
            return $"{Nombre} ({Precio:C0})";
        }
    }
}
