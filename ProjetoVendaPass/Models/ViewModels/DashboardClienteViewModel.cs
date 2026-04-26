using X.PagedList;

namespace ProjetoVendaPass.Models.ViewModels
{
    public class DashboardClienteViewModel
    {
        public List<Compra> Compras { get; set; } = new();
        public string NomeCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public int TotalCompras { get; set; }
        public decimal TotalGasto { get; set; }
        public Compra? UltimaCompra { get; set; }
        public IPagedList<Compra> ComprasRecentes { get; set; } = null!;

        // Filtros
        public string? FiltroStatus { get; set; }
        public DateTime? FiltroDataInicio { get; set; }
        public DateTime? FiltroDataFim { get; set; }
        public string? FiltroPlano { get; set; }

    }
}