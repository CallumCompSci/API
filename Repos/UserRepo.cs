using System;
using Interfaces;
using Last_Api.Models;

using Npgsql;
using RepoDb;

namespace Last_Api.Repos;

public class UserRepo : IUser
{

    private readonly IConfiguration _configuration;
    private readonly string ConnectionString;

    public UserRepo(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<User> GetUsers()
    {

        List<User> users = new List<User>();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = connection.QueryAll<User>("users");
            users = result.OrderBy(a => a.email).ToList();
        }
        
        return users;

    }

    public User GetUser(string _email)
    {
        User user = new User();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            user = connection.Query<User>("users", e => e.email == _email).FirstOrDefault();
        }
        return user;
    }



    public User AddUser(User newUser)
    {
        string password = newUser.password;
        newUser.password = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Insert<User>(tableName: "users", entity: newUser);
        }
        return newUser;
    }

    public void PatchEmail(String oldEmail, User user)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            Console.WriteLine($"Email: {oldEmail}");
            Console.WriteLine($"New Email: {user.email}");
            connection.Open();
            //Because we match based on email, the normal DBRepo command doesnt work, we could match based off password, but that doesnt seem like a good idea.
            using (var cmd = new NpgsqlCommand("UPDATE users SET email = @newEmail WHERE email = @oldEmail", connection))
            {
                cmd.Parameters.AddWithValue("@newEmail", user.email);
                cmd.Parameters.AddWithValue("@oldEmail", oldEmail);
                cmd.ExecuteNonQuery();
            }
        }
        
    }
    

    public void PatchPassword(User user)
    {
        string password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 13); 
        user.password = password;
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var fields = Field.Parse<User>(e => new
            {
                e.password
            });
            string email = user.email;
            connection.Update<User>(tableName: "users", entity: user, where: e => e.email == email);
        }
        
    }


    public void PatchRole(User user)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var fields = Field.Parse<User>(e => new
            {
                e.role
            });
            string email = user.email;
            connection.Update<User>(tableName: "users", entity: user, where: e => e.email == email);
        }
        
    }

    public void DeleteUser(string _email)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            //Delete uses Queryfields instead of lambda......
            connection.Delete(tableName: "users", where: new QueryField("email", _email));
        }

    }



}
