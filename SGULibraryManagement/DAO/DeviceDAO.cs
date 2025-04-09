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
            throw new NotImplementedException();
        }

        public bool Delete(long id)
        {
            throw new NotImplementedException();
        }
    }
}
