using ServiceStack.DataAnnotations;

namespace ProiectPbd.Data
{
    [Alias("Materii")]
    public class Materie
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required]
        public string NumeDisciplina { get; set; }

        [Required]
        public int AnStudiu { get; set; }

        [Reference]
        public int MyProperty { get; set; }

    }
}