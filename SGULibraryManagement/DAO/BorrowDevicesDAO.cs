using MySql.Data.MySqlClient;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
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
                CreateAt = r.GetDateTime("create_at"),
                DateBorrow = r.GetDateTime("date_borrow"),
                DateReturn = r.GetDateTime("date_return"),
                IsDeleted = r.GetBoolean("is_deleted")
            };
        }

        public List<BorrowDevicesDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName}";
            try
            {
                using MySqlCommand command = new(query, Connection);
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
            string query = $"SELECT * FROM {TableName} WHERE device_id = {deviceId}";
            try
            {
                using MySqlCommand command = new(query, Connection);
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
            string query = $"SELECT * FROM {TableName} WHERE id = {id}";
            try
            {
                using MySqlCommand command = new(query, Connection);
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

        public BorrowDevicesDTO Create(BorrowDevicesDTO request)
        {
            string query = $"INSERT INTO {TableName} (user_id, device_id, quantity, create_at, date_borrow, date_return, is_deleted) " +
                           $"VALUES ({request.UserId}, {request.DeviceId}, {request.Quantity}, {request.CreateAt}, {request.DateBorrow}, {request.DateReturn}, {(request.IsDeleted ? 1 : 0)})";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
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
                              SET user_id = {request.UserId},
                                  device_id = {request.DeviceId},
                                  quantity = {request.Quantity},
                                  create_at = {request.CreateAt},
                                  date_borrow = {request.DateBorrow},
                                  date_return = {request.DateReturn},
                                  is_deleted = {(request.IsDeleted ? 1 : 0)}
                              WHERE id = {request.Id}";

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

        public bool Delete(long id)
        {
            string query = $"DELETE FROM {TableName} WHERE id = {id}";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
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
