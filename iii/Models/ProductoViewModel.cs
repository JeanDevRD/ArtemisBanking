
namespace ArtemisBankingWebApp.Models
{
    public class ProductoViewModel
    {
        public string Tipo { get; set; } = "";
        public string Numero { get; set; } = "";
        public decimal Monto { get; set; }
        public string Estado { get; set; } = "";
        public DateTime Fecha { get; set; }
        public int Id { get; set; }
        public int TipoId { get; set; } // 1=Cuenta, 2=Préstamo, 3=Tarjeta
    }
}
