using MySql.Data.MySqlClient;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;

namespace SGULibraryManagement.DAO
{
    public class DeviceDAO : IDAO<long, DeviceDTO>
    {
        private MySqlConnection Connection => MySqlConnector.Instance?.Connection!;

        public string TableName { get; } = "devices";

        public DeviceDAO()
        {

        }

        private DeviceDTO FetchData(MySqlDataReader reader)
        {
            return new DeviceDTO()
            {
                Id = reader.GetInt64("id"),
                Name = reader.GetString("name"),
                Quantity = reader.GetInt32("quantity"),
                ImageSource = reader.GetString("img"),
                Description = reader.GetString("description"),
                IsDeleted = reader.GetBoolean("is_deleted"),
                IsAvailable = reader.GetBoolean("is_available")
            };
        }

        public DeviceDTO FindById(long id)
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

        public List<DeviceDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = @IsDeleted";
            Logger.Log($"Query: {query}");
            
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@IsDeleted", !isActive);
                command.Prepare();

                List<DeviceDTO> result = [];

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

        public List<Pair<DeviceDTO, int>> GetAllWithBorrowQuantity()
        {
            string query = $@"SELECT COALESCE(SUM(borrow_devices.quantity), 0) AS borrow_quantity, 
                        			 COALESCE(SUM(reservations.quantity), 0) AS reservation_quantity, 
                        			 {TableName}.*
                              FROM {TableName}
                              LEFT JOIN borrow_devices ON devices.id = borrow_devices.device_id
                              LEFT JOIN reservations ON devices.id = reservations.device_id
                              GROUP BY {TableName}.id;";

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                List<Pair<DeviceDTO, int>> result = [];

                using var reader = command.ExecuteReader();
                Logger.Log($"Query: {query}");

                while (reader.Read())
                {
                    int borrowQuantity = reader.GetInt32("borrow_quantity");
                    int reservationQuantity = reader.GetInt32("reservation_quantity");

                    result.Add(new(FetchData(reader), borrowQuantity + reservationQuantity));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return [];
        }

        private void AddData(MySqlCommand command, DeviceDTO request)
        {
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Quantity", request.Quantity);
            command.Parameters.AddWithValue("@Description", request.Description);
            command.Parameters.AddWithValue("@Img", request.ImageSource);
            command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);
            command.Parameters.AddWithValue("@IsAvailable", request.IsAvailable);
        }

        public DeviceDTO Create(DeviceDTO request)
        {
            string query = $@"INSERT INTO {TableName} (name, quantity, img, description, is_deleted, is_available) 
                              VALUES (@Name, @Quantity, @Img, @Description, @IsDeleted, @IsAvailable)";

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

        public DeviceDTO CreateV1(DeviceDTO request, MySqlTransaction transaction)
        {
            string query = $@"INSERT INTO {TableName} (name, quantity, img, description, is_deleted, is_available) 
                              VALUES (@Name, @Quantity, @Img, @Description, @IsDeleted, @IsAvailable)";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection, transaction);
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

        public bool CreateListDevice(List<DeviceDTO> listDevice)
        {
            MySqlTransaction tr = null;
            try
            {
                tr = this.Connection.BeginTransaction();
                foreach (DeviceDTO device in listDevice)
                {
                    var result = CreateV1(device, tr);
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

        public bool Update(long id, DeviceDTO request)
        {
            string query = $@"UPDATE {TableName} SET 
                           name = @Name,
                           quantity = @Quantity,
                           img = @Img,
                           description = @Description,
                           is_deleted = @IsDeleted,
                           is_available = @IsAvailable
                           WHERE id = @Id";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                AddData(command, request);
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


        public bool Delete(long id)
        {
            string query = $"UPDATE {TableName} SET is_deleted = 1 WHERE id = @Id";
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
