using MySql.Data.MySqlClient;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGULibraryManagement.Utilities;

namespace SGULibraryManagement.DAO
{
    public class StudyAreaDAO : IDAO<long, StudyAreaDTO>
    {
        private MySqlConnection Connection => MySqlConnector.Instance?.Connection!;
        public string TableName { get; } = "study_area";

        private StudyAreaDTO FetchData(MySqlDataReader reader)
        {
            return new StudyAreaDTO()
            {
                Id = reader.GetInt64("id"),
                MSSV = reader.GetInt64("mssv"),
                CheckinDate = reader.GetDateTime("check_in_date"),
                IsDeleted = reader.GetBoolean("is_deleted"),
            };
        }

        public List<StudyAreaDTO> FindByMSSV(long mssv)
        {
            string query = $"Select * from {TableName} where MSSV = {mssv}";
            try
            {
                MySqlCommand command = new(query, Connection);
                command.Prepare();

                List<StudyAreaDTO> result = [];

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

            return null!;
        }

        public StudyAreaDTO Create(StudyAreaDTO request)
        {
            string query = $@"Insert into {TableName} (mssv , check_in_date , is_deleted )
                            Values (@MSSV , @CheckInDate , @IsDeleted)";
            Logger.Log($"query ceate study area : {query}");

            try
            {
                using MySqlCommand command = new(query, Connection);
                command.Parameters.AddWithValue("@MSSV", request.MSSV);
                command.Parameters.AddWithValue("@CheckInDate", request.CheckinDate);
                command.Parameters.AddWithValue("@IsDeleted", false);
                command.Prepare();
                int rowsAffected = command.ExecuteNonQuery();


                if (rowsAffected > 0)
                {
                    return request;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return null!;
        }

        //these methods have not need yet
        public bool Delete(long id)
        {
            return false;
        }

        public List<StudyAreaDTO> GetAll(bool isActive)
        {
            return null;
        }

        public StudyAreaDTO FindById(long id)
        {
            return null;
        }

        public bool Update(long id, StudyAreaDTO request)
        {
            return false;
        }
    }
}
