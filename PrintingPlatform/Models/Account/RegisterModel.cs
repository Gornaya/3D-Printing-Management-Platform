using System.ComponentModel.DataAnnotations;

namespace PrintingPlatform.Models.Account;

public class RegisterModel
{
    [Required(ErrorMessage = "First name is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = "";
    
    [Required(ErrorMessage = "Last name is required.")]
    [Display(Name = "Last Name")]   
    public string LastName { get; set; } = "";
    
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Display(Name = "Email")]   
    public string Email { get; set; } = "";
   
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [Display(Name = "Password")]
    public string Password { get; set; } = "";
}