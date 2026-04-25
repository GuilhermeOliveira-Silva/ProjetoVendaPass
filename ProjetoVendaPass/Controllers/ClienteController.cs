using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoVendaPass.Data;
using ProjetoVendaPass.Models;
using ProjetoVendaPass.Models.ViewModels;

namespace ProjetoVendaPass.Controllers
{
    [Authorize] // exige login em todo o controller
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClienteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Cliente/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Pega o usuário logado
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return Unauthorized();

            // Busca apenas as compras do usuário logado
            var compras = await _context.Compras
                .Include(c => c.Plano)
                .Where(c => c.ClienteId == usuario.Id)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            // ViewModel
            var viewModel = new DashboardClienteViewModel
            {
                Compras = compras,
                TotalGasto = compras.Sum(c => c.ValorPago),
                UltimaCompra = compras.FirstOrDefault()
            };

            return View(viewModel);
        }
    }
}