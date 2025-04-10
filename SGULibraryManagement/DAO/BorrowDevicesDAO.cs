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
                UserName = r.GetString("username"),
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
                MySqlCommand command = new(query, Connection);
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
            return null;
        }
        public BorrowDevicesDTO Create(BorrowDevicesDTO request)
        {
            return null;
        }

        public bool Update(long id, BorrowDevicesDTO request)
        {
            return false;
        }
        public bool Delete(long id)
        {
            return false;
        }


    }
}
