using System.ComponentModel.DataAnnotations;

namespace EmployeeBook.Dto
{
    public class UserDto
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
