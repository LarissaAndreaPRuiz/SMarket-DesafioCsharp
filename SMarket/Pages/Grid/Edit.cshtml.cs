using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SMarket.Core.Data;
using SMarket.Core.Model;

namespace SMarket.Pages.Grid
{
    public class EditModel : PageModel
    {
        private readonly SMarketContext _context;

        public EditModel(SMarketContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Itens Item { get; set; }
        public string Mensagem { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Item = await _context
                            .Itens
                            .FirstOrDefaultAsync(m => m.Id == id);

            if (Item == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            Mensagem = $"Item {Item.Nome} salvo com sucesso";

            return RedirectToPage("./Index");
        }
    }
}
