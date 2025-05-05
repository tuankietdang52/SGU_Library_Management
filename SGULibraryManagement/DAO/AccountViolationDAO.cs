using MySql.Data.MySqlClient;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;
using System.Diagnostics.CodeAnalysis;

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
                UserId = reader.GetInt64("mssv"),
                ViolationId = reader.GetInt64("violation_id"),
                DateCreate = reader.GetDateTime("create_at"),
                Status = Enum.Parse<AccountViolationStatus>(reader.GetString("status")),
                BanExpired = reader.GetDateTime("ban_expired"),
                Compensation = reader.GetInt64("compensation"),
                IsDeleted = reader.GetBoolean("is_deleted")
            };
        }

        public AccountViolationDTO FindById(long id)
        {
            string query = $"SELECT * FROM {TableName} WHERE id = @Id";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", id);
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

        public List<AccountViolationDTO> FindByAccountId(long accountId)
        {
            string query = $"SELECT * FROM {TableName} WHERE mssv = @UserId";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@UserId", accountId);
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

        public List<AccountViolationDTO> FindByViolationId(long violationId)
        {
            string query = $"SELECT * FROM {TableName} WHERE violation_id = @ViolationId";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@ViolationId", violationId);
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

        public List<AccountViolationDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = @IsDeleted";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@IsDeleted", !isActive);

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

        private void AddData(MySqlCommand command, AccountViolationDTO request)
        {
            command.Parameters.AddWithValue("@Mssv", request.UserId);
            command.Parameters.AddWithValue("@ViolationId", request.ViolationId);
            command.Parameters.AddWithValue("@DateCreate", request.DateCreate);
            command.Parameters.AddWithValue("@BanExpired", request.BanExpired);
            command.Parameters.AddWithValue("@Compensation", request.Compensation);
            command.Parameters.AddWithValue("@Status", request.Status.ToString());
            command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);
        }

        public AccountViolationDTO Create(AccountViolationDTO request)
        {
            string query = $@"INSERT INTO {TableName} (mssv, violation_id, create_at, ban_expired, status, compensation, is_deleted) 
                              VALUES (@Mssv, @ViolationId, @DateCreate, @BanExpired, @Compensation, @Status, @IsDeleted)";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                AddData(command, request);

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
                              SET mssv = @Mssv, 
                                  violation_id = @ViolationId, 
                                  status = @Status, 
                                  ban_expired = @BanExpired,
                                  compensation = @Compensation, 
                                  create_at = @DateCreate, 
                                  is_deleted = @IsDeleted
                              WHERE id = @Id";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                AddData(command, request);
                command.Parameters.AddWithValue("@Id", id);

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
                              WHERE id = @Id";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", id);
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
            string query = $"SELECT * FROM {TableName} WHERE mssv = @AccountId AND DATE(ban_expired) > CURDATE() ORDER BY ABS(DATEDIFF(create_at, CURDATE())) LIMIT 1";
            Logger.Log($"accountId truyen vao violation :{accountId}");
            Logger.Log($"Query cua vialation: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@AccountId", accountId);
                command.Prepare();

                using var reader = command.ExecuteReader();
                if (reader.Read()) {
                    AccountViolationDTO rs = FetchData(reader);
                    return rs;
                }
                    
                else return null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return null;
        }

        public HashSet<AccountViolationDTO> GetAllLockedUsers()
        {
            string query = $@"SELECT * FROM account_violation 
                              WHERE DATE(ban_expired) > CURDATE() AND is_deleted = 0
                              ORDER BY ABS(DATEDIFF(create_at, CURDATE()))";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                HashSet<AccountViolationDTO> result = new(new AccountViolationUserIdComparer());
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

        public bool IsRuleViolatedByUser(long violationId)
        {
            string query = $@"SELECT * FROM {TableName} 
                              WHERE violation_id = {violationId} 
                              AND DATE(ban_expired) > CURDATE()
                              AND is_deleted = 0";
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

    public class AccountViolationUserIdComparer : IEqualityComparer<AccountViolationDTO>
    {
        public bool Equals(AccountViolationDTO? x, AccountViolationDTO? y)
        {
            return x.UserId == y.UserId;
        }

        public int GetHashCode([DisallowNull] AccountViolationDTO obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
