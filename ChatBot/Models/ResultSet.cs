
namespace ChatBot.Models
{
    public class ResultSet
    {
        #region Ctor
        public ResultSet(Status status, object returnResult)
        {
            Status = status;
            ReturnResult = returnResult;
        }
        #endregion

        #region Properties
        public Status Status { get; set; }
        public object ReturnResult { get; set; }
        #endregion
    }

    public enum Status
    {
        Success,
        Failed,
        UserExists,
        AuthFailed
    }
}
