using System.ComponentModel.Design;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using CodingGoalLibrary;
using CodingSessionLibrary;
using Dapper;
using Spectre.Console;

namespace CodingTrackerDatabaseLibrary
{
    public class GoalsDatabase
    {
        private string? goalDatabaseConnection = ConfigurationManager.AppSettings.Get("GoalsDBConnection");
        private string? sessionDatabaseConnection = ConfigurationManager.AppSettings.Get("SessionDbConnection");

        public void CreateTable()
        {
            using (var connection = new SQLiteConnection(goalDatabaseConnection))
            {
                connection.Open();

                string createCodeGoalTableQuery = @"
                    CREATE TABLE IF NOT EXISTS codeGoal(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    codingTask TEXT NOT NULL,
                    codingGoal TEXT NOT NULL,
                    startDate TEXT NOT NULL,
                    endDate TEXT NOT NULL
                    );
                ";

                connection.Execute(createCodeGoalTableQuery);
            }
        }

        public List<CodingGoals> ViewGoalTable()
        {
            using (var connection = new SQLiteConnection(goalDatabaseConnection))
            {
                var sql = "SELECT * FROM codeGoal";
                var query =  connection.Query<CodingGoals>(sql).AsList();
                return query;
            }
        }

        public void InsertGoalRecord()
        {
            var userInput = new UserInput();
            try
            {

                using (var connection = new SQLiteConnection(goalDatabaseConnection))
                {
                    var sql = "INSERT INTO codeGoal(codingTask, codingGoal, startDate, endDate)  VALUES(@CodingTask, @CodingGoal, @StartDate, @EndDate);";
                    var session = new CodingGoals()
                    {
                        Id = 1,
                        CodingTask = userInput.GetTask(),
                        CodingGoal = userInput.GetGoal(),
                        StartDate = userInput.GetTime(true),
                        EndDate = userInput.GetTime(false)
                    };
                    int r = connection.Execute(sql,session);
                    Console.WriteLine($"\nRow inserted: {r}\n");

                    var goals = ViewGoalTable();
                    DisplayGoalTable(goals);
                }
            }
            catch(Exception ex)
            {
                AnsiConsole.Markup($"\n[red]{ex.Message}[/]");
            }
        }

        public void DeleteGoalRecord()
        {
            var userInput = new UserInput();
            var goals = ViewGoalTable();
            DisplayGoalTable(goals);
           
            using (var connection = new SQLiteConnection(goalDatabaseConnection))
            {
                var sql = "DELETE FROM codeGoal WHERE id = @Id";
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
            goals = ViewGoalTable();
            DisplayGoalTable(goals);
        }

        public void UpdateGoalRecord()
        {
            var userInput = new UserInput();
            int choice = userInput.UpdateGoalsRecordChoice();

            var goals = ViewGoalTable();
            DisplayGoalTable(goals);

            
            if (choice > 5)
            {
                AnsiConsole.Markup("[red]Invalid Option. Returning to Main Menu[/]\n");
                Thread.Sleep(1000);
                return;
            }

            using (var connection = new SQLiteConnection(goalDatabaseConnection))
            {
                Console.Write("Enter id: ");
                int id = userInput.GetIntValue();
                if (!CheckIdExists(id))
                {
                    AnsiConsole.Markup("[red]ID doesnt exists. Returning to Main Menu.[/]\n");
                    Thread.Sleep(1000);
                    return;
                }
                
                try
                {
                    if(choice == 1)
                    {
                        string sql = "UPDATE codeGoal SET codingTask = @CodingTask WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            CodingTask = userInput.GetTask()
                        };
                        connection.Execute(sql, new { CodingTask = session.CodingTask, Id = id });
                    }
                    else if (choice == 2)
                    {
                        string sql = "UPDATE codeGoal SET codingTask = @CodingGoal WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            CodingGoal = userInput.GetGoal()
                        };
                        connection.Execute(sql, new { CodingTask = session.CodingGoal, Id = id });
                    }
                    else if (choice == 3) {

                        string sql = "UPDATE codeGoal SET startDate = @StartDate WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            StartDate = userInput.GetTime(true),
                        };
                        connection.Execute(sql, new { StartDate = session.StartDate, Id = id });

                    }
                    else if (choice == 4)
                    {
                        string sql = "UPDATE codeGoal SET endDate = @EndDate WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            StartDate = DateTime.Now,
                            EndDate = userInput.GetTime(false),
                        };
                        connection.Execute(sql, new { EndDate = session.EndDate, Id = id });
                    }
                    else if (choice == 5)
                    {
                        string sql = "UPDATE codeGoal SET startDate=@StartDate AND endDate = @EndDate WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            StartDate = userInput.GetTime(true),
                            EndDate = userInput.GetTime(false),
                        };
                        connection.Execute(sql, new { StartTime = session.StartDate, EndTime = session.EndDate, Id = id });
                    }

                    Console.WriteLine("\nRow updated\n");
                    ViewGoalTable();
                }
                catch(Exception ex)
                {
                    AnsiConsole.Markup($"\n[red]{ex.Message}[/]\n");
                }
                goals = ViewGoalTable();
                DisplayGoalTable(goals);
            }
        }

        public bool CheckIdExists(int id)
        {
            using (var connection = new SQLiteConnection(goalDatabaseConnection))
            {
                string sql = "SELECT COUNT(1) FROM codeGoal WHERE id= @Id";
                int count = connection.ExecuteScalar<int>(sql, new { Id = id });
                return count > 0;
            }
        }

        public void DisplayGoalTable(List<CodingGoals> goals)
        {
            Console.WriteLine("Your Goals\n");
            if (goals.Count == 0)
            {
                AnsiConsole.Markup("[red]No records found[/]. Add by starting a session.");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Coding Task");
            table.AddColumn("Coding Goal");
            table.AddColumn("Start Date");
            table.AddColumn("End Date");
            foreach (var session in goals)
            {
                table.AddRow(
                    session.Id.ToString(),
                    session.CodingTask,
                    session.CodingGoal,
                    session.StartDate.Date.ToShortDateString(),
                    session.EndDate.Date.ToShortDateString());
            }
            AnsiConsole.Write(table);
     
        }

        //Challenge 4 - reate the ability to set coding goals and show how far the users are from reaching their goal,
        //along with how many hours a day they would have to code to reach their goal.
        public void ProgressReport()
        {
            Console.WriteLine("Progress Report");
            var userInput = new UserInput();
            string task = userInput.GetTask();
            string goal = "";
            string dateExceeded;
            double progressPercent;
            double hours = 0;
            double hoursPerDay = 0;

            List<dynamic> sessionTimeSpentResult;
            List<dynamic> sessionEndTimeResult;
            List<dynamic> goalEnquiryResult;

            using(var connection = new SQLiteConnection(sessionDatabaseConnection))
            { 
                string query = @"
                        SELECT codingGoal,
                            SUM((strftime('%s', endTime) - strftime('%s', startTime))) AS total_hours
                        FROM codeSession
                        WHERE codingGoal = @Task
                        GROUP BY codingGoal;";

                sessionTimeSpentResult = connection.Query(query, new { Task = task }).AsList();
                
                if(sessionTimeSpentResult.Count == 0)
                {
                    Console.WriteLine("No sessions found");
                    return;
                }

                string endTimeQuery = @"
                            SELECT MAX(endTime) as last_date
                            FROM codeSession
                            WHERE codingGoal = @Task";
                sessionEndTimeResult = connection.Query(endTimeQuery, new { Task = task }).AsList();
            }

            using (var connection = new SQLiteConnection(goalDatabaseConnection))
            {
                string query = @"
                        SELECT codingTask, codingGoal, endDate
                        FROM codeGoal
                        WHERE codingTask = @Task;";
                goalEnquiryResult = connection.Query(query, new { Task = task }).AsList();

                if(goalEnquiryResult.Count == 0)
                {
                    Console.WriteLine("No coding goals registered yet.");
                    return;
                }
            }

            DateTime sessionEndTime = DateTime.Now;
            DateTime goalEndTime = DateTime.Now;

            foreach(var row in sessionTimeSpentResult)
            {
                hours = row.total_hours / 3600.0;
            }

            foreach(var row in sessionEndTimeResult)
            {
                string s = row.last_date;
                sessionEndTime = DateTime.Parse(s);
            }

            foreach (var row in goalEnquiryResult)
            {
                string s = row.endDate;
                goalEndTime = DateTime.Parse(s);
                goal = row.codingGoal;
            }

            dateExceeded = DateTime.Now > goalEndTime ? "yes" : "no";
            if(dateExceeded == "no")
            {
                TimeSpan timeLeft = goalEndTime - DateTime.Now;

                double daysLeft = timeLeft.TotalDays < 1 ? 1 : timeLeft.TotalDays;
                hoursPerDay = (double.Parse(goal) - hours) / daysLeft;  
            }

            var table = new Table();
            table.AddColumn("Coding Task");
            table.AddColumn("Codign Goal (hours)");
            table.AddColumn("Time Spent (hours)");
            table.AddColumn("Progress");
            table.AddColumn("End Date Exceeded?");
            if(dateExceeded == "no")
            {
                table.AddColumn("Hours Per Day Required");
            }
            
            progressPercent = (hours / double.Parse(goal)) * 100;
            if(progressPercent >= 100.00)
            {
                progressPercent = 100.00;
            }
            if(dateExceeded == "yes")
            {
                table.AddRow(
                        task,
                        goal,
                        hours.ToString("F2"),
                        progressPercent.ToString("F2"),
                        dateExceeded);
            }
            else
            {
                table.AddRow(
                        task,
                        goal,
                        hours.ToString("F2"),
                        progressPercent.ToString("F2"),
                        dateExceeded,
                        hoursPerDay.ToString("F2"));
            }
            
            AnsiConsole.Write(table);
        }
    }
}

