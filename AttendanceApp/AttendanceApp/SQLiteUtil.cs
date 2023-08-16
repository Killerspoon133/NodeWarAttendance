using Microsoft.Data.Sqlite;

namespace AttendanceApp
{
    public class SQLiteUtil
    {
        public SQLiteUtil()
        {

        }

        public SqliteConnection CreateConnection()
        {
            var SqliteConnection = new SqliteConnection("Data Source=database.db;");

            try
            {
                SqliteConnection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SQLite Create Connection exception: " + ex);
            }

            return SqliteConnection;
        }

        public void CreateTable(SqliteConnection conn)
        {
            var stringCommand = "CREATE TABLE IF NOT EXISTS WarRecords (" +
                "FamilyName VARCHAR(20), " +
                "GuildName VARCHAR(20)," +
                "WarDate BIGINT)";

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = stringCommand;
            cmd.ExecuteNonQuery();
        }

        public void InsertData(SqliteConnection conn, Record record)
        {            
            var sqlInsert = string.Format(@"INSERT INTO WarRecords(FamilyName, GuildName, WarDate) 
                                            VALUES('{0}', '{1}', {2});",
                                            record.FamilyName,
                                            record.GuildName,
                                            record.WarDate);

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlInsert;
            cmd.ExecuteNonQuery();
        }

        public List<Record> GetRecordsForFamilyName(SqliteConnection conn, string familyName) 
        {
            string sqlQuery = string.Format(@"SELECT * FROM WarRecords where FamilyName = '{0}'", familyName);

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlQuery;
            
            SqliteDataReader reader = cmd.ExecuteReader();

            List<Record> records = new List<Record>();
            Record currentRecord;

            while (reader.Read()) 
            {
                currentRecord = new Record() 
                {
                    FamilyName = (string)reader["FamilyName"],
                    GuildName = (string)reader["GuildName"],
                    WarDate = (long)reader["WarDate"]
                };

                records.Add(currentRecord);
            }

            return records;
        }

        public List<string> GetDistinctListOfFamilyNames(SqliteConnection conn) 
        {
            string sqlQuery = "SELECT DISTINCT FamilyName from WarRecords";
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlQuery;

            SqliteDataReader reader = cmd.ExecuteReader();

            List<string> familyNames = new List<string>();

            while (reader.Read()) 
            {
                familyNames.Add(reader.GetString(0));
            }

            return familyNames;
        }

        public void ReadData(SqliteConnection conn)
        {
            var stringCommand = "SELECT * FROM SampleTable";
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = stringCommand;

            SqliteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Reading Data...");
                var stringReader = reader.GetString(0);
                Console.WriteLine(stringReader);
            }
            conn.Close();
        }
    }
}
