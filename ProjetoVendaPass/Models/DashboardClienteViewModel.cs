namespace ProjetoVendaPass.Models.ViewModels
{
    public class DashboardClienteViewModel
    {
        public List<Compra> Compras { get; set; } = new();
        public decimal TotalGasto { get; set; }
        public Compra? UltimaCompra { get; set; }
    }
}