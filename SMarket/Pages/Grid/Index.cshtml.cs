using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SMarket.Core.Data;
using SMarket.Core.Model;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace SMarket.Pages.Grid
{
    public class IndexModel : PageModel
    {
        private readonly SMarketContext _context;
        private readonly IHostingEnvironment _hostEnvironment;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 1000;

        [BindProperty]
        public string Filtro { get; set; }


        [BindProperty]
        public IFormFile FileSMarket { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public IndexModel(SMarketContext context,
                            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostEnvironment = hostingEnvironment;
        }

        public IList<Itens> ListaItens { get; set; } = new List<Itens>();

        public async Task<IActionResult> OnGetAsync()
        {
            await CarregarItens();
            return Page();
        }

        public async Task<IActionResult> OnPostFiltroAsync()
        {
            await CarregarItens();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateFileAsync()
        {
            var nameFile = $"{Guid.NewGuid()}.txt";
            var savePath = Path.Combine(_hostEnvironment.WebRootPath, "arquivo", nameFile);

            var newFile = System.IO.File.CreateText(savePath);

            var list = await _context
                                .Itens
                                .ToListAsync()
                                .ConfigureAwait(false);

            var lineFirst = "id".PadRight(5) +
                                "|" +
                                "nome".PadRight(60) +
                                "|" +
                                "precoCusto".PadRight(10) +
                                "|" +
                                "precoVenda".PadRight(10) +
                                "|" +
                                "ncm".PadRight(8) +
                                "|" +
                                "referencia".PadRight(14) +
                                "|" +
                                "dataCriacao".PadRight(19);


            newFile.WriteLine(lineFirst);

            foreach (var item in list)
            {
                var referencia = item.Referencia == null ? "" : item.Referencia;

                var line = $"{item.Id.ToString().PadRight(5)}|"
                                + $"{item.Nome.PadRight(60)}|"
                                + $"{item.PrecoCusto.ToString().PadRight(10)}|"
                                + $"{item.PrecoVenda.ToString().PadRight(10)}|"
                                + $"{item.Ncm.PadRight(8)}|"
                                + $"{referencia.PadRight(14)}|"
                                + $"{item.DataCadastro.ToString().PadRight(19)}";

                newFile.WriteLine(line);

                _context
                    .Itens
                    .Remove(item);
            }

            newFile.Close();

            await _context
                    .SaveChangesAsync()
                    .ConfigureAwait(false);

            await CarregarItens()
                    .ConfigureAwait(false);

            byte[] stream = System.IO.File.ReadAllBytes(savePath);

            return File(stream, "text/plain ", "Report.txt");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (FileSMarket != null)
            {
                var nameFile = Guid.NewGuid().ToString() + Path.GetExtension(FileSMarket.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "Arquivos", nameFile);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    FileSMarket.CopyTo(stream);
                }

                await CarregarArquivo(nameFile)
                    .ConfigureAwait(false);
            }

            await CarregarItens()
                    .ConfigureAwait(false);

            return Page();
        }

        public async Task CarregarArquivo(string nameFile)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "Arquivos", nameFile);

            var fileLines = new System.IO.StreamReader(file);
            string line = string.Empty;
            var countLine = 0;

            while ((line = fileLines.ReadLine()) != null)
            {
                try
                {
                    if (countLine > 0 && !string.IsNullOrEmpty(line))
                    {
                        var parametros = line.Split("|");

                        var item = new Itens() {
                            Nome = parametros[1].TrimEnd(),
                            PrecoCusto = decimal.Parse(parametros[2].TrimEnd()),
                            PrecoVenda = decimal.Parse(parametros[3].TrimEnd()),
                            Ncm = parametros[4].TrimEnd(),
                            Referencia = parametros[5].TrimEnd(),
                            DataCadastro = DateTime.Parse(parametros[6].TrimEnd())
                        };

                        await _context
                                .Itens
                                .AddAsync(item)
                                .ConfigureAwait(false);

                    }
                }
                catch
                {
                    var log = new Logs($"Erro ao importar linha {countLine}, pois está com o formato incorreto");

                    await _context
                            .Logs
                            .AddAsync(log)
                            .ConfigureAwait(false);
                }

                countLine++;
            }

            await _context
                    .SaveChangesAsync()
                    .ConfigureAwait(false);

            fileLines.Close();
        }

        public async Task CarregarItens()
        {
            if (string.IsNullOrEmpty(Filtro))
            {
                ListaItens = await _context.Itens
                    .OrderBy(d => d.Nome)
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                Count = await _context.Itens.CountAsync();
            }
            else
            {
                ListaItens = await _context.Itens
                    .Where(c => c.Nome.Contains(Filtro) || c.Ncm.Contains(Filtro) || c.Referencia.Contains(Filtro))
                    .OrderBy(d => d.Nome)
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                Count = await _context.Itens.Where(c => c.Nome.Contains(Filtro)).CountAsync();
            }
        }
    }
}
