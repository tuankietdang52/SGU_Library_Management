using MySql.Data.MySqlClient;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;
namespace SGULibraryManagement.DAO
{
    public class BorrowDevicesDAO : IDAO<long, BorrowDevicesDTO>
    {
        public string TableName { get; } = "borrow_devices";
        private MySqlConnection Connection => MySqlConnector.Instance?.Connection!;

        private BorrowDevicesDTO FetchData(MySqlDataReader r)
        {
            return new BorrowDevicesDTO()
            {
                Id = r.GetInt64("id"),
                UserId = r.GetInt64("user_id"),
                DeviceId = r.GetInt64("device_id"),
                Quantity = r.GetInt32("quantity"),
                DateCreate = r.GetDateTime("create_at"),
                DateBorrow = r.GetDateTime("date_borrow"),
                DateReturn = r.GetDateTime("date_return"),
                IsDeleted = r.GetBoolean("is_deleted"),
                IsReturn = r.GetBoolean("is_return")
            };
        }

        public List<BorrowDevicesDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = @IsDeleted";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@IsDeleted", !isActive);

                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        public List<BorrowDevicesDTO> FindByAccountId(long accountId)
        {
            string query = $"SELECT * FROM {TableName} WHERE user_id = @Id";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", accountId);
                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        public List<BorrowDevicesDTO> FindByDeviceId(long deviceId)
        {
            string query = $"SELECT * FROM {TableName} WHERE device_id = @Id";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", deviceId);
                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        public BorrowDevicesDTO FindById(long id)
        {
            string query = $"SELECT * FROM {TableName} WHERE id = @Id";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Prepare();

                List<BorrowDevicesDTO> result = [];

                using var reader = command.ExecuteReader();
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

        private void AddData(MySqlCommand command, BorrowDevicesDTO request)
        {
            command.Parameters.AddWithValue("@UserId", request.UserId);
            command.Parameters.AddWithValue("@DeviceId", request.DeviceId);
            command.Parameters.AddWithValue("@Quantity", request.Quantity);
            command.Parameters.AddWithValue("@CreateAt", request.DateCreate);
            command.Parameters.AddWithValue("@DateBorrow", request.DateBorrow);
            command.Parameters.AddWithValue("@DateReturn", request.DateReturn);
            command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);
        }

        public BorrowDevicesDTO Create(BorrowDevicesDTO request)
        {
            string query = $@"INSERT INTO {TableName} (user_id, device_id, quantity, create_at, date_borrow, date_return, is_deleted, is_return) 
                              VALUES (@UserId, @DeviceId, @Quantity, @CreateAt, @DateBorrow, @DateReturn, @IsDeleted, 0)";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                AddData(command, request);

                command.Prepare();

                command.ExecuteNonQuery();

                return request;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return null!;
        }

        public bool Update(long id, BorrowDevicesDTO request)
        {
            string query = @$"UPDATE {TableName}
                              SET user_id = @UserId,
                                  device_id = @DeviceId,
                                  quantity = @Quantity,
                                  create_at = @CreateAt,
                                  date_borrow = @DateBorrow,
                                  date_return = @DateReturn,
                                  is_deleted = @IsDeleted,
                                  is_return = @IsReturn
                              WHERE id = @Id";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                AddData(command, request);
                command.Parameters.AddWithValue("@IsReturn", request.IsReturn);
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
            string query = $"DELETE FROM {TableName} WHERE id = @Id";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Prepare();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
                return false;
            }
        }
    }
}
