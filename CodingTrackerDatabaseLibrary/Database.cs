using System.Configuration;
using System.Data.SQLite;
using CodingSessionLibrary;
using Dapper;

namespace CodingTrackerDatabaseLibrary
{
    public class Database
    {
        private string? databseConnection = ConfigurationManager.AppSettings.Get("DatabaseConnection");

        public void CreateTable()
        {
            using (var connection = new SQLiteConnection(databseConnection))
            {
                connection.Open();  
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS codeLog(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    startTime TEXT NOT NULL,    
                    endTime TEXT NOT NULL,
                    duration TEXT NOT NULL);";

                connection.Execute(createTableQuery);
            }
        }

        public void ViewTable()
        {
            using(var connection = new SQLiteConnection(databseConnection))
            {
                var sql = "SELECT * FROM codeLog";
                var codingSession = connection.Query<CodingSession>(sql);
                Console.WriteLine("id\tStart Time\tEnd Time\tDuration");
                foreach(var session in codingSession)
                {
                    Console.WriteLine($"{session.Id}\t{session.StartTime}\t{session.EndTime}\t{session.Duration}");
                }
            } 
        }

        public void InsertRecord()
        {
            var userInput = new UserInput();
            using(var connection = new SQLiteConnection(databseConnection))
            {
                var sql = "INSERT INTO codeLog(startTime, endTime, duration)  VALUES(@StartTime, @EndTime, @Duration)";
                var session = new CodingSession
                {
                    StartTime = userInput.GetTime(true),
                    EndTime = userInput.GetTime(false)
                };
                connection.Execute(sql,new { StartTime = session.StartTime, Endtime = session.EndTime, Duration = session.Duration });
            }
            Console.WriteLine("Row inserted");
        }

        public void DeleteRecord()
        {

        }

        public void UpdateRecord()
        {

        }
    }
}