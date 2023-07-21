using EmployeeBook.Dto;
using System.ComponentModel.DataAnnotations;
using static EmployeeBook.Models.ValidationsExtensions;

namespace EmployeeBook.ViewModels
{
    public class ViewModel
    {
        public Guid Id { get; set; }
        public PersonImage personImage { get; set; }
    }
}
