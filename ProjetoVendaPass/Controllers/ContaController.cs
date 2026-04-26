using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoVendaPass.Data;
using ProjetoVendaPass.Models;
using ProjetoVendaPass.Models.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;

namespace ProjetoVendaPass.Controllers
{
    [Authorize]
    public class ContaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ContaController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Conta
        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return NotFound();

            var viewModel = new MinhaContaViewModel
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                DiscordId = usuario.DiscordId
            };

            return View(viewModel);
        }

        // POST: /Conta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MinhaContaViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return NotFound();

            usuario.Nome = viewModel.Nome;
            usuario.DiscordId = viewModel.DiscordId;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                TempData["Sucesso"] = "Dados atualizados com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var erro in resultado.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);

            return View(viewModel);
        }

        // GET: /Conta/AlterarSenha
        public IActionResult AlterarSenha()
        {
            return View();
        }

        // POST: /Conta/AlterarSenha
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return NotFound();

            var resultado = await _userManager.ChangePasswordAsync(
                usuario,
                viewModel.SenhaAtual,
                viewModel.NovaSenha
            );

            if (resultado.Succeeded)
            {
                TempData["Sucesso"] = "Senha alterada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var erro in resultado.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);

            return View(viewModel);
        }

// GET: /Conta/Dashboard
public async Task<IActionResult> Dashboard(
    string? filtroStatus,
    DateTime? filtroDataInicio,
    DateTime? filtroDataFim,
    string? filtroPLano,
    int pagina = 1) // ← parâmetro de página
{
    var usuarioId = _userManager.GetUserId(User);

    await ExpirarComprasPendentes(usuarioId);

    var query = _context.Compras
        .Include(c => c.Plano)
        .Where(c => c.ClienteId == usuarioId
                 && c.Status != StatusCompra.Expirado)
        .AsQueryable();

    if (!string.IsNullOrEmpty(filtroStatus) && Enum.TryParse<StatusCompra>(filtroStatus, out var status))
        query = query.Where(c => c.Status == status);

    if (filtroDataInicio.HasValue)
        query = query.Where(c => c.DataCompra >= filtroDataInicio.Value);

    if (filtroDataFim.HasValue)
        query = query.Where(c => c.DataCompra <= filtroDataFim.Value.AddDays(1));

    if (!string.IsNullOrEmpty(filtroPLano))
        query = query.Where(c => c.Plano!.Nome.Contains(filtroPLano));

    query = query.OrderByDescending(c => c.DataCompra);

            // Paginação — 5 por página
            var lista = await query.ToListAsync();
            var comprasPaginadas = lista.ToPagedList(pagina, 5);

            var todasCompras = await _context.Compras
        .Where(c => c.ClienteId == usuarioId)
        .ToListAsync();

    var viewModel = new DashboardClienteViewModel
    {
        TotalCompras     = todasCompras.Count(c => c.Status == StatusCompra.Pago),
        TotalGasto       = todasCompras
                            .Where(c => c.Status == StatusCompra.Pago)
                            .Sum(c => c.ValorPago),
        UltimaCompra     = comprasPaginadas.FirstOrDefault(),
        ComprasRecentes  = comprasPaginadas,
        FiltroStatus     = filtroStatus,
        FiltroDataInicio = filtroDataInicio,
        FiltroDataFim    = filtroDataFim,
        FiltroPlano      = filtroPLano
    };

    return View(viewModel);
}

        // Expira compras pendentes com mais de 72 horas
        private async Task ExpirarComprasPendentes(string usuarioId)
        {
            var limite = DateTime.Now.AddHours(-72);

            var comprasExpiradas = await _context.Compras
                .Where(c => c.ClienteId == usuarioId
                         && c.Status == StatusCompra.Pendente
                         && c.DataCompra < limite)
                .ToListAsync();

            foreach (var compra in comprasExpiradas)
                compra.Status = StatusCompra.Expirado;

            if (comprasExpiradas.Any())
                await _context.SaveChangesAsync();
        }
    }
}