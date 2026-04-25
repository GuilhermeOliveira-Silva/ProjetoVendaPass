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

        // GET: /Loja — página pública
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
        [Authorize] // exige login para comprar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comprar(int planoId)
        {
            // 1. Busca o plano
            var plano = await _context.Planos.FindAsync(planoId);
            if (plano == null || !plano.Ativo)
                return NotFound();

            // 2. Pega o usuário logado
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return Unauthorized();

            // 3. Cria a compra com status Pendente
            var compra = new Compra
            {
                ClienteId  = usuario.Id,
                PlanoId    = plano.Id,
                ValorPago  = plano.Preco,
                DataCompra = DateTime.Now,
                Status     = StatusCompra.Pendente
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            // 4. Redireciona para a confirmação passando o id da compra
            return RedirectToAction(nameof(Confirmacao), new { id = compra.Id });
        }

        // GET: /Loja/Confirmacao/5
        [Authorize]
        public async Task<IActionResult> Confirmacao(int id)
        {
            // Busca a compra com os dados do plano incluídos
            var compra = await _context.Compras
                .Include(c => c.Plano)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
                return NotFound();

            // Garante que só o dono da compra veja a confirmação
            var usuario = await _userManager.GetUserAsync(User);
            if (compra.ClienteId != usuario?.Id)
                return Forbid();

            return View(compra);
        }
    }
}