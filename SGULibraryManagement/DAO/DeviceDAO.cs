using MySql.Data.MySqlClient;
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
    public class DeviceDAO : IDAO<long, DeviceDTO>
    {
        private MySqlConnection Connection => MySqlConnector.Instance?.Connection!;

        public string TableName { get; } = "devicess";

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
                IsDeleted = reader.GetBoolean("is_deleted"),
                IsAvailable = reader.GetBoolean("is_avaible")
            };
        }

        public DeviceDTO FindById(long id)
        {
            throw new NotImplementedException();
        }

        public List<DeviceDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = {(isActive ? 0 : 1)}";
            Logger.Log($"Query: {query}");
            
            try
            {
                MySqlCommand command = new(query, Connection);
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
            string query = "SELECT COUNT(device_id) AS borrow_quantity, devicess.* " +
                           "FROM borrow_devices INNER JOIN devicess ON devicess.id = borrow_devices.device_id " +
                           "GROUP BY device_id";

            try
            {
                MySqlCommand command = new(query, Connection);
                command.Prepare();

                List<Pair<DeviceDTO, int>> result = [];

                using var reader = command.ExecuteReader();
                Logger.Log($"Query: {query}");

                while (reader.Read())
                {
                    int borrowQuantity = reader.GetInt32("borrow_quantity");
                    result.Add(new(FetchData(reader), borrowQuantity));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return [];
        }

        public DeviceDTO Create(DeviceDTO request)
        {
            string query = $"INSERT INTO {TableName} (name, quantity, img, is_deleted, is_avaible) VALUES " +
                           $"('{request.Name}', {request.Quantity}, '{request.ImageSource}', {(request.IsDeleted ? 1 : 0)}, {(request.IsAvailable ? 1 : 0)})";
            Logger.Log($"Query: {query}");

            try
            {
                MySqlCommand command = new(query, Connection);
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

        public bool Update(long id, DeviceDTO request)
        {
            string query = $"UPDATE {TableName} SET " +
                           $"name = '{request.Name}', " +
                           $"quantity = {request.Quantity}, " +
                           $"img = '{request.ImageSource}', " +
                           $"is_deleted = {(request.IsDeleted ? 1 : 0)}, " +
                           $"is_avaible = {(request.IsAvailable ? 1 : 0)} " +
                           $"WHERE id = {id}";
            Logger.Log($"Query: {query}");

            try
            {
                MySqlCommand command = new(query, Connection);
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
            string query = $"UPDATE {TableName} SET is_deleted = 1 WHERE id = {id}";
            Logger.Log($"Query: {query}");

            try
            {
                MySqlCommand command = new(query, Connection);
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
