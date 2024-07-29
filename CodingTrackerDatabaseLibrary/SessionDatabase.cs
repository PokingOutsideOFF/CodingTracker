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
        }

        public void FilterByWeek()
        {
            using(var connection = new SQLiteConnection(databseConnection))
            {
                string query = @"
                    SELECT strftime('%w', startTime) AS day_of_week,
                    CASE strftime('%w', startTime)
                        WHEN '0' THEN 'SUNDAY'
                        WHEN '1' THEN 'MONDAY'
                        WHEN '2' THEN 'TUESDAY'
                        WHEN '3' THEN 'WEDNESDAY'
                        WHEN '4' THEN 'THURSDAY'
                        WHEN '5' THEN 'FRIDAY'
                        WHEN '6' THEN 'SATURDAY'
                    END as day_name,
                    COUNT(*) AS session_count
                    FROM codeSession
                    GROUP BY day_of_week
                    ORDER BY day_of_week";
                var result = connection.Query(query).AsList();


                var table = new Table();
                table.AddColumn("Day of the Week");
                table.AddColumn("No. of Sessions");
                foreach (var row in result)
                {
                    string day_name = row.day_name;
                    long session_count = row.session_count;
                    table.AddRow(
                        day_name,
                        session_count.ToString());   
                }
                AnsiConsole.Write(table);
            }
        }


        public void FilterByMonths()
        {
            using(var connection = new SQLiteConnection(databseConnection))
            {
                string query = @"
                    SELECT strftime('%m', startTime) AS month,
                    CASE strftime('%m', startTime)
                        WHEN '01' THEN 'JANUARY'
                        WHEN '02' THEN 'FEBRUARY'
                        WHEN '03' THEN 'MARCH'
                        WHEN '04' THEN 'APRIL'
                        WHEN '05' THEN 'MAY'
                        WHEN '06' THEN 'JUNE'
                        WHEN '07' THEN 'JULY'
                        WHEN '08' THEN 'AUGUST'
                        WHEN '09' THEN 'SEPTEMBER'
                        WHEN '10' THEN 'OCTOBER'
                        WHEN '11' THEN 'NOVEMBER'
                        WHEN '12' THEN 'DECEMBER'
                    END as month_name,
                    COUNT(*) AS session_count
                    FROM codeSession
                    GROUP BY month
                    ORDER BY month";

                var result = connection.Query(query).AsList();

                var table = new Table();
                table.AddColumn("Month");
                table.AddColumn("No. of Sessions");

                foreach (var row in result)
                {
                    string month_name = row.month_name;
                    long session_count = row.session_count;
                    table.AddRow(
                        month_name,
                        session_count.ToString());
                }
                AnsiConsole.Write(table);
            }
        }
    }
}