using ProiectPbd.Data;
using ProiectPbd.DataModels;
using ProiectPbd.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProiectPbd.Repositories
{
    public interface ITableUtils
    {
        Task createTablesAsync();
        Task populateTablesAsync();
        Task insertNotaAsync(Nota nota);
        Task<List<CatalogModel>> getMapCatalog();
        Task<IEnumerable<Tuple<Nota, Student, Materie>>> getNepromovatiAsync();
        Task<IEnumerable<Tuple<Nota, Student, Materie>>> getRaportAsync();
        Task<double> getPassPercentageAsync(string materie);
        Task<List<StudentAni>> getRestantieriPercentageAsync();

        Task<StudentPrezentari> getMaxPrezentariAsync();
    }
    public class TableUtils : ITableUtils
    {
        private readonly IDbConnection Database;
        public TableUtils(IDbConnection database)
        {
            Database = database;
        }
        public async Task createTablesAsync()
        {

            Database.DropTable<Nota>();
            Database.DropTable<StudentMedieAn>();
            Database.DropAndCreateTable<Student>();
            Database.DropAndCreateTable<Materie>();
            Database.CreateTable<Nota>();
            Database.CreateTable<StudentMedieAn>();
            await Database.ExecuteSqlAsync(
                @"CREATE TRIGGER update_medie_an AFTER INSERT ON note
                    FOR EACH ROW
                    BEGIN
                        DECLARE medie FLOAT;
                        DECLARE an INT;
                        SET an = (SELECT AnStudiu FROM materii WHERE new.MaterieId=materii.Id LIMIT 1);
                        SET medie = (SELECT AVG(x.NotaMax) FROM  (
                                    SELECT MAX(NotaObtinuta) AS NotaMax
                                    FROM note
                                    JOIN materii m ON m.Id = note.MaterieId
                                    AND m.AnStudiu=an AND StudentId=new.StudentId
                                    GROUP BY MaterieId) x);

                        INSERT INTO medii(Id, an, studentid, medie)
                        VALUES(CONCAT(new.StudentId,'/',an), an, new.StudentId, TRUNCATE(medie,2)) ON DUPLICATE KEY UPDATE
                            Medie = TRUNCATE(medie,2);
                    END;");

            await Database.ExecuteSqlAsync(
                @"CREATE TRIGGER update_medie_generala_u AFTER UPDATE ON medii
                    FOR EACH ROW
                    BEGIN
                        UPDATE studenti
                            SET MedieGenerala = TRUNCATE((SELECT AVG(Medie) FROM medii WHERE StudentId=studenti.Id),2)
                        WHERE new.StudentId = studenti.Id;
                    END;");

            await Database.ExecuteSqlAsync(
                @"CREATE TRIGGER update_medie_generala_i AFTER INSERT ON medii
                    FOR EACH ROW
                    BEGIN
                        UPDATE studenti
                            SET MedieGenerala = TRUNCATE((SELECT AVG(Medie) FROM medii WHERE StudentId=studenti.Id),2)
                        WHERE new.StudentId = studenti.Id;
                    END;");

        }
        public async Task populateTablesAsync()
        {

            var materii = new List<Materie>{
                    new Materie{
                    NumeDisciplina = "Fizica",
                    AnStudiu = 1
                    },
                    new Materie{
                    NumeDisciplina = "Matematica",
                    AnStudiu = 1
                    },
                    new Materie{
                    NumeDisciplina = "Engleza",
                    AnStudiu = 2
                    },
                    new Materie{
                    NumeDisciplina = "OOP",
                    AnStudiu = 2
                    },
                    new Materie{
                    NumeDisciplina = "PBD",
                    AnStudiu = 3
                    },
                    new Materie{
                    NumeDisciplina = "MTP",
                    AnStudiu = 3
                    },
                };

            var studenti = new List<Student>
                {
                    new Student
                    {
                        Nume="Popescu",
                        Prenume = "Alin",
                        NrLegitimatie = 450721,
                        Note = new List<Nota>
                        {
                            new Nota
                            {
                                MaterieId = 1,
                                DataPrezentarii = DateTime.Now,
                                NotaObtinuta = 7,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 2,
                                DataPrezentarii = DateTime.Now,
                                NotaObtinuta = 8,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 3,
                                DataPrezentarii = DateTime.Now.AddDays(1),
                                NotaObtinuta = 9,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 4,
                                DataPrezentarii = DateTime.Now.AddDays(1),
                                NotaObtinuta = 10,
                                NrPrezentare = 1
                            }
                        }
                    },
                    new Student
                    {
                        Nume="Marinescu",
                        Prenume = "Vasile",
                        NrLegitimatie = 450735,
                        Note = new List<Nota>
                        {
                            new Nota
                            {
                                MaterieId = 3,
                                DataPrezentarii = DateTime.Now.AddDays(1),
                                NotaObtinuta = 9,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 4,
                                DataPrezentarii = DateTime.Now.AddDays(1),
                                NotaObtinuta = 10,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 5,
                                DataPrezentarii = DateTime.Now.AddDays(2),
                                NotaObtinuta = 3,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 6,
                                DataPrezentarii = DateTime.Now.AddDays(2),
                                NotaObtinuta = 4,
                                NrPrezentare = 1
                            }
                        }
                    },
                    new Student
                    {
                        Nume="Zanfir",
                        Prenume = "Elena",
                        NrLegitimatie = 450755,
                        Note = new List<Nota>
                        {
                            new Nota
                            {
                                MaterieId = 1,
                                DataPrezentarii = DateTime.Now,
                                NotaObtinuta = 7,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 2,
                                DataPrezentarii = DateTime.Now,
                                NotaObtinuta = 8,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 5,
                                DataPrezentarii = DateTime.Now.AddDays(2),
                                NotaObtinuta = 3,
                                NrPrezentare = 1
                            },
                            new Nota
                            {
                                MaterieId = 6,
                                DataPrezentarii = DateTime.Now.AddDays(2),
                                NotaObtinuta = 4,
                                NrPrezentare = 1
                            }
                        }
                    }

                };

            await Database.InsertAllAsync(materii);
            foreach (var student in studenti)
            {
                await Database.SaveAsync(student, references: true);
            }

        }

        public async Task insertNotaAsync(Nota nota)
        {
            await Database.InsertAsync(nota);
        }

        public async Task<List<CatalogModel>> getMapCatalog()
        {
            List<CatalogModel> catalogEntries = new List<CatalogModel>();

            var ani = await Database.ScalarAsync<Materie, int>(x => Sql.Max(x.AnStudiu));

            var q = Database.From<Nota>().Join<Nota, Student>().Join<Nota, Materie>().OrderByDescending(x => x.DataPrezentarii).Select();
            var result = await Database.SelectMultiAsync<Nota, Student, Materie>(q);

            foreach (var tuple in result)
            {
                CatalogModel catalog = new CatalogModel();

                catalog.Student = tuple.Item2;
                catalog.Nota = tuple.Item1;
                catalog.Materie = tuple.Item3;
                catalog.Medii = new float[ani];
                for (int i = 0; i < ani; i++)
                {
                    catalog.Medii[i] = await Database.ScalarAsync<StudentMedieAn, float>(x => x.Medie, x => x.An == (i + 1) && x.StudentId == tuple.Item2.Id);
                }
                catalog.NrAni = ani;
                catalogEntries.Add(catalog);
            }

            return catalogEntries;

        }

        public async Task<IEnumerable<Tuple<Nota, Student, Materie>>> getNepromovatiAsync()
        {
            List<Tuple<Nota, Student, Materie>> result = new List<Tuple<Nota, Student, Materie>>();
            var materii = await Database.ColumnDistinctAsync<int>(Database.From<Materie>().Select(x => x.Id));
            var studenti = await Database.ColumnDistinctAsync<int>(Database.From<Student>().Select(x => x.Id));
            if (materii != null)
                foreach (var materie in materii)
                {
                    if (studenti != null)
                        foreach (var student in studenti)
                        {
                            var q = Database.From<Nota>().Join<Student>().Join<Materie>().Having(x => Sql.Max(x.NotaObtinuta) < 5)
                                .Where(x => x.MaterieId == materie && x.StudentId == student).Select();
                            result.AddRange(await Database.SelectMultiAsync<Nota, Student, Materie>(q));
                        }
                }
            return result;
        }

        public async Task<IEnumerable<Tuple<Nota, Student, Materie>>> getRaportAsync()
        {
            List<Tuple<Nota, Student, Materie>> result = new List<Tuple<Nota, Student, Materie>>();
            var materii = await Database.ColumnDistinctAsync<int>(Database.From<Materie>().Select(x => x.Id));
            var studenti = await Database.ColumnDistinctAsync<int>(Database.From<Student>().Select(x => x.Id));
            if (materii != null)
                foreach (var materie in materii)
                {
                    if (studenti != null)
                        foreach (var student in studenti)
                        {
                            var q = Database.From<Nota>().Join<Student>().Join<Materie>().Having(x => Sql.Max(x.NotaObtinuta) > 5)
                                .Where(x => x.MaterieId == materie && x.StudentId == student).Select();
                            result.AddRange(await Database.SelectMultiAsync<Nota, Student, Materie>(q));
                        }
                }
            return result;
        }

        public async Task<double> getPassPercentageAsync(string materie)
        {
            int promovati = 0;
            var materieId = await Database.ScalarAsync<int>(Database.From<Materie>().Select(x => x.Id).Where(x => x.NumeDisciplina == materie));
            var studenti = await Database.ColumnDistinctAsync<int>(Database.From<Student>().Select(x => x.Id));
            if (studenti != null)
                foreach (var student in studenti)
                {
                    var q = Database.From<Nota>().Join<Student>().Join<Materie>().Having(x => Sql.Max(x.NotaObtinuta) > 5)
                                 .Where(x => x.MaterieId == materieId && x.StudentId == student).Select();
                    var r = await Database.SelectMultiAsync<Nota, Student, Materie>(q);
                    if (r.Count() != 0) promovati++;
                }
            var total = await Database.ScalarAsync<Nota, int>(x => Sql.CountDistinct(x.StudentId), x => materieId == x.MaterieId);
            return promovati / (double)total;
        }

        public async Task<List<StudentAni>> getRestantieriPercentageAsync()
        {
            List<StudentAni> result = new List<StudentAni>();
            var ani = await Database.ScalarAsync<Materie, int>(x => Sql.Max(x.AnStudiu));
            var studenti = await Database.ColumnDistinctAsync<int>(Database.From<Student>().Select(x => x.Id));
            if (studenti.Count != 0)
            {
                foreach (var student in studenti)
                    for (int i = 1; i <= ani - 1; i++)
                    {
                        var q1 = Database.From<Nota>().Join<Student>().Join<Materie>().Having(x => Sql.Max(x.NotaObtinuta) < 5)
                        .Where<Nota, Materie>((x, y) => x.StudentId == student && y.AnStudiu == i).Select<Student>(x => new { x.Nume, x.Prenume, x.NrLegitimatie });
                        var q2 = Database.From<Nota>().Join<Student>().Join<Materie>().Having(x => Sql.Max(x.NotaObtinuta) < 5)
                       .Where<Nota, Materie>((x, y) => x.StudentId == student && y.AnStudiu == i + 1).Select<Student>(x => new { x.Nume, x.Prenume, x.NrLegitimatie });
                        var s1 = await Database.SingleAsync<Student>(q1);
                        var s2 = await Database.SingleAsync<Student>(q2);
                        if (s1 != null && s2 != null)
                        {
                            result.Add(new StudentAni
                            {
                                student = s1,
                                ani = new int[] { i, i + 1 }
                            });
                        }
                    }
            }
            return result;
        }

        public async Task<StudentPrezentari> getMaxPrezentariAsync()
        {
            var studenti = await Database.ColumnDistinctAsync<int>(Database.From<Student>().Select(x => x.Id));
            List<StudentPrezentari> nrprezentari = new List<StudentPrezentari>();
            foreach (var student in studenti)
            {
                var nc = await Database.ScalarAsync<Nota, int>(x => Sql.Count(x.Id), x => x.StudentId == student);
                nrprezentari.Add(new StudentPrezentari
                {
                    student = await Database.SingleByIdAsync<Student>(student),
                    prezentari = nc
                });
            }

            int max = 0;
            foreach (var item in nrprezentari)
            {
                max = Math.Max(max, item.prezentari);
            }
            var result = nrprezentari.Find(x => x.prezentari == max);
            var studentId = result.student.Id;
            int promovate = 0;
            var materii = await Database.ColumnDistinctAsync<int>(Database.From<Materie>().Select(x => x.Id));
            if (materii != null)
                foreach (var materie in materii)
                {
                    var q = Database.From<Nota>().Join<Student>().Join<Materie>().Having(x => Sql.Max(x.NotaObtinuta) > 5)
                                 .Where(x => x.MaterieId == materie && x.StudentId == studentId).Select();
                    var r = await Database.SelectMultiAsync<Nota, Student, Materie>(q);
                    if (r.Count() != 0) promovate++;
                }
            var total = await Database.ScalarAsync<Nota, int>(x => Sql.CountDistinct(x.Id), x => x.StudentId == studentId);
            result.prom = promovate / (double)total;

            return result;
        }
    }
}
