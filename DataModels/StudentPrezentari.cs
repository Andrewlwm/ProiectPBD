using ProiectPbd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProiectPbd.DataModels
{
    public class StudentPrezentari
    {
        public Student student { get; set; }
        public int prezentari { get; set; }
        public double prom { get; set; }
    }
}
