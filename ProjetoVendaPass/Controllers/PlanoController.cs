using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoVendaPass.Data;
using ProjetoVendaPass.Models;

namespace ProjetoVendaPass.Controllers
{
    [Authorize(Roles = "Admin")] // apenas usuários Admin acessam
    public class PlanoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Plano
        public async Task<IActionResult> Index()
        {
            var planos = await _context.Planos.ToListAsync();
            return View(planos);
        }

        // GET: /Plano/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Plano/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plano plano)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plano);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plano);
        }

        // GET: /Plano/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plano = await _context.Planos.FindAsync(id);
            if (plano == null) return NotFound();

            return View(plano);
        }

        // POST: /Plano/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Plano plano)
        {
            if (id != plano.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plano);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Planos.Any(p => p.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(plano);
        }

        // GET: /Plano/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var plano = await _context.Planos.FirstOrDefaultAsync(p => p.Id == id);
            if (plano == null) return NotFound();

            return View(plano);
        }

        // POST: /Plano/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plano = await _context.Planos.FindAsync(id);
            if (plano != null)
            {
                _context.Planos.Remove(plano);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}