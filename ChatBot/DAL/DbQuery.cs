using ChatBot.Interface;
using ChatBot.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChatBot.DB
{
    public class DbQuery : IDbQuery
    {
        private readonly IConfigurationRoot _configRoot;
        private readonly string _path;

        #region Ctor
        public DbQuery(IConfigurationRoot configRoot)
        {
            _configRoot = configRoot;
            _path = GetDBPath();
            Users = GetUsers();
        }
        #endregion

        #region Properties
        private List<User> Users { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Login user to chat
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultSet Login(User user)
        {
            var dbUser = Users.FirstOrDefault(x => x.UserName == user.UserName);

            if (dbUser == null)
                return new ResultSet(Status.AuthFailed, "User does not exist");

            string password = string.Empty;
            try
            {
                var key = _configRoot.GetSection("Key").Value;
                password = AesHelper.DecryptString(key, dbUser.Password);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ResultSet(Status.Failed, "Oops, Somthing Wrong..");
            }

            if(!password.Equals(user.Password))
                return new ResultSet(Status.AuthFailed, "Password is incorrect");

            return new ResultSet(Status.Success, user);
        }

        /// <summary>
        /// Add user to DB
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultSet AddUser(User user)
        {
            try
            {
                if (Users.Any(x => x.UserName == user.UserName))
                    return new ResultSet(Status.UserExists, "User Already Exists");

               
                    var key = _configRoot.GetSection("Key").Value;
                    user.Password = AesHelper.EncryptString(key, user.Password);

                    Users.Add(user);
                    string newJson = JsonConvert.SerializeObject(Users, Formatting.Indented);

                    // write JSON directly to a file
                    File.WriteAllText(_path, newJson);
                
                

                return new ResultSet(Status.Success, user);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ResultSet(Status.Failed, "Oops, Somthing Wrong..");
            }
        }

        /// <summary>
        /// Get all users from DB
        /// </summary>
        /// <returns></returns>
        private List<User> GetUsers()
        {
            try
            {
                string json = File.ReadAllText(_path);
                if (!string.IsNullOrEmpty(json))
                    return JsonConvert.DeserializeObject<List<User>>(json);
                return new List<User>();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Get DB path
        /// </summary>
        /// <returns></returns>
        private static string GetDBPath()
        {
            string fileName = "T_Users.txt";
            string path = Path.Combine(Environment.CurrentDirectory, @"DB\", fileName);
            return path;
        }
        #endregion
    }
}
