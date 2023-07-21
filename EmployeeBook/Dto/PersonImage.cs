using System.ComponentModel.DataAnnotations;
using System.Drawing;
using static EmployeeBook.Models.ValidationsExtensions;

namespace EmployeeBook.Dto
{
    public class PersonImage
    {
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Please enter 11 digit code")]
        public string PersonCode { get; set; }
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"(86|\+3706)\d{3}\d{4}", ErrorMessage = "Wrong Format Telephone format, please use 86/+3706 format numbers")]
        public string TelephoneNumber { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        [ValidExtensions(new string[] { ".png", ".jpg", })]
        public IFormFile ProfilePicture { get; set; }
    }
}
