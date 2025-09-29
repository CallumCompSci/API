using Last_Api.Models;


namespace Interfaces
{
    public interface IUser
    {
        public List<User> GetUsers();

        public User GetUser(string email);

        public User AddUser(User user);

        public void PatchEmail(string oldEmail, User user);
        public void PatchPassword(User user);
        public void PatchRole(User use);
        public void DeleteUser(string email);

    }
}