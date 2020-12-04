using System.ComponentModel.DataAnnotations;

namespace LuzApp.Common.Requests
{
    public class EmailRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}