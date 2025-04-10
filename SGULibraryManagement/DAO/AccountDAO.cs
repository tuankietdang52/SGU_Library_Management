using MySql.Data.MySqlClient;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SGULibraryManagement.DAO
{
    public class AccountDAO : IDAO<long, AccountDTO>
    {
        private MySqlConnection Connection => MySqlConnector.Instance?.Connection!;
        public string TableName { get; } = "accounts";

        private AccountDTO FetchData(MySqlDataReader reader)
        {
            return new AccountDTO()
            {
                Username = reader.GetString("username"),
                Password = reader.GetString("password"),
                FirstName = reader.GetString("first_name"),
                LastName = reader.GetString("last_name"),
                Phone = reader.GetString("phone"),
                IdRole = reader.GetInt64("role_id"),
                Avatar = reader.GetString("avt"),
                IsDeleted = reader.GetBoolean("is_deleted")
            };
        }

        public List<AccountDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = {(isActive ? 0 : 1)}";
            try
            {
                MySqlCommand command = new(query, Connection);
                command.Prepare();

                List<AccountDTO> result = [];

                using MySqlDataReader reader = command.ExecuteReader();
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
        public AccountDTO FindById(long id)
        {
            return null;
        }
        public AccountDTO Create(AccountDTO request)
        {
            string query = $@"INSERT INTO {TableName}
                            VALUES (@Username, @Password, @First_name, @Last_name, @Phone, @Role, @Avt, @IsActive)";

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Username", request.Username);
                command.Parameters.AddWithValue("@Password", request.Password);
                command.Parameters.AddWithValue("@First_name", request.FirstName);
                command.Parameters.AddWithValue("@Last_name", request.LastName);
                command.Parameters.AddWithValue("@Phone", request.Phone);
                command.Parameters.AddWithValue("@Role", request.IdRole);
                command.Parameters.AddWithValue("@Avt", request.Avatar);
                command.Parameters.AddWithValue("@IsActive", request.IsDeleted);

                command.Prepare();
                int rowsAffected = command.ExecuteNonQuery();


                if (rowsAffected > 0)
                {
                    return request;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
            return null;
        }

        public bool Update(long id, AccountDTO request)
        {
            string query = $@"UPDATE {TableName}
                            SET password = @Password,
                                first_name = @First_name,
                                last_name = @Last_name,
                                phone = @Phone,
                                role_id = @RoleId,
                                avt = @Avt,
                                is_deleted = @Is_Deleted
                            WHERE username = @Username";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Password", request.Password);
                command.Parameters.AddWithValue("@First_name", request.FirstName);
                command.Parameters.AddWithValue("@Last_name", request.LastName);
                command.Parameters.AddWithValue("@Phone", request.Phone);
                command.Parameters.AddWithValue("@RoleId", request.IdRole);
                command.Parameters.AddWithValue("@Avt", request.Avatar);
                command.Parameters.AddWithValue("@Is_Deleted", request.IsDeleted);
                command.Parameters.AddWithValue("@Username", request.Username); 

                command.Prepare();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            return false;
        }
        public bool Delete(long id)
        {
            return false;
        }

        public bool DeleteV2(string username)
        {
            string query = $@"UPDATE {TableName}
                            SET is_deleted = 1
                            WHERE username = @Username";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Prepare();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return false;
        }

    }
}
