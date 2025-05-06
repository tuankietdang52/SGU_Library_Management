using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using SGULibraryManagement.BUS;
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
    public class ViolationDAO : IDAO<long, ViolationDTO>
    {
        public string TableName => "violations";
        private MySqlConnection Connection => MySqlConnector.Instance!.Connection!;

        private ViolationDTO FetchData(MySqlDataReader reader)
        {
            return new ViolationDTO()
            {
                Id = reader.GetInt64("id"),
                Name = reader.GetString("name"),
                Description = reader.GetString("description"),
                IsDeleted = reader.GetBoolean("is_deleted")
            };
        }

        public ViolationDTO FindById(long id)
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

        public List<ViolationDTO> GetAll(bool isActive)
        {
            string query = $"SELECT * FROM {TableName} WHERE is_deleted = @IsDeleted";
            Logger.Log($"Query: {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@IsDeleted", !isActive);
                command.Prepare();

                List<ViolationDTO> result = [];

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

        public List<Pair<ViolationDTO, int>> GetAllWithViolationCount()
        {
            string query = $@"SELECT COUNT(mssv) as violation_count, violations.*
                              FROM account_violation
                              INNER JOIN {TableName} ON violations.id = violation_id AND account_violation.is_deleted = 0
                              GROUP BY violation_id";

            Logger.Log(query);

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Prepare();

                List<Pair<ViolationDTO, int>> result = [];
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new(FetchData(reader), reader.GetInt32("violation_count")));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace!);
            }

            return [];
        }

        private void AddData(MySqlCommand command, ViolationDTO request)
        {
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Description", request.Description);
            command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);
        }

        public ViolationDTO Create(ViolationDTO request)
        {
            string query = $"INSERT INTO {TableName} (name, description, is_deleted) " +
                           $"VALUES (@Name, @Description, @IsDeleted)";

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

        public bool Update(long id, ViolationDTO request)
        {
            string query = $@"UPDATE {TableName} 
                              SET name = @Name,
                                  description = @Description,
                                  is_deleted = @IsDeleted
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

        public bool DeleteMultiple(List<long> ids)
        {
            string item = "(";

            foreach (var id in ids)
            {
                item += $"{id}, ";
            }

            item = item.Trim()[..^1];
            item += ");";

            string query = $@"UPDATE {TableName} 
                              SET is_deleted = 1
                              WHERE id In {item}";

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
