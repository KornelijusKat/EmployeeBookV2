using EmployeeBook.Dto;
using MySqlConnector;

namespace EmployeeBook.Models
{
    public interface IDbContext
    {
        Person GetProfile(Guid userId);
        void DeleteUser(Guid Id);
        void EditPerson(Person person);
        User GetUser(UserDto userDto);
        void CreateProfile(PersonImage personImage, Guid dataGuid, byte[] pic);
        List<Person> ShowProfiles();
        void CreateUser(User user);
        Person GetProfileByProfileId(Guid Id);
        bool CheckUserExist(UserDto user);
        bool CheckUserHasProfile(Guid userId);
        void EditUser(User user);
        User GetUserById(Guid Id);
        List<User> ShowUsers();
        void DeleteProfile(Guid Id);
    }
}
