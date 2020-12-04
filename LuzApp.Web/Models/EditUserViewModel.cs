using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuzApp.Web.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Documento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(20)]
        public string Document { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(50)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string LastName { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(100)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Address { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Imagen")]
        public string ImagePath { get; set; }

        [Display(Name = "Imagen")]
        public string ImageFullPath => string.IsNullOrEmpty(ImagePath)
            ? $"http://keypress.serveftp.net:88/LuzAppApi/images/Users/nouser.png"
           : $"http://keypress.serveftp.net:88/LuzAppApi{ImagePath.Substring(1)}";

        [Display(Name = "Imagen")]
        public IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Provincia")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una Provincia.")]
        public int DepartmentId { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una Ciudad")]
        public int CityId { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Barrio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Barrio")]
        public int NeighborhoodId { get; set; }

        public IEnumerable<SelectListItem> Neighborhoods { get; set; }
    }
}