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
    public class RoleDAO : IDAO<long, RoleDTO>
    {
        private MySqlConnection Connection => MySqlConnector.Instance?.Connection!;
        public string TableName { get; } = "roles";

        private RoleDTO FetchData(MySqlDataReader reader)
        {
            return new RoleDTO()
            {
                Id = reader.GetInt64("id"),
                Name = reader.GetString("name"),
                IsDeleted = reader.GetBoolean("is_deleted"),
            };
        }

        public List<RoleDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = @IsDeleted";

            try
            {
                MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@IsDeleted", !isActive);
                command.Prepare();

                List<RoleDTO> result = [];

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
        public RoleDTO FindById(long id)
        {
            return null;
        }
        public RoleDTO Create(RoleDTO request)
        {
            return null;
        }
        public bool Update(long id, RoleDTO request)
        {
            return false;
        }
        public bool Delete(long id)
        {
            return false;
        }
    }
}
