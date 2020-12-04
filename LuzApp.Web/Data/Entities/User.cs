using LuzApp.Common.Entities;
using LuzApp.Common.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LuzApp.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "Documento")]
        [MaxLength(20)]
        [Required]
        public string Document { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(100)]
        public string Address { get; set; }

        [Display(Name = "Imagen")]
        public string ImagePath { get; set; }

        //TODO: Pending to put the correct paths
        [Display(Name = "Image")]
        public string ImageFullPath => string.IsNullOrEmpty(ImagePath)
            ? $"http://keypress.serveftp.net:88/LuzAppApi/images/Users/nouser.png"
           : $"http://keypress.serveftp.net:88/LuzAppApi{ImagePath.Substring(1)}";

        [Display(Name = "User Type")]
        public UserType UserType { get; set; }

        public Neighborhood Neighborhood { get; set; }

        [Display(Name = "Usuario")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Usuario")]
        public string FullNameWithDocument => $"{FirstName} {LastName} - {Document}";
    }
}