using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoVendaPass.Data;
using ProjetoVendaPass.Models;

namespace ProjetoVendaPass.Controllers
{
    public class LojaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LojaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Loja — público
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var planos = await _context.Planos
                .Where(p => p.Ativo)
                .OrderBy(p => p.Preco)
                .ToListAsync();

            return View(planos);
        }

        // POST: /Loja/Comprar
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comprar(int planoId)
        {
            var plano = await _context.Planos.FindAsync(planoId);
            if (plano == null || !plano.Ativo)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId == null)
                return Unauthorized();

            var compra = new Compra
            {
                ClienteId = usuarioId,
                PlanoId = plano.Id,
                ValorPago = plano.Preco,
                DataCompra = DateTime.Now,
                Status = StatusCompra.Pendente
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pagamento), new { id = compra.Id });
        }


        // GET: /Loja/MinhasCompras — histórico do cliente
        [Authorize]
        public async Task<IActionResult> MinhasCompras()
        {
            var usuarioId = _userManager.GetUserId(User);

            var compras = await _context.Compras
                .Include(c => c.Plano)
                .Where(c => c.ClienteId == usuarioId)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            return View(compras);
        }

        // GET: /Loja/Detalhes/5
        [Authorize]
        public async Task<IActionResult> Detalhes(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            // Busca a compra já filtrando pelo dono e incluindo o plano
            var compra = await _context.Compras
                .Include(c => c.Plano)
                .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == usuarioId);

            // Se não existe OU não pertence ao usuário → 404
            if (compra == null)
                return NotFound();

            return View(compra);
        }
         
        // GET: /Loja/Pagamento/5
        [Authorize]
        public async Task<IActionResult> Pagamento(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var compra = await _context.Compras
                .Include(c => c.Plano)
                .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == usuarioId);

            if (compra == null)
                return NotFound();

            // Se já foi pago, vai direto para confirmação
            if (compra.Status == StatusCompra.Pago)
                return RedirectToAction(nameof(Confirmacao), new { id = compra.Id });

            return View(compra);
        }

        // POST: /Loja/ConfirmarPagamento/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPagamento(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var compra = await _context.Compras
                .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == usuarioId);

            if (compra == null)
                return NotFound();

            // Simula a aprovação do pagamento
            compra.Status = StatusCompra.Pago;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmacao), new { id = compra.Id });
        }

        // GET: /Loja/Confirmacao/5
        [Authorize]
        public async Task<IActionResult> Confirmacao(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var compra = await _context.Compras
                .Include(c => c.Plano) // ← obrigatório
                .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == usuarioId);

            if (compra == null)
                return NotFound();

            return View(compra);
        }

        //expira compras pendentes após 72 horas
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