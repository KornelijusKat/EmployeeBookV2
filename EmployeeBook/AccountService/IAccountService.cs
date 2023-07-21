using EmployeeBook.Models;

namespace EmployeeBook.AccountService
{
    public interface IAccountService
    {
        User CreateAccount(string username, string password);
        Boolean Logins(string username, string passwords, byte[] password, byte[] salt);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    }
}
