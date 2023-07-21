using EmployeeBook.Dto;
using Microsoft.CodeAnalysis.Differencing;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NuGet.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace EmployeeBook.Models
{
    public class DbContext :IDbContext
    {
        public string ConnectionString { get; set; } 
        private MySqlConnection GetConnection()
        {
            var builder = WebApplication.CreateBuilder();
            string connString = builder.Configuration.GetConnectionString("DefaultConnection");
            return new MySqlConnection(connString);
        }
        public Person GetProfile(Guid userId)
        {
            Person person = new Person();
            using (var sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM persons Where UserId = @UserId", sqlCon);
                cmd.Parameters.AddWithValue("UserId", userId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        person.Id = reader.GetGuid("Id");
                        person.FirstName = reader.GetString("FirstName");
                        person.LastName = reader.GetString("LastName");
                        person.PersonCode = reader.GetString("PersonCode");
                        person.TelephoneNumber = reader.GetString("TelephoneNumber");
                        person.Email = reader.GetString("Email");
                        person.ProfilePicture = reader.GetFieldValue<byte[]>(6);                     
                    }
                }
            }
            return person;
        }
        public Person GetProfileByProfileId(Guid Id)
        {
            Person person = new Person();
            using (var sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM persons Where Id = @Id", sqlCon);
                cmd.Parameters.AddWithValue("Id", Id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        person.Id = reader.GetGuid("Id");
                        person.FirstName = reader.GetString("FirstName");
                        person.LastName = reader.GetString("LastName");
                        person.PersonCode = reader.GetString("PersonCode");
                        person.TelephoneNumber = reader.GetString("TelephoneNumber");
                        person.Email = reader.GetString("Email");
                        person.ProfilePicture = reader.GetFieldValue<byte[]>(6);
                    }
                }
            }
            return person;
        }
        public Person GetProfileByProfileUserId(Guid Id)
        {
            Person person = new Person();
            using (var sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM persons Where Id = @Id", sqlCon);
                cmd.Parameters.AddWithValue("Id", Id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        person.Id = reader.GetGuid("Id");
                        person.FirstName = reader.GetString("FirstName");
                        person.LastName = reader.GetString("LastName");
                        person.PersonCode = reader.GetString("PersonCode");
                        person.TelephoneNumber = reader.GetString("TelephoneNumber");
                        person.Email = reader.GetString("Email");
                        person.ProfilePicture = reader.GetFieldValue<byte[]>(6);
                    }
                }
            }
            return person;
        }
        public void CreateProfile(PersonImage personImage, Guid dataGuid, byte[] pic)
        {
            using (var sqlCon = GetConnection())
            {
                sqlCon.Open();
                string query = "INSERT INTO persons VALUES(@Id,@Username,@Password,@PasswordSalt,@Role)";

                MySqlCommand cmd = new MySqlCommand("INSERT INTO persons (Id, FirstName, LastName, PersonCode, TelephoneNumber, Email, ProfilePicture, UserId) VALUES (@Id, @FirstName, @LastName,@PersonCode,@TelephoneNumber,@Email,@ProfilePicture,@UserId)", sqlCon);
                cmd.Parameters.AddWithValue("Id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("FirstName", personImage.FirstName);
                cmd.Parameters.AddWithValue("LastName", personImage.LastName);
                cmd.Parameters.AddWithValue("PersonCode", personImage.PersonCode);
                cmd.Parameters.AddWithValue("TelephoneNumber", personImage.TelephoneNumber);
                cmd.Parameters.AddWithValue("Email", personImage.Email);
                cmd.Parameters.AddWithValue("ProfilePicture", pic);
                cmd.Parameters.AddWithValue("UserID", dataGuid);
                cmd.ExecuteNonQuery();
            }
        }
        public void EditPerson(Person person)
        {
            if (person.FirstName != null)
            {
                using (var sqlCon = GetConnection())
                {
                    sqlCon.Open();

                    string query = "UPDATE persons SET";
                    var parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(person.FirstName))
                    {
                        query += " FirstName = @FirstName,";
                        parameters.Add(new MySqlParameter("@FirstName", person.FirstName));
                    }

                    if (!string.IsNullOrEmpty(person.LastName))
                    {
                        query += " LastName = @LastName,";
                        parameters.Add(new MySqlParameter("@LastName", person.LastName));
                    }

                    if (!string.IsNullOrEmpty(person.TelephoneNumber))
                    {
                        query += " TelephoneNumber = @TelephoneNumber,";
                        parameters.Add(new MySqlParameter("@TelephoneNumber", person.TelephoneNumber));
                    }
                    if (!string.IsNullOrEmpty(person.PersonCode))
                    {
                        query += " PersonCode = @PersonCode,";
                        parameters.Add(new MySqlParameter("@PersonCode", person.PersonCode));
                    }
                    if (!string.IsNullOrEmpty(person.Email))
                    {
                        query += " Email = @Email,";
                        parameters.Add(new MySqlParameter("@Email", person.Email));
                    }

                    if (person.ProfilePicture != null && person.ProfilePicture.Length > 0)
                    {
                        query += " ProfilePicture = @ProfilePicture,";
                        parameters.Add(new MySqlParameter("@ProfilePicture", person.ProfilePicture));
                    }
                    query = query.TrimEnd(',');
                    query += " WHERE Id = @Id";
                    parameters.Add(new MySqlParameter("@Id", person.Id));

                    using (var cmd = new MySqlCommand(query, sqlCon))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public void EditUser(User user)
        {
            using (var sqlCon = GetConnection())
            {
                sqlCon.Open();
                string query = "UPDATE user SET";
                var parameters = new List<MySqlParameter>();

                if (!string.IsNullOrEmpty(user.Username))
                {
                    query += " Username = @Username,";
                    parameters.Add(new MySqlParameter("@Username", user.Username));
                }

                if (user.Password != null && user.Password.Length > 0)
                {
                    query += " Password = @Password,";
                    parameters.Add(new MySqlParameter("@Password", user.Password));
                }

                if (user.PasswordSalt != null && user.PasswordSalt.Length > 0)
                {
                    query += " PasswordSalt = @PasswordSalt,";
                    parameters.Add(new MySqlParameter("@PasswordSalt", user.PasswordSalt));
                }
                if (!string.IsNullOrEmpty(user.Role))
                {
                    query += " Role = @Role,";
                    parameters.Add(new MySqlParameter("@Role", user.Role));
                }
                // Remove the trailing comma from the query
                query = query.TrimEnd(',');
                query += " WHERE Id = @Id";
                parameters.Add(new MySqlParameter("@Id", user.Id));
                using (var cmd = new MySqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());

                    cmd.ExecuteNonQuery();

           
                }
            }
        }
        public void DeleteUser(Guid Id)
        {
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM user WHERE Id = @Id", sqlCon);
                cmd.Parameters.AddWithValue("Id", Id);
                cmd.ExecuteNonQuery();
            }
        }
        public void CreateUser(User user)
        {
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand checkUsernameCmd = new MySqlCommand("SELECT COUNT(*) FROM user WHERE Username = @Username", sqlCon);
                checkUsernameCmd.Parameters.AddWithValue("@Username", user.Username);
                int existingUserCount = Convert.ToInt32(checkUsernameCmd.ExecuteScalar());

                if (existingUserCount > 0)
                {
                    // Username already exists, handle the error accordingly
                    throw new Exception("Username already exists");
                }
                MySqlCommand cmd = new MySqlCommand("INSERT INTO user (Id,Username, Password, PasswordSalt, Role) VALUES ('" + user.Id + "','" + user.Username + "',@Password, @PasswordSalt,'" + user.Role + "')", sqlCon);
                cmd.Parameters.AddWithValue("Password", user.Password);
                cmd.Parameters.AddWithValue("PasswordSalt", user.PasswordSalt);
                cmd.ExecuteNonQuery();
            }
        }
        public User GetUser(UserDto userDto)
        {
            User user = new User();
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM user Where Username = @Username ", sqlCon);
                cmd.Parameters.AddWithValue("Username", userDto.UserName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Id = reader.GetFieldValue<Guid>(0);
                        user.Username = reader.GetFieldValue<string>(1);
                        user.Password = reader.GetFieldValue<byte[]>(2);
                        user.PasswordSalt = reader.GetFieldValue<byte[]>(3);
                        user.Role = reader.GetFieldValue<string>(4);
                    }
                }         
            }
            return user;
        }
        public User GetUserById(Guid Id)
        {
            User user = new User();
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM user Where Id = @Id ", sqlCon);
                cmd.Parameters.AddWithValue("Id", Id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Id = reader.GetFieldValue<Guid>(0);
                        user.Username = reader.GetFieldValue<string>(1);
                        user.Password = reader.GetFieldValue<byte[]>(2);
                        user.PasswordSalt = reader.GetFieldValue<byte[]>(3);
                        user.Role = reader.GetFieldValue<string>(4);
                    }
                }
            }
            return user;
        }
        public List<Person> ShowProfiles()
        {
            List<Person> list = new List<Person>();
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM persons", sqlCon);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Person()
                        {
                            Id = reader.GetGuid("Id"),
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            PersonCode = reader.GetString("PersonCode"),
                            TelephoneNumber = reader.GetString("TelephoneNumber"),
                            Email = reader.GetString("Email"),
                            ProfilePicture = reader.GetFieldValue<byte[]>(6),
                        });
                    }
                }
            }
            return list;
        }
        public List<User> ShowUsers()
        {
            List<User> list = new List<User>();
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM user", sqlCon);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User()
                        {
                           Id=reader.GetGuid("Id"),
                           Username = reader.GetString("Username"),
                           Role = reader.GetString("Role")
                        });
                    }
                }
            }
            return list;
        }
        public bool CheckUserExist(UserDto user)
        {
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand checkUsernameCmd = new MySqlCommand("SELECT COUNT(*) FROM user WHERE Username = @Username", sqlCon);
                checkUsernameCmd.Parameters.AddWithValue("@Username", user.UserName);
                int existingUserCount = Convert.ToInt32(checkUsernameCmd.ExecuteScalar());

                if (existingUserCount > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public bool CheckUserHasProfile(Guid userId)
        {
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand checkUsernameCmd = new MySqlCommand("SELECT COUNT(*) FROM persons WHERE UserId = @UserId", sqlCon);
                checkUsernameCmd.Parameters.AddWithValue("@UserId", userId);
                int existingUserCount = Convert.ToInt32(checkUsernameCmd.ExecuteScalar());

                if (existingUserCount > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public void DeleteProfile(Guid Id)
        {
            using (MySqlConnection sqlCon = GetConnection())
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM persons WHERE Id = @Id", sqlCon);
                cmd.Parameters.AddWithValue("Id", Id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
