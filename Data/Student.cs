using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ProiectPbd.Data
{
    [Alias("Studenti")]
    public class Student
    {
        [AutoIncrement]
        public int Id { get; set; }

        [StringLength(15)]
        [Required]
        public string Nume { get; set; }

        [StringLength(20)]
        [Required]
        public string Prenume { get; set; }

        [Index]
        [StringLength(6)]
        [Unique]
        [Required]
        public int NrLegitimatie { get; set; }

        [Reference]
        public List<Nota> Note { get; set; }

        [CheckConstraint("MedieGenerala BETWEEN 1 AND 10")]
        public double? MedieGenerala { get; set; }
    }
}
