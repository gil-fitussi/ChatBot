
namespace ChatBot.Models
{
    public class GeneralHelper
    {
        #region Methods
        /// <summary>
        /// Checking object type
        /// </summary>
        /// <returns>true if same type</returns>
        public static bool CheckIfType<T>(object obj)
        {
            return obj is T;
        }
        #endregion
    }
}

