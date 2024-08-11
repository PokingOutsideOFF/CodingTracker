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
        private string? sessionDatabaseConnection = ConfigurationManager.AppSettings.Get("SessionDBConnection");

        public void CreateTable()
        {
            using (var connection = new SQLiteConnection(sessionDatabaseConnection))
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
            using (var connection = new SQLiteConnection(sessionDatabaseConnection))
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
                using (var connection = new SQLiteConnection(sessionDatabaseConnection))
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
                AnsiConsole.Markup("\n\n[blue]Row inserted[/]\n\n");
                var sessions = ViewSessionsTable();
                DisplaySessionTable(sessions);
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                AnsiConsole.Markup($"\n[red]{ex.Message}[/]");
            }
        }

        public void DeleteSession()
        {
            var userInput = new UserInput();
            var sessions = ViewSessionsTable();
            DisplaySessionTable(sessions);

            using (var connection = new SQLiteConnection(sessionDatabaseConnection))
            {
                var sql = "DELETE FROM codeSession WHERE id = @Id";
                Console.Write("\nEnter id to be deleted: ");
                int id = userInput.GetIntValue();

                if (!CheckIdExists(id))
                {
                    AnsiConsole.Markup("[red]ID doesnt exists. Returning to Main Menu.[/]\n");
                    Thread.Sleep(1000);
                    return;
                }

                connection.Execute(sql, new { Id = id });
            }
            AnsiConsole.Markup("\n[red]Row deleted[/]\n");
        }

        public void UpdateSessionRecord()
        {
            var userInput = new UserInput();
            var sessions = ViewSessionsTable();
            DisplaySessionTable(sessions);
            Console.Write("Enter id: ");
            int id = userInput.GetIntValue();

            using (var connection = new SQLiteConnection(sessionDatabaseConnection))
            {
                connection.Open();
                string query = "UPDATE codeSession SET codingGoal=@Task WHERE id = @Id";
                string task = userInput.GetTask();
                connection.Execute(query, new { Task = task, Id = id });
            }

            AnsiConsole.Markup("\n[blue]Row updated[/]\n");


            sessions = ViewSessionsTable();
            DisplaySessionTable(sessions);
        }

        public void DisplaySessionTable(List<CodingSession> sessions)
        {
            Console.WriteLine("Your Sessions\n");
            if (sessions.Count == 0)
            {
                AnsiConsole.Markup("[red]No records found[/]. Add by starting a session.");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Coding Task");
            table.AddColumn("Start Time");
            table.AddColumn("End Time");
            table.AddColumn("Duration");
            foreach (var session in sessions)
            {
                int totalHours = (int)session.Duration.TotalHours; // Total hours as an integer
                int minutes = session.Duration.Minutes;
                table.AddRow(
                    session.Id.ToString(),
                    session.CodingGoal,
                    session.StartTime.ToString(),
                    session.EndTime.ToString(),
                    $"{totalHours:D2}:{minutes:D2} hours");
            }
            AnsiConsole.Write(table);
        }

        public bool CheckIdExists(int id)
        {
            using (var connection = new SQLiteConnection(sessionDatabaseConnection))
            {
                string sql = "SELECT COUNT(1) FROM codeSession WHERE id= @Id";
                int count = connection.ExecuteScalar<int>(sql, new { Id = id });
                return count > 0;
            }

        }

        //Challenge 2 - Let the users filter their coding records per period
        //(weeks, days, years) and/or order ascending or descending.

        public void FilterByDay()
        {
            using(var connection = new SQLiteConnection(sessionDatabaseConnection))
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
            using(var connection = new SQLiteConnection(sessionDatabaseConnection))
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

        public void FilterByYear()
        {
            using(var connection = new SQLiteConnection(sessionDatabaseConnection))
            {
                string query = @"
                    SELECT strftime('%Y', startTime) AS year,
                    COUNT(*) AS session_count
                    FROM codeSession
                    GROUP BY year
                    ORDER BY year
                    ";

                var result = connection.Query(query).ToList();

                var table = new Table();
                table.AddColumn("Year");
                table.AddColumn("No. of Sessions");

                foreach (var row in result)
                {
                    string year = row.year;
                    long session_count = row.session_count;
         
                    table.AddRow(
                        year,
                        session_count.ToString());
                }

                AnsiConsole.Write(table);
            }
        }

        
    }
}