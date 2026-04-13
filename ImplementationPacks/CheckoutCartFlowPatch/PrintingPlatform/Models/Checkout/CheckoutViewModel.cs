using System.ComponentModel.DataAnnotations;

namespace PrintingPlatform.Models.Checkout
{
    public class CheckoutViewModel
    {
        [Required (ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required (ErrorMessage = "Email is required")]
        [EmailAddress (ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required (ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required (ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required (ErrorMessage = "Postal code is required")]

        [Display(Name = "Postal Code")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Postal code must be between 5 and 10 characters")]
        public string PostalCode { get; set; } = string.Empty;
    }
}