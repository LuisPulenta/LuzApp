using System.ComponentModel.DataAnnotations;

namespace LuzApp.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}