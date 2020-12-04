using System;
using System.ComponentModel.DataAnnotations;

namespace LuzApp.Web.Data.Entities
{
    public class LuminaryImage
    {
        public int Id { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime Date { get; set; }

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public User User { get; set; }

        [Display(Name = "Imagen")]
        public string ImageUrl { get; set; }

        [Display(Name = "Observaciones")]
        public string Remarks { get; set; }


        public string ImageFullPath => string.IsNullOrEmpty(ImageUrl)
        ? $"http://keypress.serveftp.net:88/LuzAppApi/images/Luminaries/noimage.png"
        : $"http://keypress.serveftp.net:88/LuzAppApi{ImageUrl.Substring(1)}";
    }
}
