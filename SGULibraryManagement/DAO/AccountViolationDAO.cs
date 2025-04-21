using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.DAO
{
    public class AccountViolationDAO : IDAO<long, AccountViolationDTO>
    {
        public string TableName => "account_violation";
        private MySqlConnection Connection => MySqlConnector.Instance!.Connection!;

        private AccountViolationDTO FetchData(MySqlDataReader reader)
        {
            return new AccountViolationDTO()
            {
                Id = reader.GetInt64("id"),
                UserId = reader.GetInt64("user_id"),
                ViolationId = reader.GetInt64("violation_id"),
                DateCreate = reader.GetDateTime("create_at"),
                IsDeleted = reader.GetBoolean("is_deleted")
            };
        }

        public AccountViolationDTO FindById(long id)
        {
            string query = $"SELECT * FROM {TableName} WHERE id = {id}";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                using var reader = command.ExecuteReader();

                if (reader.Read()) return FetchData(reader);
                else return null!;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return null!;
        }

        public List<AccountViolationDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = {(isActive ? 0 : 1)}";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                List<AccountViolationDTO> result = [];

                using var reader = command.ExecuteReader();

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

        public AccountViolationDTO Create(AccountViolationDTO request)
        {
            string query = $@"INSERT INTO {TableName} (user_id, violation_id, create_at, is_deleted) 
                              VALUES ({request.UserId}, {request.ViolationId}, @DateCreate, @IsDeleted)";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@DateCreate", request.DateCreate);
                command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);

                command.Prepare();
                int row = command.ExecuteNonQuery();

                return row > 0 ? request : null!;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return null!;
        }

        public bool Update(long id, AccountViolationDTO request)
        {
            string query = $@"UPDATE {TableName} 
                              SET user_id = {request.UserId}, 
                                  violation_id = {request.ViolationId}, 
                                  create_at = @DateCreate, 
                                  is_deleted = @IsDeleted
                              WHERE id = {request.Id}";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@DateCreate", request.DateCreate);
                command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);

                command.Prepare();
                int row = command.ExecuteNonQuery();

                return row > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return false;
        }

        public bool Delete(long id)
        {
            string query = $@"UPDATE {TableName}
                              SET is_deleted = 1
                              WHERE id = {id}";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                int row = command.ExecuteNonQuery();
                return row > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return false;
        }

        public AccountViolationDTO? IsAccountLocked(long accountId)
        {
            string query = $"SELECT * FROM {TableName} WHERE user_id = {accountId} AND is_deleted = 0";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                using var reader = command.ExecuteReader();
                if (reader.Read()) return FetchData(reader);
                else return null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return null;
        }

        public bool IsRuleViolatedByUser(long violationId)
        {
            string query = $"SELECT * FROM {TableName} WHERE violation_id = {violationId} AND is_deleted = 0";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                using var reader = command.ExecuteReader();
                return reader.Read();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return false;
        }
    }
}
