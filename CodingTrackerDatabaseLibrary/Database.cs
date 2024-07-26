using System.Configuration;
using System.Data.SQLite;
using CodingSessionLibrary;
using Spectre.Console;
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
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Start Time");
            table.AddColumn("End Time");
            table.AddColumn("Duration");

            using (var connection = new SQLiteConnection(databseConnection))
            {
                var sql = "SELECT * FROM codeLog";
                var codingSession = connection.Query<CodingSession>(sql);
                //Console.WriteLine("ID\t\tStart Time\t\tEnd Time\tDuration");
                foreach(var session in codingSession)
                {
                    table.AddRow(
                        session.Id.ToString(),
                        session.StartTime.ToString(),
                        session.EndTime.ToString(),
                        $"{session.Duration.Days} days {session.Duration.Hours:D2}:{session.Duration.Minutes:D2} hours");
                }
            } 
            AnsiConsole.Write(table);
        }

        public void InsertRecord()
        {
            var userInput = new UserInput();
            using(var connection = new SQLiteConnection(databseConnection))
            {
                var sql = "INSERT INTO codeLog(startTime, endTime, duration)  VALUES(@StartTime, @EndTime, @Duration);";
                var session = new CodingSession()
                {
                    Id = 1,
                    StartTime = userInput.GetTime(true),
                    CodingGoal = userInput.GetGoal();
                    EndTime = userInput.GetTime(false)
                };
                connection.Execute(sql, session);
            }
            Console.WriteLine("Row inserted\n");
            ViewTable();
        }

        public void DeleteRecord()
        {
            var userInput = new UserInput();
            ViewTable();
            using(var connection = new SQLiteConnection(databseConnection))
            {
                var sql = "DELETE FROM codeLog WHERE id = @Id";
                Console.Write("Enter id to be deleted: ");
                int id = userInput.GetIntValue();
                if (!CheckIdExists(id))
                {
                    AnsiConsole.Markup("[red]ID doesnt exists. Returning to Main Menu.[/]\n");
                    Thread.Sleep(1000);
                    return;
                }
                connection.Execute(sql, new { Id = id });
            }
            Console.WriteLine("Row deleted\n");
            ViewTable();
        }

        public void UpdateRecord()
        {
            var userInput = new UserInput();
            Console.WriteLine("\nWhat do you want to update?");
            Console.WriteLine("1. Start Time");
            Console.WriteLine("2. End Time");
            Console.WriteLine("3. Both");
            Console.Write("Enter choice: ");
            int choice = userInput.GetIntValue();
            if(choice > 3)
            {
                AnsiConsole.Markup("[red]Invalid Option. Returning to Main Menu[/]\n");
                Thread.Sleep(1000);
                return;
            }
            using (var connection = new SQLiteConnection(databseConnection))
            {
                Console.Write("Enter id: ");
                int id = userInput.GetIntValue();
                if (!CheckIdExists(id))
                {
                    AnsiConsole.Markup("[red]ID doesnt exists. Returning to Main Menu.[/]\n");
                    Thread.Sleep(1000);
                    return ;
                }
                if(choice == 1)
                {
                    string sql = "UPDATE codeLog SET startTime = @StartTime WHERE id = @Id";
                    var session = new CodingSession()
                    {
                        StartTime = userInput.GetTime(true),
                    };
                    connection.Execute(sql, new {StartTime = session.StartTime, Id = id});
                }
                else if(choice == 2)
                {
                    string sql = "UPDATE codeLog SET endTime = @EndTime WHERE id = @Id";
                    var session = new CodingSession()
                    {
                        EndTime = userInput.GetTime(false),
                    };
                    connection.Execute(sql, new { EndTime = session.EndTime, Id = id });
                }
                else
                {
                    string sql = "UPDATE codeLog SET startTime=@StartTime AND endTime = @EndTime WHERE id = @Id";
                    var session = new CodingSession()
                    {
                        StartTime = userInput.GetTime(true),
                        EndTime = userInput.GetTime(false),
                    };
                    connection.Execute(sql, new { StartTime = session.StartTime, EndTime = session.EndTime, Id = id });
                }
            }
        }

        public bool CheckIdExists(int id)
        {
            using(var connection = new SQLiteConnection(databseConnection))
            {
                string sql = "SELECT COUNT(1) FROM codeLog WHERE id= @Id";
                int count = connection.ExecuteScalar<int>(sql, new { Id = id });
                return count > 0;
            }
            
        }
    }
}