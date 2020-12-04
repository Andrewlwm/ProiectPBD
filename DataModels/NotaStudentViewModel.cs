using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProiectPbd.DataModels
{
    public class NotaStudentViewModel
    {
        [Required]
        public string Nume { get; set; }
        [Required]
        public string Prenume { get; set; }
        public string StudentId { get; set; }

        [Display(Name = "Nota obținută")]
        [Required]
        [Range(1,10,ErrorMessage ="Nota trebuie să fie un număr decimal între 1 și 10")]
        public int NotaObținută { get; set; }

        [Display(Name = "Materie")]
        [Required]
        public string DenumireMaterie { get; set; }
    }
}
