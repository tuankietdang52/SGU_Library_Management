using DotNetEnv;
using MySql.Data.MySqlClient;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Helper
{
    /// <summary>
    /// A helper class for database connection
    /// </summary>
    public class MySqlConnector
    {
        private static MySqlConnector? instance;
        public static MySqlConnector? Instance
        {
            get
            {
                instance ??= new MySqlConnector();
                return instance;
            }
            private set => instance = value;
        }

        public MySqlConnection? Connection { get; private set; }

        private MySqlConnector()
        {
            OpenConnection();
        }

        private string GetConnectionString()
        {
            string host = Env.GetString("DATABASE_HOST");
            string port = Env.GetString("DATABASE_PORT");
            string database = Env.GetString("DATABASE");
            string user = Env.GetString("DATABASE_USER");
            string password = Env.GetString("DATABASE_PASSWORD");

            return $"Server={host};Port={port};Database={database};User ID={user};Password={password};";
        }

        private void OpenConnection()
        {
            Connection = new(GetConnectionString());

            try
            {
                Connection.Open();
                Logger.Log("Success", "Connection Successful");
            }
            catch (Exception ex)
            {
                Logger.Log("Fail", "Connection Failed");
                Logger.LogError(ex.Message);
            }
        }
    }
}
