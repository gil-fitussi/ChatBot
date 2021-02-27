using ChatBot.Models;

namespace ChatBot.Interface
{
    public interface IDbQuery
    {
        ResultSet AddUser(User user);
        ResultSet Login(User user);
    }
}
