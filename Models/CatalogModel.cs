using ProiectPbd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProiectPbd.Models
{
    public class CatalogModel
    {
        public Student Student { get; set; }
        public Nota Nota { get; set; }
        public float[] Medii { get; set; }
        public Materie Materie { get; set; }

        public int[] Note { get; set; }

        public string[] NumeMaterie { get; set; }

        public int NrAni { get; set; }
    }
}
