using MySql.Data.MySqlClient;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SGULibraryManagement.DAO
{
    public class UserDAO : IDAO<long, UserDTO>
    {
        private MySqlConnection Connection => MySqlConnector.Instance?.Connection!;
        public string TableName { get; } = "thanhvien";

        private UserDTO FetchData(MySqlDataReader reader)
        {
            return new UserDTO()
            {
                Id = reader.GetInt64("MaTV"),
                HoTen = reader.GetString("HoTen"),
                Khoa = reader.GetString("Khoa"),
                Nganh = reader.GetString("Nganh"),
                Phone = reader.GetString("Phone")
            };
        }

        public List<UserDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName}";
            try
            {
                MySqlCommand command = new(query, Connection);
                command.Prepare();

                List<UserDTO> result = [];

                using var reader = command.ExecuteReader();
                Logger.Log($"Query: {query}");

                while (reader.Read())
                {
                    result.Add(FetchData(reader));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return [];
        }
        public UserDTO FindById(long id)
        {
            return null;
        }
        public UserDTO Create(UserDTO request)
        {
            return null;
        }
        public bool Update(long id, UserDTO request)
        {
            return false;
        }
        public bool Delete(long id)
        {
            return false;
        }

    }
}
