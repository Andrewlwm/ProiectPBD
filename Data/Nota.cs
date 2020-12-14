using System;
using Newtonsoft.Json;
using ServiceStack.DataAnnotations;

namespace ProiectPbd.Data
{
    [Alias("Note")]
    public class Nota
    {
        [AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Student), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        [Required]
        public int StudentId { get; set; }

        [ForeignKey(typeof(Materie), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        [Required]
        public int MaterieId { get; set; }

        [Required]
        public int NrPrezentare { get; set; }

        [Required]
        public DateTime DataPrezentarii { get; set; }

        [Required]
        [CheckConstraint("NotaObtinuta BETWEEN 1 AND 10")]
        public int NotaObtinuta { get; set; }
    }
}