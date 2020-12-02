using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace LuzApp.Common.Entities
{
    public class City
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres.")]
        [Required]
        [Display(Name = "Ciudad")]
        public string Name { get; set; }

        public ICollection<Neighborhood> Neighborhoods { get; set; }

        [DisplayName("N° de Barrios")]
        public int NeighborhoodsNumber => Neighborhoods == null ? 0 : Neighborhoods.Count;

        [JsonIgnore]
        [NotMapped]
        public int IdDepartment { get; set; }

        [JsonIgnore]
        public Department Department { get; set; }

        
    }
}