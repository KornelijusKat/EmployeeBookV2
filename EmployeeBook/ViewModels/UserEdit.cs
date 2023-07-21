using EmployeeBook.Dto;

namespace EmployeeBook.ViewModels
{
    public class UserEdit
    {
        public string Role { get; set; }
        public UserDto User { get; set; }
        public Guid Id { get; set; }
    }
}
