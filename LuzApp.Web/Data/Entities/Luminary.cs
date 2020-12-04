using LuzApp.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LuzApp.Web.Data.Entities
{
    public class Luminary
    {
        public int Id { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Address { get; set; }

        [Display(Name = "Latitud")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public double Latitude { get; set; }

        [Display(Name = "Longitud")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public double Longitude { get; set; }

        [Display(Name = "Relevado por")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public User User { get; set; }

        [Display(Name = "Barrio")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public Neighborhood Neighborhood { get; set; }

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime Date { get; set; }

        [Display(Name = "Tipo")]
        [MaxLength(15, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        public string Type { get; set; }

        public string BasePhoto { get; set; }
        public string TopPhoto { get; set; }
        public string FullPhoto { get; set; }
       
        [Display(Name = "Foto de la Base")]
        public string BasePhotoFullPath => string.IsNullOrEmpty(BasePhoto)
          ? "noimage"//null
          : $"http://keypress.serveftp.net:88/LuzAppApi{BasePhoto.Substring(1)}";

        [Display(Name = "Foto de la luminaria")]
        public string TopPhotoFullPath => string.IsNullOrEmpty(TopPhoto)
          ? "noimage"//null
          : $"http://keypress.serveftp.net:88/LuzAppApi{BasePhoto.Substring(1)}";

        [Display(Name = "Foto poste completo")]
        public string FullPhotoFullPath => string.IsNullOrEmpty(FullPhoto)
          ? "noimage"//null
          : $"http://keypress.serveftp.net:88/LuzAppApi{BasePhoto.Substring(1)}";

        [Display(Name = "Estado")]
        [MaxLength(15, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        public string State { get; set; }

        [Display(Name = "Observaciones")]
        public string Remarks { get; set; }


        public ICollection<LuminaryImage> LuminaryImages { get; set; }

        [DisplayName("Product Images Number")]
        public int LuminaryImagesNumber => LuminaryImages == null ? 0 : LuminaryImages.Count;

        [Display(Name = "Imagen")]
        public string ImageFullPath => LuminaryImages == null || LuminaryImages.Count == 0
            ? $"http://keypress.serveftp.net:88/LuzAppApi/images/Luminaries/noimage.png"
            : LuminaryImages.FirstOrDefault().ImageFullPath;
    }
}