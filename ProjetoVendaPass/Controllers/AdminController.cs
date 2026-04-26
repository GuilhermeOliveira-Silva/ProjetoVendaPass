using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoVendaPass.Data;
using ProjetoVendaPass.Models;
using ProjetoVendaPass.Models.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;

namespace ProjetoVendaPass.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Admin
        public async Task<IActionResult> Index(
     string? filtroCliente,
     string? filtroStatus,
     DateTime? filtroDataInicio,
     DateTime? filtroDataFim,
     decimal? filtroValorMin,
     decimal? filtroValorMax,
     int pagina = 1)
        {
            var clientes = await _userManager.GetUsersInRoleAsync("Cliente");
            var totalCompras = await _context.Compras.CountAsync();
            var totalVendas = await _context.Compras
                .Where(c => c.Status == StatusCompra.Pago)
                .SumAsync(c => (decimal?)c.ValorPago) ?? 0;

            // Query base
            var query = _context.Compras
                .Include(c => c.Plano)
                .Include(c => c.Cliente)
                .AsQueryable();

            // Filtro por cliente (nome ou email)
            if (!string.IsNullOrEmpty(filtroCliente))
                query = query.Where(c =>
                    c.Cliente!.Email!.Contains(filtroCliente) ||
                    c.Cliente!.Nome.Contains(filtroCliente));

            // Filtro por status
            if (!string.IsNullOrEmpty(filtroStatus) && Enum.TryParse<StatusCompra>(filtroStatus, out var status))
                query = query.Where(c => c.Status == status);

            // Filtro por data
            if (filtroDataInicio.HasValue)
                query = query.Where(c => c.DataCompra >= filtroDataInicio.Value);
            if (filtroDataFim.HasValue)
                query = query.Where(c => c.DataCompra <= filtroDataFim.Value.AddDays(1));

            // Filtro por valor
            if (filtroValorMin.HasValue)
                query = query.Where(c => c.ValorPago >= filtroValorMin.Value);
            if (filtroValorMax.HasValue)
                query = query.Where(c => c.ValorPago <= filtroValorMax.Value);

            var lista = await query
              .OrderByDescending(c => c.DataCompra)
              .ToListAsync();

            var comprasPaginadas = lista.ToPagedList(pagina, 10);

            var viewModel = new DashboardViewModel
            {
                TotalClientes = clientes.Count,
                TotalCompras = totalCompras,
                TotalVendas = totalVendas,
                UltimasCompras = comprasPaginadas,
                FiltroCliente = filtroCliente,
                FiltroStatus = filtroStatus,
                FiltroDataInicio = filtroDataInicio,
                FiltroDataFim = filtroDataFim,
                FiltroValorMin = filtroValorMin,
                FiltroValorMax = filtroValorMax
            };

            return View(viewModel);
        }

        // GET: /Admin/DashboardCliente/id
        public async Task<IActionResult> DashboardCliente(string id)
        {
            // Valida se o cliente existe
            var cliente = await _userManager.FindByIdAsync(id);
            if (cliente == null)
                return NotFound();

            // Garante que é realmente um cliente e não outro admin
            if (!await _userManager.IsInRoleAsync(cliente, "Cliente"))
                return NotFound();

            // Busca todas as compras do cliente
            var compras = await _context.Compras
                .Include(c => c.Plano)
                .Where(c => c.ClienteId == id)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            var viewModel = new DashboardClienteViewModel
            {
                NomeCliente = cliente.Nome,
                EmailCliente = cliente.Email ?? string.Empty,
                TotalCompras = compras.Count,
                TotalGasto = compras.Sum(c => c.ValorPago),
                UltimaCompra = compras.FirstOrDefault(),
                ComprasRecentes = compras.ToPagedList(1, 10)
            };

            return View(viewModel);
        }

        // GET: /Admin/Clientes
        public async Task<IActionResult> Clientes()
        {
            // Busca apenas usuários com role "Cliente"
            var clientes = await _userManager.GetUsersInRoleAsync("Cliente");

            return View(clientes);
        }
    }
}