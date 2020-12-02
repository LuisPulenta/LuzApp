using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuzApp.Common.Entities
{
    public class Neighborhood
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres.")]
        [Required]
        [Display(Name = "Barrio")]
        public string Name { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int IdCity{ get; set; }

        [JsonIgnore]
        public City City { get; set; }
    }
}