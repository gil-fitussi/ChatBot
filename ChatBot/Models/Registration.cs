using ChatBot.Interface;
using Serilog;
using System;


namespace ChatBot.Models
{
    public sealed class Registration : IRegistration
    {
        private readonly IDbQuery _dbQuery;

        #region Ctor
        public Registration(IDbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get User Login\SignUp Path
        /// </summary>
        public ResultSet CheckUserLoginDecision()
        {
            Console.WriteLine("Welcome. Please Press (1) For Login (2) For Sign Up");
            var operation = Console.ReadLine();

            if (!operation.Equals("1") && !operation.Equals("2"))
                CheckUserLoginDecision();

            if (operation.Equals("1"))
            {
                return LoginOperation();
            }
            else
                return SignUpOperation();
        }

        /// <summary>
        /// Login Operation
        /// </summary>
        /// <returns></returns>
        public ResultSet LoginOperation()
        {
            bool tryAgain = true;
            ResultSet result = null;

            while (tryAgain)
            {
                Console.WriteLine("Please Enter Username:");
                var userName = Console.ReadLine();
                Console.WriteLine("Please Enter Password:");
                var password = Console.ReadLine();

                User user = new User(userName, password);

                try
                {
                    //Save User To Json DB
                    result = _dbQuery.Login(user);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }

                if (result.Status != Status.Success)
                {
                    if (GeneralHelper.CheckIfType<string>(result.ReturnResult))
                        Console.WriteLine(result.ReturnResult.ToString());
                }
                else
                    tryAgain = false;
            }

            return result;
        }

        /// <summary>
        /// Sign Up Method
        /// </summary>
        public ResultSet SignUpOperation()
        {
            bool tryAgain = true;
            ResultSet result = null;

            while (tryAgain)
            {
                Console.WriteLine("Please Enter Username:");
                var userName = Console.ReadLine();
                Console.WriteLine("Please Enter Password:");
                var password = Console.ReadLine();

                if (string.IsNullOrEmpty(userName))
                {
                    Console.WriteLine("Please specify valid username");
                    continue;
                }

                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Please specify valid password");
                    continue;
                }

                User user = new User(userName, password);

                try
                {
                    //Save User To Json DB
                    result = _dbQuery.AddUser(user);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }

                if (result.Status != Status.Success)
                {
                    if (GeneralHelper.CheckIfType<string>(result.ReturnResult))
                        Console.WriteLine(result.ReturnResult.ToString());
                }
                else
                    tryAgain = false;
                
            }

            return result;
        }

        #endregion
    }
}
