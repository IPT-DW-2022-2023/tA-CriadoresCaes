using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CriadorCaes.Data;
using CriadorCaes.Models;

namespace CriadorCaes.Controllers {
   public class AnimaisController : Controller {
      private readonly ApplicationDbContext _bd;

      public AnimaisController(ApplicationDbContext context) {
         _bd=context;
      }

      // GET: Animais
      public async Task<IActionResult> Index() {

         var listaAnimais = _bd.Animais.Include(a => a.Criador).Include(a => a.Raca);

         return View(await listaAnimais.ToListAsync());

      }

      // GET: Animais/Details/5
      public async Task<IActionResult> Details(int? id) {
         if (id==null||_bd.Animais==null) {
            return NotFound();
         }

         var animal = await _bd.Animais
                                .Include(a => a.Criador)
                                .Include(a => a.Raca)
                                .FirstOrDefaultAsync(m => m.Id==id);
         if (animal==null) {
            return NotFound();
         }

         return View(animal);
      }



      // GET: Animais/Create
      /// <summary>
      /// Mostra o formulário de criação de um novo animal
      /// </summary>
      /// <returns></returns>
      public IActionResult Create() {

         // prepara os dados para colocar dados nas dropdowns do formulário
         ViewData["CriadorFK"]=new SelectList(_bd.Criadores, "Id", "Nome");
         ViewData["RacaFK"]=new SelectList(_bd.Racas, "Id", "Nome");

         // invoca a view
         return View();
      }


      // POST: Animais/Create
      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      /// <summary>
      /// Método que irá receber os dados fornecidos pelo Browser
      /// </summary>
      /// <param name="animal">dados do animal a ser inserido</param>
      /// <returns></returns>
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("Id,Nome,DataNascimento,DataCompra,Sexo,NumLOP,CriadorFK,RacaFK")] Animais animal) {

         // Validação final dos dados recebidos do browser
         // só se avança, se forem corretos. Ie, se respeitarem as regras
         // definidas no Model
         if (ModelState.IsValid) {

            try {
               // adicionar os dados do animal à BD
               _bd.Add(animal);
               // efetuar COMMIT dos dados
               await _bd.SaveChangesAsync();
               // redirecionar o utilizador para a página inicial
               return RedirectToAction(nameof(Index));
            }
            catch (Exception) {
               ModelState.AddModelError("", "Ocorreu um erro com a adição dos dados do "+animal.Nome);
              // throw;
            }


         }
         // se chego aqui, é porque os dados não eram válidos
         // devolve-se o controlo à View
         // preparar os dados para as dropdown
         ViewData["CriadorFK"]=new SelectList(_bd.Criadores, "Id", "Nome", animal.CriadorFK);
         ViewData["RacaFK"]=new SelectList(_bd.Racas, "Id", "Nome", animal.RacaFK);
         return View(animal);
      }

      // GET: Animais/Edit/5
      public async Task<IActionResult> Edit(int? id) {
         if (id==null||_bd.Animais==null) {
            return NotFound();
         }

         var animais = await _bd.Animais.FindAsync(id);
         if (animais==null) {
            return NotFound();
         }
         ViewData["CriadorFK"]=new SelectList(_bd.Criadores, "Id", "Email", animais.CriadorFK);
         ViewData["RacaFK"]=new SelectList(_bd.Racas, "Id", "Id", animais.RacaFK);
         return View(animais);
      }

      // POST: Animais/Edit/5
      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,DataNascimento,DataCompra,Sexo,NumLOP,CriadorFK,RacaFK")] Animais animais) {
         if (id!=animais.Id) {
            return NotFound();
         }

         if (ModelState.IsValid) {
            try {
               _bd.Update(animais);
               await _bd.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
               if (!AnimaisExists(animais.Id)) {
                  return NotFound();
               }
               else {
                  throw;
               }
            }
            return RedirectToAction(nameof(Index));
         }
         ViewData["CriadorFK"]=new SelectList(_bd.Criadores, "Id", "Email", animais.CriadorFK);
         ViewData["RacaFK"]=new SelectList(_bd.Racas, "Id", "Id", animais.RacaFK);
         return View(animais);
      }

      // GET: Animais/Delete/5
      public async Task<IActionResult> Delete(int? id) {
         if (id==null||_bd.Animais==null) {
            return NotFound();
         }

         var animais = await _bd.Animais
             .Include(a => a.Criador)
             .Include(a => a.Raca)
             .FirstOrDefaultAsync(m => m.Id==id);
         if (animais==null) {
            return NotFound();
         }

         return View(animais);
      }

      // POST: Animais/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(int id) {
         if (_bd.Animais==null) {
            return Problem("Entity set 'ApplicationDbContext.Animais'  is null.");
         }
         var animais = await _bd.Animais.FindAsync(id);
         if (animais!=null) {
            _bd.Animais.Remove(animais);
         }

         await _bd.SaveChangesAsync();
         return RedirectToAction(nameof(Index));
      }

      private bool AnimaisExists(int id) {
         return _bd.Animais.Any(e => e.Id==id);
      }
   }
}
