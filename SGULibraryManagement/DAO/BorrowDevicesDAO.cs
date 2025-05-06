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
            DateTime dateReturn;
            if (r.IsDBNull(r.GetOrdinal("date_return")))
            {
                dateReturn = DateTime.MinValue;
            }
            else dateReturn = r.GetDateTime("date_return");

            return new BorrowDevicesDTO()
            {
                Id = r.GetInt64("id"),
                UserId = r.GetInt64("mssv"),
                DeviceId = r.GetInt64("device_id"),
                Code = r.GetString("code"),
                Quantity = r.GetInt32("quantity"),
                DateCreate = r.GetDateTime("create_at"),
                DateBorrow = r.GetDateTime("date_borrow"),
                DateReturn = dateReturn,
                DateReturnExpected = r.GetDateTime("date_return_expected"),
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

        #region Get By Borrow Date

        public List<BorrowDevicesDTO> GetAllByBorrowDate(DateTime date, bool fromStart)
        {
            if (fromStart) return GetAllByDateStart(date);
            else return GetAllByDateEnd(date);
        }

        private List<BorrowDevicesDTO> GetAllByDateStart(DateTime start)
        {
            string query = $@"SELECT * FROM {TableName}
                              WHERE DATE(date_borrow) >= DATE(@Start)
                              AND is_deleted = 0";

            try
            {
                MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Start", start);

                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        private List<BorrowDevicesDTO> GetAllByDateEnd(DateTime end)
        {
            string query = $@"SELECT * FROM {TableName}
                              WHERE DATE(date_borrow) <= DATE(@End)
                              AND is_deleted = 0";

            try
            {
                MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@End", end);

                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        public List<BorrowDevicesDTO> GetAllByBorrowDate(DateTime start, DateTime end)
        {
            string query = $@"SELECT * FROM {TableName} 
                              WHERE DATE(date_borrow) >= DATE(@Start) AND DATE(date_borrow) <= DATE(@End)
                              AND is_deleted = 0";
            try
            {
                MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Start", start);
                command.Parameters.AddWithValue("@End", end);

                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        #endregion

        public List<BorrowDevicesDTO> GetCurrentlyBorrowByDate(DateTime date, bool fromStart)
        {
            if (fromStart) return GetCurrentlyBorrowByDateStart(date);
            else return GetCurrentlyBorrowByDateEnd(date);
        }

        private List<BorrowDevicesDTO> GetCurrentlyBorrowByDateStart(DateTime start)
        {
            string query = $@"SELECT * FROM {TableName}
                              WHERE DATE(COALESCE(date_return, date_return_expected)) >= DATE(@Start)
                              AND is_deleted = 0 
                              AND is_return = 0";

            try
            {
                MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Start", start);

                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        private List<BorrowDevicesDTO> GetCurrentlyBorrowByDateEnd(DateTime end)
        {
            string query = $@"SELECT * FROM {TableName} 
                              WHERE DATE(date_borrow) <= DATE(@End)
                              AND is_deleted = 0
                              AND is_return = 0";

            try
            {
                MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@End", end);

                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        public List<BorrowDevicesDTO> GetCurrentlyBorrowByDate(DateTime start, DateTime end)
        {
            string query = $@"SELECT * FROM {TableName} 
                              WHERE NOT (@End < date_borrow OR @Start > DATE(COALESCE(date_return, date_return_expected)))
                              AND is_return = 0 
                              AND is_deleted = 0;";
            try
            {
                MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Start", start);
                command.Parameters.AddWithValue("@End", end);

                command.Prepare();

                List<BorrowDevicesDTO> result = [];

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

        public List<BorrowDevicesDTO> FindByAccountMssv(long mssv)
        {
            string query = $"SELECT * FROM {TableName} WHERE mssv = @Mssv AND is_deleted = 0";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Mssv", mssv);
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


        public BorrowDevicesDTO FindByCode(string code)
        {
            string query = $"SELECT * FROM {TableName} WHERE code = @Code AND is_deleted = 0";

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Code", code);
                command.Prepare();

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

        public List<BorrowDevicesDTO> FindByDeviceId(long deviceId)
        {
            string query = $"SELECT * FROM {TableName} WHERE device_id = @Id AND is_deleted = 0";
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
            string query = $"SELECT * FROM {TableName} WHERE id = @Id AND is_deleted = 0";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Prepare();

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
            command.Parameters.AddWithValue("@Code", request.Code);
            command.Parameters.AddWithValue("@Quantity", request.Quantity);
            command.Parameters.AddWithValue("@CreateAt", request.DateCreate);
            command.Parameters.AddWithValue("@DateBorrow", request.DateBorrow);
            command.Parameters.AddWithValue("@DateReturnExpected", request.DateReturnExpected);
            command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);
        }

        public BorrowDevicesDTO Create(BorrowDevicesDTO request)
        {
            string query = $@"INSERT INTO {TableName} (mssv, device_id, code, quantity, create_at, date_borrow, date_return_expected, is_deleted, is_return) 
                              VALUES (@UserId, @DeviceId, @Code, @Quantity, @CreateAt, @DateBorrow, @DateReturnExpected, @IsDeleted, 0)";

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
                              SET mssv = @UserId,
                                  device_id = @DeviceId,
                                  code = @Code,
                                  quantity = @Quantity,
                                  create_at = @CreateAt,
                                  date_borrow = @DateBorrow,
                                  date_return = @DateReturn,
                                  date_return_expected = @DateReturnExpected,
                                  is_deleted = @IsDeleted,
                                  is_return = @IsReturn
                              WHERE id = @Id";

            Logger.Log($"Query: {query}");
            
            try
            {
                using MySqlCommand command = new(query, Connection);
                AddData(command, request);
                command.Parameters.AddWithValue("@IsReturn", request.IsReturn);
                command.Parameters.AddWithValue("@DateReturn", request.DateReturn);
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

        public bool DeleteByStudentCode(long studentCode)
        {
            string query = $@"UPDATE {TableName} 
                              SET is_deleted = 1 
                              WHERE mssv = @Id";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", studentCode);
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

        public bool DeleteMultipleByStudentCode(List<long> studentCodes)
        {
            string query = $@"UPDATE {TableName} 
                              SET is_deleted = 1 
                              WHERE mssv IN (";

            foreach (var id in studentCodes)
            {
                query += $"{id}, ";
            }

            query = query.Trim()[..^1];
            query += ");";

            Logger.Log($"Query: {query}");
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected >= 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
                return false;
            }
        }
    }
}
