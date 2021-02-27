using ChatBot.Interface;

namespace ChatBot.Models
{
    public class User : IUser
    {
        #region Ctor
        public User(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
        #endregion

        #region Properties
        public string UserName { get; set; }
        public string Password { get; set; }
        #endregion
    }
}
