using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using ProiectPbd.Data;
using ProiectPbd.DataModels;
using ProiectPbd.Models;
using ProiectPbd.Repositories;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProiectPbd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDbConnection Database;
        private readonly ITableUtils _repo;

        public HomeController(ILogger<HomeController> logger, IDbConnection database, ITableUtils repo)
        {
            Database = database;
            _logger = logger;
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            await _repo.createTablesAsync();
            await _repo.populateTablesAsync();
            return RedirectToAction("Catalog");
        }

        public async Task<IActionResult> Catalog()
        {
            return View(await _repo.getMapCatalog());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Raport(int? type)
        {
            if (type != null)
            {
                if (type == 1)
                {
                    List<Tuple<Nota, Student, Materie>> tuple = new List<Tuple<Nota, Student, Materie>>();
                    List<int> Ani = new List<int>();
                    var studenti = await Database.SelectAsync<Student>();
                    for (int i = 0; i < studenti.Count; i++)
                    {
                        var q = Database.From<Materie>().Join<Materie, Nota>((m, n) => m.Id == n.MaterieId && n.StudentId == studenti[i].Id).Select(x => Sql.Max(x.AnStudiu));
                        var result = await Database.ScalarAsync<int?>(q) ?? 0;
                        Ani.Add(result);
                        Nota n = null;
                        Materie m = null;
                        tuple.Add(Tuple.Create(n, studenti[i], m));
                    }
                    ViewBag.AniStudiu = Ani;

                    return View(tuple);
                }
                else if (type == 2)
                {
                    var list = await _repo.getRaportAsync() as List<Tuple<Nota, Student, Materie>>;
                    foreach (var item in list)
                    {
                        item.Item1.NotaObtinuta = await Database.ScalarAsync<Nota, int>(x => Sql.Max(x.NotaObtinuta), x => x.StudentId == item.Item2.Id && x.MaterieId == item.Item3.Id);
                    }
                    list.Sort((x, y) =>
                    {
                        var cmp = x.Item2.Nume.CompareTo(y.Item2.Nume);
                        if (cmp == 0) cmp = x.Item2.Prenume.CompareTo(y.Item2.Prenume);
                        if (cmp == 0) cmp = x.Item3.AnStudiu - y.Item3.AnStudiu;
                        if (cmp == 0) cmp = x.Item3.NumeDisciplina.CompareTo(y.Item3.NumeDisciplina);
                        return cmp;
                    });
                    return View(list);
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Procentaje(int? type, string materie)
        {
            double r = -1;
            if (type == 1 && !string.IsNullOrEmpty(materie))
                r = await _repo.getPassPercentageAsync(materie);
            if (type == 2) ViewBag.StudentiAn = await _repo.getRestantieriPercentageAsync();
            if (type == 3) ViewBag.StudentProm = await _repo.getMaxPrezentariAsync();

            return View(r);
        }

        [HttpGet]
        public IActionResult AdaugareNota()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Nepromovati()
        {
            return View(await _repo.getNepromovatiAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AdaugareNota(NotaStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var studentId = await Database.ScalarAsync<Student, int?>(x => x.Id, x => x.Nume == model.Nume.Trim() && x.Prenume == model.Prenume.Trim());
                if (studentId == null)
                {
                    ModelState.AddModelError("", "Studentul introdus nu există!");
                    return View();
                };

                var materieId = await Database.ScalarAsync<Materie, int>(x => x.Id, x => x.NumeDisciplina == model.DenumireMaterie.Trim());
                await _repo.insertNotaAsync(new Nota
                {
                    StudentId = studentId ?? 0,
                    NotaObtinuta = model.NotaObținută,
                    DataPrezentarii = DateTime.Now.AddDays(5),
                    MaterieId = materieId,
                    NrPrezentare = await Database.ScalarAsync<Nota, int?>(x => Sql.Max(x.NrPrezentare), x => x.MaterieId == materieId && x.StudentId == studentId) + 1 ?? 1
                });
                return RedirectToAction("Catalog");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
