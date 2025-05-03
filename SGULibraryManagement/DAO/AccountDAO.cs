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
                Mssv = reader.GetInt64("mssv"),
                Password = reader.GetString("password"),
                FirstName = reader.GetString("first_name"),
                LastName = reader.GetString("last_name"),
                Phone = reader.GetString("phone"),
                Email = reader.GetString("email"),
                IdRole = reader.GetInt64("role_id"),
                Faculty = reader.GetString("faculty"),
                Major = reader.GetString("major"),
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

        public AccountDTO FindById(long mssv)
        {
            string query = $"SELECT * FROM {TableName} WHERE mssv = {mssv}";

            try
            {
                MySqlCommand command = new(query, Connection);
                command.Prepare();

                using MySqlDataReader reader = command.ExecuteReader();
                Logger.Log($"Query: {query}");

                if (reader.Read()) return FetchData(reader);
                else return null!;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return null!;

        }
        public AccountDTO Create(AccountDTO request)
        {
            string query = $@"INSERT INTO {TableName} (mssv, password, first_name, last_name, phone, email, role_id, faculty, major, avt, is_deleted) 
                            VALUES (@Mssv, @Password, @First_name, @Last_name, @Phone, @Email, @Role, @Faculty, @Major, @Avt, @IsActive)";

            Logger.Log(query);
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@mssv", request.Mssv);
                command.Parameters.AddWithValue("@Password", request.Password);
                command.Parameters.AddWithValue("@First_name", request.FirstName);
                command.Parameters.AddWithValue("@Last_name", request.LastName);
                command.Parameters.AddWithValue("@Phone", request.Phone);
                command.Parameters.AddWithValue("@Email", request.Email);
                command.Parameters.AddWithValue("@Role", request.IdRole);
                command.Parameters.AddWithValue("@Faculty", request.Faculty);
                command.Parameters.AddWithValue("@Major", request.Major);
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
            }
            return null!;
        }


        public AccountDTO CreateV1(AccountDTO request, MySqlTransaction transaction)
        {
            string query = $@"INSERT INTO {TableName} (mssv, password, first_name, last_name, phone, email, role_id, faculty, major, avt, is_deleted) 
                            VALUES (@Mssv, @Password, @First_name, @Last_name, @Phone, @Email, @Role, @Faculty, @Major, @Avt, @IsActive)";

            Logger.Log(query);
            try
            {
                using MySqlCommand command = new(query, Connection, transaction);
                command.Parameters.AddWithValue("@mssv", request.Mssv);
                command.Parameters.AddWithValue("@Password", request.Password);
                command.Parameters.AddWithValue("@First_name", request.FirstName);
                command.Parameters.AddWithValue("@Last_name", request.LastName);
                command.Parameters.AddWithValue("@Phone", request.Phone);
                command.Parameters.AddWithValue("@Email", request.Email);
                command.Parameters.AddWithValue("@Role", request.IdRole);
                command.Parameters.AddWithValue("@Faculty", request.Faculty);
                command.Parameters.AddWithValue("@Major", request.Major);
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
            }
            return null!;
        }
        public bool CreateListAccount(List<AccountDTO> listAccount)
        {
            MySqlTransaction tr = null;
            try
            {
                tr = this.Connection.BeginTransaction();
                foreach (AccountDTO account in listAccount)
                {
                    var result = CreateV1(account, tr);
                    if (result == null)
                    {
                        tr.Rollback();
                        return false;
                    }
                    
                }

                tr.Commit();
                return true;
            }
            catch (MySqlException ex)
            {
                tr.Rollback();
                return false;
            }
        }


        public bool Update(long mssv, AccountDTO request)
        {
            string query = $@"UPDATE {TableName}
                            SET password = @Password,
                                first_name = @First_name,
                                last_name = @Last_name,
                                phone = @Phone,
                                email = @Email, 
                                role_id = @RoleId,
                                avt = @Avt,
                                is_deleted = @Is_Deleted 
                            WHERE mssv = @Mssv";

            Logger.Log(query);
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Password", request.Password);
                command.Parameters.AddWithValue("@First_name", request.FirstName);
                command.Parameters.AddWithValue("@Last_name", request.LastName);
                command.Parameters.AddWithValue("@Phone", request.Phone);
                command.Parameters.AddWithValue("@Email", request.Email);
                command.Parameters.AddWithValue("@RoleId", request.IdRole);
                command.Parameters.AddWithValue("@Avt", request.Avatar);
                command.Parameters.AddWithValue("@Is_Deleted", request.IsDeleted);
                command.Parameters.AddWithValue("@Mssv", request.Mssv);

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

        public AccountDTO? FindByUsername(long mssv)
        {
            string query = $"SELECT * FROM {TableName} WHERE mssv = @Mssv AND is_deleted = 0";

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Mssv", mssv);
                command.Prepare();

                using MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return FetchData(reader);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            return null;
        }

        public bool Delete(long mssv)
        {
            string query = $@"UPDATE {TableName} 
                            SET is_deleted = 1 
                            WHERE mssv = @Mssv";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Mssv", mssv);
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
