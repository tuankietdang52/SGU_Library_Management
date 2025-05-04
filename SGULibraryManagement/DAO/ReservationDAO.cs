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
    public class ReservationDAO : IDAO<long, ReservationDTO>
    {
        private MySqlConnection Connection => MySqlConnector.Instance?.Connection!;
        public string TableName => "reservations";

        private ReservationDTO FetchData(MySqlDataReader r)
        {
            return new ReservationDTO()
            {
                Id = r.GetInt64("id"),
                UserId = r.GetInt64("user_id"),
                DeviceId = r.GetInt64("device_id"),
                Quantity = r.GetInt32("quantity"),
                DateCreate = r.GetDateTime("create_at"),
                DateBorrow = r.GetDateTime("date_borrow"),
                DateReturn = r.GetDateTime("date_return"),
                IsCheckedOut = r.GetBoolean("is_checked_out"),
                IsDeleted = r.GetBoolean("is_deleted"),
            };
        }

        public List<ReservationDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = @IsDeleted";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@IsDeleted", !isActive);

                command.Prepare();

                List<ReservationDTO> result = [];

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

        public List<ReservationDTO> FindByAccountId(long accountId)
        {
            string query = $"SELECT * FROM {TableName} WHERE mssv = @Mssv";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Mssv", accountId);
                command.Prepare();

                List<ReservationDTO> result = [];

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

        public List<ReservationDTO> FindByDeviceId(long deviceId)
        {
            string query = $"SELECT * FROM {TableName} WHERE device_id = @Id";
            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@Id", deviceId);
                command.Prepare();

                List<ReservationDTO> result = [];

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

        public ReservationDTO FindById(long id)
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

        private void AddData(MySqlCommand command, ReservationDTO request)
        {
            command.Parameters.AddWithValue("@UserId", request.UserId);
            command.Parameters.AddWithValue("@DeviceId", request.DeviceId);
            command.Parameters.AddWithValue("@Quantity", request.Quantity);
            command.Parameters.AddWithValue("@CreateAt", request.DateCreate);
            command.Parameters.AddWithValue("@DateBorrow", request.DateBorrow);
            command.Parameters.AddWithValue("@DateReturn", request.DateReturn);
            command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);
        }

        public ReservationDTO Create(ReservationDTO request)
        {
            string query = $@"INSERT INTO {TableName} (user_id, device_id, quantity, create_at, date_borrow, date_return, is_checked_out, is_deleted) 
                              VALUES (@UserId, @DeviceId, @Quantity, @CreateAt, @DateBorrow, @DateReturn, 0, @IsDeleted)";

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

        public bool Update(long id, ReservationDTO request)
        {
            string query = @$"UPDATE {TableName}
                              SET user_id = @UserId,
                                  device_id = @DeviceId,
                                  quantity = @Quantity,
                                  create_at = @CreateAt,
                                  date_borrow = @DateBorrow,
                                  date_return = @DateReturn,
                                  is_checked_out = @IsCheckedOut,
                                  is_deleted = @IsDeleted
                              WHERE id = @Id";

            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                AddData(command, request);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@IsCheckedOut", request.IsCheckedOut);

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

        public bool Checkout(long id)
        {
            string query = @$"UPDATE {TableName}
                              SET is_checked_out = 1
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
