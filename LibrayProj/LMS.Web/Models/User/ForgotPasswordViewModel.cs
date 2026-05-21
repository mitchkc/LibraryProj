using System.ComponentModel.DataAnnotations;

namespace LMS.Web.Models.User;
public class ForgotPasswordViewModel
{
    [Required]
    public string Email { get; set; }
    
}
