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
        public async Task<IActionResult> Index()
        {
            // Total de clientes com role "Cliente"
            var clientes = await _userManager.GetUsersInRoleAsync("Cliente");

            // Total de compras
            var totalCompras = await _context.Compras.CountAsync();

            // Soma total das vendas
            var totalVendas = await _context.Compras
                .Where(c => c.Status == StatusCompra.Pago)
                .SumAsync(c => (decimal?)c.ValorPago) ?? 0;

            // Últimas 10 compras com dados do plano e cliente incluídos
            var ultimasCompras = await _context.Compras
                .Include(c => c.Plano)
                .Include(c => c.Cliente)
                .OrderByDescending(c => c.DataCompra)
                .Take(10)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalClientes = clientes.Count,
                TotalCompras = totalCompras,
                TotalVendas = totalVendas,
                UltimasCompras = ultimasCompras
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