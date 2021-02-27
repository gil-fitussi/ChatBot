using ChatBot.Models;

namespace ChatBot.Interface
{
    public interface IRegistration
    {
        ResultSet CheckUserLoginDecision();
        ResultSet LoginOperation();
        ResultSet SignUpOperation();
    }
}
