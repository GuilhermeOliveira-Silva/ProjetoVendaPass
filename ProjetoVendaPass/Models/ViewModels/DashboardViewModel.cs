using X.PagedList;

namespace ProjetoVendaPass.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalClientes { get; set; }
        public int TotalCompras { get; set; }
        public decimal TotalVendas { get; set; }
        public IPagedList<Compra> UltimasCompras { get; set; } = null!;

        // Filtros
        public string? FiltroCliente { get; set; }
        public string? FiltroStatus { get; set; }
        public DateTime? FiltroDataInicio { get; set; }
        public DateTime? FiltroDataFim { get; set; }
        public decimal? FiltroValorMin { get; set; }
        public decimal? FiltroValorMax { get; set; }
    }
}