using System.Configuration;
using System.Data.SQLite;
using CodingSessionLibrary;
using CodingGoalLibrary;
using Spectre.Console;
using Dapper;


namespace CodingTrackerDatabaseLibrary
{
    public class SessionDatabase
    {
        private string? databseConnection = ConfigurationManager.AppSettings.Get("SessionDBConnection");

        public void CreateTable()
        {
            using (var connection = new SQLiteConnection(databseConnection))
            {
                connection.Open();
                string createCodeSessionTableQuery = @"
                    CREATE TABLE IF NOT EXISTS codeSession(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    codingGoal TEXT NOT NULL,
                    startTime TEXT NOT NULL, 
                    endTime TEXT NOT NULL,
                    duration TEXT NOT NULL);";
                connection.Execute(createCodeSessionTableQuery);
            }

        }

        public List<CodingSession> ViewSessionsTable()
        {
            using (var connection = new SQLiteConnection(databseConnection))
            {
                var sql = "SELECT * FROM codeSession";
                return connection.Query<CodingSession>(sql).AsList();
            }
        }

        public void InsertSession()
        {
            var userInput = new UserInput();
            try
            {
                using (var connection = new SQLiteConnection(databseConnection))
                {
                    var sql = "INSERT INTO codeSession(codingGoal, startTime, endTime, duration)  VALUES(@CodingGoal, @StartTime, @EndTime, @Duration);";
                    var goal = userInput.GetTask();
                    var sessionTime = userInput.GetSessionTime();
                    var session = new CodingSession()
                    {
                        Id = 1,
                        CodingGoal = goal,
                        StartTime = sessionTime.Start,
                        EndTime = sessionTime.End,
                    };
                    connection.Execute(sql, session);
                }
                Console.WriteLine("\nRow inserted\n");
                var sessions = ViewSessionsTable();
                DisplaySessionTable(sessions);
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                AnsiConsole.Markup($"\n[red]{ex.Message}[/]");
            }
        }


        public void DisplaySessionTable(List<CodingSession> sessions)
        {
            if (sessions.Count == 0)
            {
                AnsiConsole.Markup("[red]No records found[/]. Add by starting a session.");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Coding Goal");
            table.AddColumn("Start Time");
            table.AddColumn("End Time");
            table.AddColumn("Duration");
            foreach (var session in sessions)
            {
                table.AddRow(
                    session.Id.ToString(),
                    session.CodingGoal,
                    session.StartTime.ToString(),
                    session.EndTime.ToString(),
                    $"{session.Duration.Days} days {session.Duration.Hours:D2}:{session.Duration.Minutes:D2} hours");
            }
            AnsiConsole.Write(table);
            AnsiConsole.Markup("[blue]Press enter key to return to menu[/]");
        }
    }
}