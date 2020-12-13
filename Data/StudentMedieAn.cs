using ProiectPbd.Data;
using ServiceStack.DataAnnotations;

namespace ProiectPbd.Data
{
    [Alias("Medii")]
    public class StudentMedieAn
    {
        [PrimaryKey]
        public string Id
        {
            get { return $"{this.StudentId}/{this.An}"; }
        }

        [Required]
        public int An { get; set; }

        [ForeignKey(typeof(Student), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        [Required]
        public int StudentId { get; set; }

        [Required]
        [CheckConstraint("Medie BETWEEN 1 AND 10")]
        public float? Medie { get; set; }
    }
}