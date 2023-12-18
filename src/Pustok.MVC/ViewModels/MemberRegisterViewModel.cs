using System.ComponentModel.DataAnnotations;

namespace Pustok.MVC.ViewModels
{
    public class MemberRegisterViewModel
    {
        [StringLength(maximumLength: 30)]
        public string Fullname { get; set; }
        [StringLength(maximumLength: 30)]
        public string Username { get; set; }
        [StringLength(maximumLength: 55),
        DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(maximumLength: 25, MinimumLength = 8), 
         DataType(DataType.Password)]
        public string Password { get; set; }
        [StringLength(maximumLength: 25, MinimumLength = 8), 
        DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
