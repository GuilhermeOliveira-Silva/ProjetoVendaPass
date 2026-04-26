namespace ProjetoVendaPass.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalClientes { get; set; }
        public int TotalCompras { get; set; }
        public decimal TotalVendas { get; set; }
        public List<Compra> UltimasCompras { get; set; } = new();
    }
}